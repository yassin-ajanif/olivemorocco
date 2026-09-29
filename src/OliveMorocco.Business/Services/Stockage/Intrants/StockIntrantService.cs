using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Stockage.Intrants;

/// <summary>
/// Owns the intrant stock ledger: <see cref="Intrant"/> stock is the sum of its
/// <see cref="MouvementIntrant"/> rows (no persisted stock column), and every source —
/// intervention, manual adjustment — writes through this service.
/// </summary>
public sealed class StockIntrantService : IStockIntrantService
{
    /// <summary>Intrant used in an intervention (out), or its correction / reversal on edit / delete.</summary>
    public const string OrigineIntervention = "Intervention";

    /// <summary>Manual adjustment of intrant stock.</summary>
    public const string OrigineImport = "Import";

    private readonly IRepository<Intrant> _intrants;
    private readonly IRepository<MouvementIntrant> _mouvements;
    private readonly IValidator<CreateAjustementIntrantDto>? _ajustementValidator;

    public StockIntrantService(
        IRepository<Intrant> intrants,
        IRepository<MouvementIntrant> mouvements,
        IEnumerable<IValidator<CreateAjustementIntrantDto>> ajustementValidators)
    {
        _intrants = intrants;
        _mouvements = mouvements;
        _ajustementValidator = ajustementValidators.FirstOrDefault();
    }

    public async Task<PagedResult<IntrantStockListItemDto>> GetStockIntrantAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _intrants.QueryPagedAsync(
            i => pattern == null
                 || EF.Functions.ILike(i.Nom, pattern),
            query => query
                .OrderBy(i => i.Nom),
            i => new IntrantStockListItemDto(
                i.Id,
                i.Nom,
                i.Unite,
                ComputeStock(i),
                i.PrixAchatHT),
            page,
            pageSize,
            cancellationToken,
            [i => i.MouvementsIntrant]);

