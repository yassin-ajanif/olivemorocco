using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using OliveMorocco.Business.DTOs;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Business.Services.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Operationnel;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.Business.Services.Operationnel;

public sealed class RemplissageService : IRemplissageService
{
    private const string NoteModification = "Modification du remplissage";
    private const string NoteSuppression = "Suppression du remplissage";

    private readonly IRepository<Remplissage> _remplissages;
    private readonly IRepository<Variete> _varietes;
    private readonly IRepository<Produit> _produits;
    private readonly IRepository<MouvementStockVariete> _mouvementsHuile;
    private readonly IRepository<MouvementStock> _mouvementsProduit;
    private readonly IValidator<CreateRemplissageDto>? _createValidator;
    private readonly IValidator<UpdateRemplissageDto>? _updateValidator;

    public RemplissageService(
        IRepository<Remplissage> remplissages,
        IRepository<Variete> varietes,
        IRepository<Produit> produits,
        IRepository<MouvementStockVariete> mouvementsHuile,
        IRepository<MouvementStock> mouvementsProduit,
        IEnumerable<IValidator<CreateRemplissageDto>> createValidators,
        IEnumerable<IValidator<UpdateRemplissageDto>> updateValidators)
    {
        _remplissages = remplissages;
        _varietes = varietes;
        _produits = produits;
        _mouvementsHuile = mouvementsHuile;
        _mouvementsProduit = mouvementsProduit;
        _createValidator = createValidators.FirstOrDefault();
        _updateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<PagedResult<RemplissageListItemDto>> GetRemplissagesAsync(
        string? search = null,
        int page = 1,
        int pageSize = 15,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var q = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        var pattern = q is null ? null : $"%{q}%";

        var (items, totalCount) = await _remplissages.QueryPagedAsync(
            r => pattern == null
                 || EF.Functions.ILike(r.Numero, pattern)
                 || EF.Functions.ILike(r.Variete.Nom, pattern)
                 || r.Lignes.Any(l => EF.Functions.ILike(l.Produit.Reference, pattern)
                                      || EF.Functions.ILike(l.Produit.Designation, pattern)),
            query => query.OrderByDescending(r => r.Date).ThenByDescending(r => r.Id),
            r => new RemplissageListItemDto(
                r.Id,
                r.Numero,
                r.Date,
                r.Variete.Nom,
                !r.Lignes.Any()
                    ? "—"
                    : string.Join("; ", r.Lignes
                        .OrderBy(l => l.Produit.Reference)
                        .Select(l => l.Produit.Reference + " × " + ((int)l.Quantite).ToString())),
                r.QuantiteHuile,
                r.Perte),
            page,
            pageSize,
            cancellationToken);

        return new PagedResult<RemplissageListItemDto>(items, totalCount);
    }

    public async Task<RemplissageDto?> GetRemplissageByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _remplissages.GetByIdWithNavigationsAsync(
            id,
            [r => r.Variete, r => r.Lignes],
            cancellationToken);

        if (entity is null)
            return null;

        var produitIds = entity.Lignes.Select(l => l.ProduitId).Distinct().ToList();
        var produits = produitIds.Count == 0
            ? []
            : await _produits.FindAsync(p => produitIds.Contains(p.Id), cancellationToken);
        var produitMap = produits.ToDictionary(p => p.Id);

        var lignes = entity.Lignes
            .OrderBy(l => produitMap[l.ProduitId].Reference)
            .Select(l => new RemplissageLigneDto(
                l.Id,
                l.ProduitId,
                produitMap[l.ProduitId].Reference,
                produitMap[l.ProduitId].Designation,
                l.Quantite,
                l.ContenanceLitres,
                l.Litres))
            .ToList();

        return new RemplissageDto(
            entity.Id,
            entity.Numero,
            entity.VarieteId,
            entity.Variete.Nom,
            entity.Date,
            entity.QuantiteHuile,
            entity.Perte,
            entity.Note,
            lignes);
    }

    public async Task<RemplissageDto> CreateRemplissageAsync(
        CreateRemplissageDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);

        var variete = await GetVarieteAsync(dto.VarieteId, cancellationToken);
        var lignes = await BuildLignesAsync(dto.VarieteId, dto.Lignes, cancellationToken);
        var quantiteHuile = lignes.Sum(l => l.Litres) + dto.Perte;

        EnsureStockHuileSuffisant(variete, quantiteHuile);

        var entity = new Remplissage
        {
            Numero = await GenerateNumeroAsync(cancellationToken),
            VarieteId = dto.VarieteId,
            Date = dto.Date,
            QuantiteHuile = quantiteHuile,
            Perte = dto.Perte,
            Note = NormalizeNote(dto.Note),
            Lignes = lignes,
        };

