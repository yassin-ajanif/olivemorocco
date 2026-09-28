using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Stockage;

/// <summary>
/// Owns the bulk-oil ledger of <see cref="Variete.StockHuile"/>: reads the per-variété stock,
/// lists <see cref="MouvementStockVariete"/> entries and applies the movements produced by
/// manual adjustments, pressages and remplissages (single writer of the ledger).
/// </summary>
public sealed class StockHuileService : IStockHuileService
{
    /// <summary>Oil produced by a pressage (in), or its correction / reversal on edit / delete.</summary>
    public const string OriginePressage = "Pressage";

    /// <summary>Manual adjustment of bulk oil stock.</summary>
    public const string OrigineImport = "Import";

    /// <summary>Oil filled into bottles (out), or its correction / reversal on edit / delete.</summary>
    public const string OrigineRemplissage = "Remplissage";

    private readonly IRepository<Variete> _varietes;
    private readonly IRepository<MouvementStockVariete> _mouvements;
    private readonly IValidator<CreateAjustementStockDto>? _ajustementValidator;

    public StockHuileService(
        IRepository<Variete> varietes,
        IRepository<MouvementStockVariete> mouvements,
        IEnumerable<IValidator<CreateAjustementStockDto>> ajustementValidators)
    {
        _varietes = varietes;
        _mouvements = mouvements;
        _ajustementValidator = ajustementValidators.FirstOrDefault();
    }

    public async Task<PagedResult<StockHuileListItemDto>> GetStockHuileAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _varietes.QueryPagedAsync(
            v => pattern == null
                 || EF.Functions.ILike(v.Nom, pattern)
                 || (v.Code != null && EF.Functions.ILike(v.Code, pattern)),
            query => query.OrderByDescending(v => v.StockHuile).ThenBy(v => v.Nom),
            v => new StockHuileListItemDto(v.Id, v.Nom, v.Code, v.StockHuile),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<StockHuileListItemDto>(items, totalCount);
    }

    public async Task<StockHuileDetailDto?> GetDetailAsync(
        int varieteId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _varietes.GetByIdAsync(varieteId, cancellationToken);
        return entity is null
            ? null
            : new StockHuileDetailDto(entity.Id, entity.Nom, entity.Code, entity.RegionOrigine, entity.StockHuile);
    }

    public async Task<PagedResult<MouvementStockListItemDto>> GetMouvementsAsync(
        int varieteId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        if (!await _varietes.AnyAsync(v => v.Id == varieteId, cancellationToken))
            throw new KeyNotFoundException($"Variété {varieteId} introuvable.");

        var (items, totalCount) = await _mouvements.QueryPagedAsync(
            m => m.VarieteId == varieteId,
            query => query.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id),
            m => new MouvementStockListItemDto(
                m.Id,
                m.CreatedAt,
                m.Type,
                m.Quantite,
                m.StockAvant,
                m.Type == TypeMouvement.Sortie ? m.StockAvant - m.Quantite : m.StockAvant + m.Quantite,
                m.OrigineType,
                m.OrigineId,
                FormatOrigineLabel(m.OrigineType, m.OrigineId),
                m.Note),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<MouvementStockListItemDto>(items, totalCount);
    }

    public async Task CreateAjustementAsync(
        int varieteId,
        CreateAjustementStockDto dto,
        CancellationToken cancellationToken = default)
    {
        if (_ajustementValidator is not null)
        {
            var result = await _ajustementValidator.ValidateAsync(dto, cancellationToken);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }

        await ApplyVarieteMouvementAsync(
            varieteId,
            dto.Variation,
            OrigineImport,
            origineId: null,
            dto.Note,
            nameof(CreateAjustementStockDto.Variation),
            cancellationToken);
    }

    public async Task ApplyVarieteMouvementAsync(
        int varieteId,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName,
        CancellationToken cancellationToken = default)
    {
        if (variation == 0)
            return;

        var variete = await _varietes.GetByIdAsync(varieteId, cancellationToken)
            ?? throw new KeyNotFoundException($"Variété {varieteId} introuvable.");

        var mouvement = BuildMouvement(
            variete,
            variation,
            origineType,
            origineId,
            note,
            errorPropertyName);

        await _mouvements.AddAsync(mouvement, cancellationToken);
    }

    /// <summary>
    /// Builds the movement and keeps <see cref="Variete.StockHuile"/> in sync.
    /// The caller persists the returned movement (which also saves the tracked variété).
    /// </summary>
    private static MouvementStockVariete BuildMouvement(
        Variete variete,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName)
    {
        var stockAvant = variete.StockHuile;
        var nouveauStock = stockAvant + variation;

        if (nouveauStock < 0)
        {
            throw new ValidationException([
                new ValidationFailure(
                    errorPropertyName,
                    $"Stock d'huile insuffisant pour « {variete.Nom} » : {stockAvant:N2} L disponibles, {Math.Abs(variation):N2} L requis."),
            ]);
        }

        variete.StockHuile = nouveauStock;

        return new MouvementStockVariete
        {
            VarieteId = variete.Id,
            Type = variation > 0 ? TypeMouvement.Entree : TypeMouvement.Sortie,
            Quantite = Math.Abs(variation),
            StockAvant = stockAvant,
            OrigineType = origineType,
            OrigineId = origineId,
            Note = note?.Trim() ?? string.Empty,
        };
    }

    private static string FormatOrigineLabel(string origineType, int? origineId) =>
        origineType switch
        {
            OriginePressage => origineId.HasValue ? $"Pressage #{origineId}" : "Pressage",
            OrigineRemplissage => origineId.HasValue ? $"Remplissage #{origineId}" : "Remplissage",
            OrigineImport => "Ajustement manuel",
            _ => origineId.HasValue ? $"{origineType} #{origineId}" : origineType,
        };
}