        return new PagedResult<IntrantStockListItemDto>(items, totalCount);
    }

    public async Task<IntrantStockDetailDto?> GetDetailAsync(
        int intrantId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _intrants.GetByIdWithNavigationsAsync(
            intrantId,
            [i => i.MouvementsIntrant],
            cancellationToken);

        return entity is null ? null : new IntrantStockDetailDto(
            entity.Id,
            entity.Nom,
            entity.Unite,
            ComputeStock(entity),
            entity.PrixAchatHT);
    }

    public async Task<PagedResult<MouvementIntrantListItemDto>> GetMouvementsAsync(
        int intrantId,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        if (!await _intrants.AnyAsync(i => i.Id == intrantId, cancellationToken))
            throw new KeyNotFoundException($"Intrant {intrantId} introuvable.");

        var (items, totalCount) = await _mouvements.QueryPagedAsync(
            m => m.IntrantId == intrantId,
            query => query.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id),
            m => new MouvementIntrantListItemDto(
                m.Id,
                m.CreatedAt,
                m.Type,
                m.Quantite,
                m.StockAvant,
                ComputeStockApres(m.Type, m.StockAvant, m.Quantite),
                m.OrigineType,
                m.OrigineId,
                FormatOrigineLabel(m.OrigineType, m.OrigineId),
                m.Note),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<MouvementIntrantListItemDto>(items, totalCount);
    }

    public async Task CreateAjustementAsync(
        int intrantId,
        CreateAjustementIntrantDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_ajustementValidator, dto, cancellationToken);

        var intrant = await LoadIntrantAsync(intrantId, cancellationToken);

        var mouvement = BuildMouvement(
            intrant,
            dto.Variation,
            OrigineImport,
            origineId: null,
            dto.Note,
            nameof(dto.Variation));

        await _mouvements.AddAsync(mouvement, cancellationToken);
    }

    public async Task ApplyInterventionSortieAsync(
        int interventionId,
        IEnumerable<(int IntrantId, decimal Quantite)> lignes,
        CancellationToken cancellationToken = default)
    {
        foreach (var (intrantId, quantite) in lignes)
        {
            if (quantite <= 0)
                continue;

            await ApplyMouvementAsync(
                intrantId,
                -quantite,
                OrigineIntervention,
                interventionId,
                string.Empty,
                "intrant",
                cancellationToken);
        }
    }

    public async Task ApplyInterventionAjustementAsync(
        int interventionId,
        IEnumerable<(int IntrantId, decimal Delta)> deltas,
        CancellationToken cancellationToken = default)
    {
        foreach (var (intrantId, delta) in deltas)
        {
            if (delta == 0)
                continue;

            await ApplyMouvementAsync(
                intrantId,
                delta,
                OrigineIntervention,
                interventionId,
                "Modification de l'intervention",
                "intrant",
                cancellationToken);
        }
    }

    public async Task ReverseInterventionAsync(
        int interventionId,
        IEnumerable<(int IntrantId, decimal Quantite)> lignes,
        CancellationToken cancellationToken = default)
    {
        foreach (var (intrantId, quantite) in lignes)
        {
            if (quantite <= 0)
                continue;

            await ApplyMouvementAsync(
                intrantId,
                quantite,
                OrigineIntervention,
                interventionId,
                "Suppression de l'intervention",
                string.Empty,
                cancellationToken);
        }
    }

    public async Task ApplyMouvementAsync(
        int intrantId,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName,
        CancellationToken cancellationToken = default)
    {
        if (variation == 0)
            return;

        var intrant = await LoadIntrantAsync(intrantId, cancellationToken);

        var mouvement = BuildMouvement(
            intrant,
            variation,
            origineType,
            origineId,
            note,
            errorPropertyName);

        await _mouvements.AddAsync(mouvement, cancellationToken);
    }

    private async Task<Intrant> LoadIntrantAsync(int intrantId, CancellationToken cancellationToken) =>
        await _intrants.GetByIdWithNavigationsAsync(
            intrantId, [i => i.MouvementsIntrant], cancellationToken)
        ?? throw new KeyNotFoundException($"Intrant {intrantId} introuvable.");

    /// <summary>
    /// Intrant stock is derived from the movement ledger only — nothing is stored on the intrant,
    /// so <see cref="Intrant.MouvementsIntrant"/> must be loaded for the result to be meaningful.
    /// </summary>
    public static decimal ComputeStock(Intrant intrant) =>
        intrant.MouvementsIntrant.Sum(m => m.Type == TypeMouvement.Entree ? m.Quantite : -m.Quantite);

    private static MouvementIntrant BuildMouvement(
        Intrant intrant,
        decimal variation,
        string origineType,
        int? origineId,
        string? note,
        string errorPropertyName)
    {
        var stockAvant = ComputeStock(intrant);
        var nouveauStock = stockAvant + variation;

        if (nouveauStock < 0)
        {
            throw new ValidationException([
                new ValidationFailure(
                    errorPropertyName,
                    BuildStockInsuffisantMessage(intrant, stockAvant, variation, origineType)),
            ]);
        }

        return new MouvementIntrant
        {
            IntrantId = intrant.Id,
            Type = variation > 0 ? TypeMouvement.Entree : TypeMouvement.Sortie,
            Quantite = Math.Abs(variation),
            StockAvant = stockAvant,
            OrigineType = origineType,
            OrigineId = origineId,
            Note = note?.Trim() ?? string.Empty,
        };
    }

    private static string BuildStockInsuffisantMessage(
        Intrant intrant,
        decimal stockAvant,
        decimal variation,
        string origineType) =>
        origineType switch
        {
            OrigineImport => "Le stock ne peut pas devenir négatif.",
            _ => $"Stock insuffisant pour « {intrant.Nom} » : {stockAvant:N0} en stock, {Math.Abs(variation):N0} demandés.",
        };

    private static decimal ComputeStockApres(TypeMouvement type, decimal stockAvant, decimal quantite) =>
        type switch
        {
            TypeMouvement.Entree => stockAvant + quantite,
            TypeMouvement.Sortie => stockAvant - quantite,
            _ => stockAvant + quantite,
        };

    private static string FormatOrigineLabel(string origineType, int? origineId) =>
        origineType switch
        {
            OrigineIntervention => origineId.HasValue ? $"Intervention #{origineId}" : "Intervention",
            OrigineImport => "Ajustement manuel",
            _ => origineId.HasValue ? $"{origineType} #{origineId}" : origineType,
        };

    private static async Task ValidateAsync<T>(
        IValidator<T>? validator,
        T dto,
        CancellationToken cancellationToken)
    {
        if (validator is null)
            return;

        var result = await validator.ValidateAsync(dto, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