        await _remplissages.ExecuteInTransactionAsync(async ct =>
        {
            await _remplissages.AddAsync(entity, ct);
            await ApplyHuileAsync(variete, -quantiteHuile, entity.Id, string.Empty, ct);

            foreach (var ligne in lignes)
                await ApplyProduitAsync(ligne.ProduitId, ligne.Quantite, entity.Id, string.Empty, ct);
        }, cancellationToken);

        return (await GetRemplissageByIdAsync(entity.Id, cancellationToken))!;
    }

    public async Task UpdateRemplissageAsync(
        int id,
        UpdateRemplissageDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_updateValidator, dto, cancellationToken);

        var entity = await _remplissages.GetByIdWithNavigationsAsync(id, [r => r.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Remplissage {id} introuvable.");

        var nouvelleVariete = await GetVarieteAsync(dto.VarieteId, cancellationToken);
        var nouvellesLignes = await BuildLignesAsync(dto.VarieteId, dto.Lignes, cancellationToken);
        var nouvelleHuile = nouvellesLignes.Sum(l => l.Litres) + dto.Perte;

        var ancienneVarieteId = entity.VarieteId;
        var ancienneHuile = entity.QuantiteHuile;

        var produitDeltas = new Dictionary<int, decimal>();
        foreach (var ligne in entity.Lignes)
            produitDeltas[ligne.ProduitId] = produitDeltas.GetValueOrDefault(ligne.ProduitId) - ligne.Quantite;
        foreach (var ligne in nouvellesLignes)
            produitDeltas[ligne.ProduitId] = produitDeltas.GetValueOrDefault(ligne.ProduitId) + ligne.Quantite;

        await _remplissages.ExecuteInTransactionAsync(async ct =>
        {
            if (ancienneVarieteId == dto.VarieteId)
            {
                await ApplyHuileAsync(nouvelleVariete, ancienneHuile - nouvelleHuile, id, NoteModification, ct);
            }
            else
            {
                var ancienneVariete = await GetVarieteAsync(ancienneVarieteId, ct);
                await ApplyHuileAsync(ancienneVariete, ancienneHuile, id, NoteModification, ct);
                await ApplyHuileAsync(nouvelleVariete, -nouvelleHuile, id, NoteModification, ct);
            }

            foreach (var (produitId, delta) in produitDeltas)
                await ApplyProduitAsync(produitId, delta, id, NoteModification, ct);

            entity.Lignes.Clear();
            foreach (var ligne in nouvellesLignes)
            {
                ligne.RemplissageId = entity.Id;
                entity.Lignes.Add(ligne);
            }

            entity.VarieteId = dto.VarieteId;
            entity.Date = dto.Date;
            entity.Perte = dto.Perte;
            entity.QuantiteHuile = nouvelleHuile;
            entity.Note = NormalizeNote(dto.Note);

            await _remplissages.UpdateAsync(entity, ct);
        }, cancellationToken);
    }

    public async Task DeleteRemplissageAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _remplissages.GetByIdWithNavigationsAsync(id, [r => r.Lignes], cancellationToken)
            ?? throw new KeyNotFoundException($"Remplissage {id} introuvable.");

        await _remplissages.ExecuteInTransactionAsync(async ct =>
        {
            var variete = await GetVarieteAsync(entity.VarieteId, ct);
            await ApplyHuileAsync(variete, entity.QuantiteHuile, id, NoteSuppression, ct);

            foreach (var ligne in entity.Lignes)
                await ApplyProduitAsync(ligne.ProduitId, -ligne.Quantite, id, NoteSuppression, ct);

            await _remplissages.DeleteAsync(id, ct);
        }, cancellationToken);
    }

    public async Task<IReadOnlyList<RemplissageVarieteSelectItemDto>> GetVarietesForSelectAsync(
        CancellationToken cancellationToken = default)
    {
        var varietes = await _varietes.GetAllAsync(cancellationToken);
        return varietes
            .OrderBy(v => v.Nom)
            .Select(v => new RemplissageVarieteSelectItemDto(v.Id, v.Nom, v.StockHuile))
            .ToList();
    }

    public async Task<IReadOnlyList<RemplissageProduitSelectItemDto>> GetProduitsForSelectAsync(
        CancellationToken cancellationToken = default)
    {
        var produits = await _produits.FindAsync(
            p => p.Actif && p.ContenanceLitres != null && p.ContenanceLitres > 0,
            cancellationToken);

        return produits
            .OrderBy(p => p.Reference)
            .Select(p => new RemplissageProduitSelectItemDto(
                p.Id,
                p.VarieteId,
                p.Reference,
                p.Designation,
                p.Unite,
                p.ContenanceLitres!.Value,
                p.StockActuel))
            .ToList();
    }

    private async Task<string> GenerateNumeroAsync(CancellationToken cancellationToken)
    {
        var prefix = $"RMP-{DateTime.Today.Year}-";
        var existing = await _remplissages.FindAsync(r => r.Numero.StartsWith(prefix), cancellationToken);
        var next = existing
            .Select(r => int.TryParse(r.Numero[prefix.Length..], out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max() + 1;

        return $"{prefix}{next:D4}";
    }

    private async Task<Variete> GetVarieteAsync(int varieteId, CancellationToken cancellationToken)
    {
        return await _varietes.GetByIdAsync(varieteId, cancellationToken)
            ?? throw new ValidationException([
                new ValidationFailure(nameof(CreateRemplissageDto.VarieteId), "Variété introuvable."),
            ]);
    }

    private async Task<List<RemplissageLigne>> BuildLignesAsync(
        int varieteId,
        IReadOnlyList<CreateRemplissageLigneDto> lignes,
        CancellationToken cancellationToken)
    {
        var result = new List<RemplissageLigne>(lignes.Count);

        foreach (var ligne in lignes)
        {
            var produit = await _produits.GetByIdAsync(ligne.ProduitId, cancellationToken)
                ?? throw LigneError("Un produit sélectionné est introuvable.");

            if (produit.VarieteId != varieteId)
                throw LigneError($"Le produit « {produit.Reference} » n'appartient pas à la variété sélectionnée.");

            if (produit.ContenanceLitres is not > 0)
                throw LigneError($"Le produit « {produit.Reference} » n'a pas de contenance (L) — renseignez-la dans la fiche produit.");

            var contenance = produit.ContenanceLitres.Value;
            result.Add(new RemplissageLigne
            {
                ProduitId = produit.Id,
                Quantite = ligne.Quantite,
                ContenanceLitres = contenance,
                Litres = Math.Round(ligne.Quantite * contenance, 4),
            });
        }

        return result;
    }

    private static void EnsureStockHuileSuffisant(Variete variete, decimal quantiteHuile)
    {
        if (variete.StockHuile >= quantiteHuile)
            return;

        throw new ValidationException([
            new ValidationFailure(
                nameof(CreateRemplissageDto.Lignes),
                $"Stock d'huile insuffisant pour « {variete.Nom} » : {variete.StockHuile:N2} L disponibles, {quantiteHuile:N2} L requis."),
        ]);
    }

    private async Task ApplyHuileAsync(
        Variete variete,
        decimal variation,
        int remplissageId,
        string note,
        CancellationToken cancellationToken)
    {
        if (variation == 0)
            return;

        var mouvement = StockHuileMouvements.Apply(
            variete,
            variation,
            StockHuileMouvements.OrigineRemplissage,
            remplissageId,
            note,
            nameof(CreateRemplissageDto.Lignes),
            $"Stock d'huile insuffisant pour « {variete.Nom} » : {variete.StockHuile:N2} L disponibles, {-variation:N2} L requis.");

        await _mouvementsHuile.AddAsync(mouvement, cancellationToken);
    }

    private async Task ApplyProduitAsync(
        int produitId,
        decimal variation,
        int remplissageId,
        string note,
        CancellationToken cancellationToken)
    {
        if (variation == 0)
            return;

        var produit = await _produits.GetByIdAsync(produitId, cancellationToken)
            ?? throw new KeyNotFoundException($"Produit {produitId} introuvable.");

        var stockAvant = produit.StockActuel;
        var nouveauStock = stockAvant + variation;

        if (nouveauStock < 0)
        {
            throw LigneError(
                $"Stock insuffisant pour « {produit.Reference} » : {stockAvant:N0} en stock, ces unités ont déjà été vendues ou ajustées.");
        }

        produit.StockActuel = nouveauStock;

        await _mouvementsProduit.AddAsync(new MouvementStock
        {
            ProduitId = produit.Id,
            Type = variation > 0 ? TypeMouvement.Entree : TypeMouvement.Sortie,
            Quantite = Math.Abs(variation),
            StockAvant = stockAvant,
            OrigineType = StockHuileMouvements.OrigineRemplissage,
            OrigineId = remplissageId,
            Note = note,
        }, cancellationToken);
    }

    private static ValidationException LigneError(string message) =>
        new([new ValidationFailure(nameof(CreateRemplissageDto.Lignes), message)]);

    private static string? NormalizeNote(string? note) =>
        string.IsNullOrWhiteSpace(note) ? null : note.Trim();

    private static async Task ValidateAsync<T>(
        IValidator<T>? validator,
        T instance,
        CancellationToken cancellationToken)
    {
        if (validator is null)
            return;

        var result = await validator.ValidateAsync(instance, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
