using Microsoft.EntityFrameworkCore;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.DataAccess.Tests.Helpers;
using OliveMorocco.Domain.Entities.Common;
using OliveMorocco.Domain.Entities.Vente;
using OliveMorocco.Domain.Enums;

namespace OliveMorocco.DataAccess.Tests.Repositories;

[TestClass]
public sealed class RepositoryTests
{
    private sealed record TiersNameDto(string Nom);

    // ══════════════════════════════════════════════════════════════════════
    // GetByIdAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    //
    // Lookup by primary key (DbSet.FindAsync). Returns tracked entity or null.
    //
    // Covered:
    //   [x] id exists           → returns entity with correct data
    //   [x] id missing          → returns null
    //   [x] id zero             → returns null (no row with Id 0)
    //   [x] entity is tracked   → EntityState.Unchanged after Find
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task GetByIdAsync_returns_entity_when_exists()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var saved = await repo.AddAsync(new Tiers { Nom = "Client A", Type = TypeTiers.Client });

        var result = await repo.GetByIdAsync(saved.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual("Client A", result!.Nom);
    }

    [TestMethod]
    public async Task GetByIdAsync_returns_null_when_missing()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var result = await repo.GetByIdAsync(999);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByIdAsync_returns_null_for_zero_id()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var result = await repo.GetByIdAsync(0);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByIdAsync_returns_tracked_entity()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var saved = await repo.AddAsync(new Tiers { Nom = "Tracked", Type = TypeTiers.Client });
        var result = await repo.GetByIdAsync(saved.Id);

        Assert.IsNotNull(result);
        Assert.AreEqual(EntityState.Unchanged, db.Entry(result!).State);
    }

    // ══════════════════════════════════════════════════════════════════════
    // GetByIdWithNavigationsAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task<T?> GetByIdWithNavigationsAsync(
    //     int id,
    //     Expression<Func<T, object>>[] includes,
    //     CancellationToken cancellationToken = default)
    //
    // Loads one entity by id and eagerly loads navigation properties (Include).
    //
    // Covered:
    //   [x] id exists + include     → navigation populated (DevisClient.Client)
    //   [x] id missing              → returns null
    //   [x] multiple includes       → Client + Lignes loaded
    //   [x] empty includes array    → entity returned without eager load
    //   [x] non-navigation include  → throws InvalidOperationException
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task GetByIdWithNavigationsAsync_loads_included_navigation()
    {
        await using var db = TestDbContextFactory.Create();
        var tiersRepo = new Repository<Tiers>(db);
        var devisRepo = new Repository<DevisClient>(db);

        var client = await tiersRepo.AddAsync(new Tiers { Nom = "Client Nav", Type = TypeTiers.Client });
        var devis = await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-001",
            ClientId = client.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });

        var result = await devisRepo.GetByIdWithNavigationsAsync(devis.Id, [d => d.Client]);

        Assert.IsNotNull(result);
        Assert.AreEqual("Client Nav", result!.Client.Nom);
    }

    [TestMethod]
    public async Task GetByIdWithNavigationsAsync_returns_null_when_missing()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<DevisClient>(db);

        var result = await repo.GetByIdWithNavigationsAsync(999, [d => d.Client]);

        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByIdWithNavigationsAsync_loads_multiple_includes()
    {
        await using var db = TestDbContextFactory.Create();
        var tiersRepo = new Repository<Tiers>(db);
        var produitRepo = new Repository<Produit>(db);
        var devisRepo = new Repository<DevisClient>(db);
        var ligneRepo = new Repository<DevisClientLigne>(db);

        var client = await tiersRepo.AddAsync(new Tiers { Nom = "Multi Nav", Type = TypeTiers.Client });
        var produit = await produitRepo.AddAsync(new Produit { Reference = "P-1", Designation = "Huile" });
        var devis = await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-MULTI",
            ClientId = client.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });
        await ligneRepo.AddAsync(new DevisClientLigne
        {
            DevisClientId = devis.Id,
            ProduitId = produit.Id,
            Designation = "Ligne 1",
            Quantite = 1,
            PrixUnitaireHT = 100
        });

        var result = await devisRepo.GetByIdWithNavigationsAsync(
            devis.Id,
            [d => d.Client, d => d.Lignes]);

        Assert.IsNotNull(result);
        Assert.AreEqual("Multi Nav", result!.Client.Nom);
        Assert.AreEqual(1, result.Lignes.Count);
    }

    [TestMethod]
    public async Task GetByIdWithNavigationsAsync_works_with_empty_includes()
    {
        await using var db = TestDbContextFactory.Create();
        var tiersRepo = new Repository<Tiers>(db);
        var devisRepo = new Repository<DevisClient>(db);

        var client = await tiersRepo.AddAsync(new Tiers { Nom = "No Include", Type = TypeTiers.Client });
        var devis = await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-EMPTY",
            ClientId = client.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });

        var result = await devisRepo.GetByIdWithNavigationsAsync(devis.Id, []);

        Assert.IsNotNull(result);
        Assert.AreEqual("DV-EMPTY", result!.Numero);
    }

    [TestMethod]
    public async Task GetByIdWithNavigationsAsync_throws_for_non_navigation_include()
    {
        await using var db = TestDbContextFactory.Create();
        var tiersRepo = new Repository<Tiers>(db);
        var devisRepo = new Repository<DevisClient>(db);

        var client = await tiersRepo.AddAsync(new Tiers { Nom = "Bad Include", Type = TypeTiers.Client });
        var devis = await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-BAD",
            ClientId = client.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => devisRepo.GetByIdWithNavigationsAsync(devis.Id, [d => d.Numero]));
    }

    // ══════════════════════════════════════════════════════════════════════
    // GetAllAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    //
    // Returns every row (AsNoTracking). No filter, no paging.
    //
    // Covered:
    //   [x] rows exist        → returns full list
    //   [x] table empty       → returns empty list (not null)
    //   [x] many rows         → returns all (25 seeded)
    //   [x] results detached  → EntityState.Detached (AsNoTracking)
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task GetAllAsync_returns_all_entities()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "Client A", Type = TypeTiers.Client });
        await repo.AddAsync(new Tiers { Nom = "Client B", Type = TypeTiers.Client });

        var results = await repo.GetAllAsync();

        Assert.AreEqual(2, results.Count);
    }

    [TestMethod]
    public async Task GetAllAsync_returns_empty_list_when_none()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var results = await repo.GetAllAsync();

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public async Task GetAllAsync_returns_many_entities()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        for (var i = 1; i <= 25; i++)
        {
            await repo.AddAsync(new Tiers { Nom = $"Client {i}", Type = TypeTiers.Client });
        }

        var results = await repo.GetAllAsync();

        Assert.AreEqual(25, results.Count);
    }

    [TestMethod]
    public async Task GetAllAsync_returns_detached_entities()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "Detached", Type = TypeTiers.Client });
        var results = await repo.GetAllAsync();

        Assert.AreEqual(EntityState.Detached, db.Entry(results[0]).State);
    }

    // ══════════════════════════════════════════════════════════════════════
    // FindAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task<IReadOnlyList<T>> FindAsync(
    //     Expression<Func<T, bool>> predicate,
    //     CancellationToken cancellationToken = default)
    //
    // Filtered query (AsNoTracking). Returns all matches, no paging.
    //
    // Covered:
    //   [x] predicate matches some rows → returns subset
    //   [x] predicate matches nothing   → returns empty list
    //   [x] compound predicate (AND)    → both conditions applied
    //   [x] case-sensitive match        → "rabat" ≠ "Rabat"
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task FindAsync_returns_matching_entities()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "Client A", Type = TypeTiers.Client, Ville = "Rabat" });
        await repo.AddAsync(new Tiers { Nom = "Client B", Type = TypeTiers.Client, Ville = "Tétouan" });
        await repo.AddAsync(new Tiers { Nom = "Fournisseur", Type = TypeTiers.Fournisseur, Ville = "Rabat" });

        var results = await repo.FindAsync(t => t.Ville == "Rabat");

        Assert.AreEqual(2, results.Count);
    }

    [TestMethod]
    public async Task FindAsync_returns_empty_when_no_match()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "Client A", Type = TypeTiers.Client });

        var results = await repo.FindAsync(t => t.Ville == "Casablanca");

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public async Task FindAsync_supports_compound_predicate()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "A", Type = TypeTiers.Client, Ville = "Rabat" });
        await repo.AddAsync(new Tiers { Nom = "B", Type = TypeTiers.Fournisseur, Ville = "Rabat" });
        await repo.AddAsync(new Tiers { Nom = "C", Type = TypeTiers.Client, Ville = "Tétouan" });

        var results = await repo.FindAsync(t => t.Ville == "Rabat" && t.Type == TypeTiers.Client);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("A", results[0].Nom);
    }

    [TestMethod]
    public async Task FindAsync_is_case_sensitive_by_default()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "Client", Type = TypeTiers.Client, Ville = "Rabat" });

        var results = await repo.FindAsync(t => t.Ville == "rabat");

        Assert.AreEqual(0, results.Count);
    }

    // ══════════════════════════════════════════════════════════════════════
    // AnyAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task<bool> AnyAsync(
    //     Expression<Func<T, bool>> predicate,
    //     CancellationToken cancellationToken = default)
    //
    // Exists-check (AsNoTracking). Stops at first match — faster than FindAsync for guards.
    //
    // Covered:
    //   [x] match exists              → true
    //   [x] no match / empty table    → false
    //   [x] multiple matches          → true (existence only, not count)
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task AnyAsync_returns_true_when_match_exists()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "Client A", Type = TypeTiers.Client });

        var exists = await repo.AnyAsync(t => t.Nom == "Client A");

        Assert.IsTrue(exists);
    }

    [TestMethod]
    public async Task AnyAsync_returns_false_when_no_match()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var exists = await repo.AnyAsync(t => t.Nom == "Missing");

        Assert.IsFalse(exists);
    }

    [TestMethod]
    public async Task AnyAsync_returns_true_when_multiple_matches_exist()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "A", Type = TypeTiers.Client, Ville = "Rabat" });
        await repo.AddAsync(new Tiers { Nom = "B", Type = TypeTiers.Client, Ville = "Rabat" });
        await repo.AddAsync(new Tiers { Nom = "C", Type = TypeTiers.Client, Ville = "Rabat" });

        var exists = await repo.AnyAsync(t => t.Ville == "Rabat");

        Assert.IsTrue(exists);
    }

    // ══════════════════════════════════════════════════════════════════════
    // FindWithIncludesAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task<IReadOnlyList<T>> FindWithIncludesAsync(
    //     Expression<Func<T, bool>> predicate,
    //     Expression<Func<T, object>>[] includes,
    //     CancellationToken cancellationToken = default)
    //
    // Filter + eager-load navigations (AsNoTracking). For list pages needing related data.
    //
    // Covered:
    //   [x] predicate + include           → matching rows with navigation loaded
    //   [x] no matches                    → empty list
    //   [x] multiple includes             → Client + Lignes on matches
    //   [x] predicate excludes other rows → only filtered client's devis returned
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task FindWithIncludesAsync_returns_matches_with_navigation()
    {
        await using var db = TestDbContextFactory.Create();
        var tiersRepo = new Repository<Tiers>(db);
        var devisRepo = new Repository<DevisClient>(db);

        var client = await tiersRepo.AddAsync(new Tiers { Nom = "Client Find", Type = TypeTiers.Client });
        await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-001",
            ClientId = client.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });
        await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-002",
            ClientId = client.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });

        var results = await devisRepo.FindWithIncludesAsync(
            d => d.ClientId == client.Id,
            [d => d.Client]);

        Assert.AreEqual(2, results.Count);
        Assert.IsTrue(results.All(d => d.Client.Nom == "Client Find"));
    }

    [TestMethod]
    public async Task FindWithIncludesAsync_returns_empty_when_no_match()
    {
        await using var db = TestDbContextFactory.Create();
        var devisRepo = new Repository<DevisClient>(db);

        var results = await devisRepo.FindWithIncludesAsync(
            d => d.ClientId == 999,
            [d => d.Client]);

        Assert.AreEqual(0, results.Count);
    }

    [TestMethod]
    public async Task FindWithIncludesAsync_loads_multiple_includes()
    {
        await using var db = TestDbContextFactory.Create();
        var tiersRepo = new Repository<Tiers>(db);
        var produitRepo = new Repository<Produit>(db);
        var devisRepo = new Repository<DevisClient>(db);
        var ligneRepo = new Repository<DevisClientLigne>(db);

        var client = await tiersRepo.AddAsync(new Tiers { Nom = "Inc Multi", Type = TypeTiers.Client });
        var produit = await produitRepo.AddAsync(new Produit { Reference = "P-2", Designation = "Huile" });
        var devis = await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-INC",
            ClientId = client.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });
        await ligneRepo.AddAsync(new DevisClientLigne
        {
            DevisClientId = devis.Id,
            ProduitId = produit.Id,
            Designation = "Ligne",
            Quantite = 2,
            PrixUnitaireHT = 50
        });

        var results = await devisRepo.FindWithIncludesAsync(
            d => d.ClientId == client.Id,
            [d => d.Client, d => d.Lignes]);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("Inc Multi", results[0].Client.Nom);
        Assert.AreEqual(1, results[0].Lignes.Count);
    }

    [TestMethod]
    public async Task FindWithIncludesAsync_excludes_non_matching_rows()
    {
        await using var db = TestDbContextFactory.Create();
        var tiersRepo = new Repository<Tiers>(db);
        var devisRepo = new Repository<DevisClient>(db);

        var clientA = await tiersRepo.AddAsync(new Tiers { Nom = "A", Type = TypeTiers.Client });
        var clientB = await tiersRepo.AddAsync(new Tiers { Nom = "B", Type = TypeTiers.Client });
        await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-A",
            ClientId = clientA.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });
        await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-B",
            ClientId = clientB.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });

        var results = await devisRepo.FindWithIncludesAsync(
            d => d.ClientId == clientA.Id,
            [d => d.Client]);

        Assert.AreEqual(1, results.Count);
        Assert.AreEqual("DV-A", results[0].Numero);
    }

    // ══════════════════════════════════════════════════════════════════════
    // QueryPagedAsync<TResult>
    // ──────────────────────────────────────────────────────────────────────
    // Task<(IReadOnlyList<TResult> Items, int TotalCount)> QueryPagedAsync<TResult>(
    //     Expression<Func<T, bool>>? predicate,       // null = no filter
    //     Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
    //     Expression<Func<T, TResult>> selector,        // projection (DTO shape)
    //     int page,
    //     int pageSize,
    //     CancellationToken cancellationToken = default)
    //
    // List pages: filter → count all matches → sort → skip/take → project.
    // Normalizes page < 1 → 1 and pageSize < 1 → 15.
    //
    // Covered:
    //   [x] paging math (page 2, size 2) + total count unchanged by page
    //   [x] invalid page / pageSize normalized
    //   [x] predicate filters total count
    //   [x] last page partial (5 items, size 2 → page 3 has 1 item)
    //   [x] descending orderBy
    //   [x] page beyond last → empty items, total unchanged
    //   [x] selector projects to DTO type
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task QueryPagedAsync_returns_correct_page_and_total_count()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        for (var i = 1; i <= 5; i++)
        {
            db.Tiers.Add(new Tiers { Nom = $"Client {i}", Type = TypeTiers.Client });
        }

        await db.SaveChangesAsync();

        var (items, totalCount) = await repo.QueryPagedAsync(
            predicate: null,
            orderBy: q => q.OrderBy(t => t.Nom),
            selector: t => t.Nom,
            page: 2,
            pageSize: 2);

        Assert.AreEqual(5, totalCount);
        CollectionAssert.AreEqual(new[] { "Client 3", "Client 4" }, items.ToArray());
    }

    [TestMethod]
    public async Task QueryPagedAsync_normalizes_invalid_page_and_page_size()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        db.Tiers.Add(new Tiers { Nom = "Only", Type = TypeTiers.Client });
        await db.SaveChangesAsync();

        var (items, totalCount) = await repo.QueryPagedAsync(
            predicate: null,
            orderBy: q => q.OrderBy(t => t.Id),
            selector: t => t.Nom,
            page: 0,
            pageSize: 0);

        Assert.AreEqual(1, totalCount);
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual("Only", items[0]);
    }

    [TestMethod]
    public async Task QueryPagedAsync_predicate_filters_total_count()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        for (var i = 1; i <= 3; i++)
        {
            await repo.AddAsync(new Tiers { Nom = $"Client {i}", Type = TypeTiers.Client });
        }

        await repo.AddAsync(new Tiers { Nom = "Fournisseur", Type = TypeTiers.Fournisseur });

        var (items, totalCount) = await repo.QueryPagedAsync(
            predicate: t => t.Type == TypeTiers.Client,
            orderBy: q => q.OrderBy(t => t.Nom),
            selector: t => t.Nom,
            page: 1,
            pageSize: 10);

        Assert.AreEqual(3, totalCount);
        Assert.AreEqual(3, items.Count);
    }

    [TestMethod]
    public async Task QueryPagedAsync_last_page_can_be_partial()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        for (var i = 1; i <= 5; i++)
        {
            await repo.AddAsync(new Tiers { Nom = $"Client {i}", Type = TypeTiers.Client });
        }

        var (items, totalCount) = await repo.QueryPagedAsync(
            predicate: null,
            orderBy: q => q.OrderBy(t => t.Nom),
            selector: t => t.Nom,
            page: 3,
            pageSize: 2);

        Assert.AreEqual(5, totalCount);
        Assert.AreEqual(1, items.Count);
        Assert.AreEqual("Client 5", items[0]);
    }

    [TestMethod]
    public async Task QueryPagedAsync_supports_descending_order()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "A", Type = TypeTiers.Client });
        await repo.AddAsync(new Tiers { Nom = "C", Type = TypeTiers.Client });
        await repo.AddAsync(new Tiers { Nom = "B", Type = TypeTiers.Client });

        var (items, _) = await repo.QueryPagedAsync(
            predicate: null,
            orderBy: q => q.OrderByDescending(t => t.Nom),
            selector: t => t.Nom,
            page: 1,
            pageSize: 3);

        CollectionAssert.AreEqual(new[] { "C", "B", "A" }, items.ToArray());
    }

    [TestMethod]
    public async Task QueryPagedAsync_beyond_last_page_returns_empty_items()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "Only", Type = TypeTiers.Client });

        var (items, totalCount) = await repo.QueryPagedAsync(
            predicate: null,
            orderBy: q => q.OrderBy(t => t.Id),
            selector: t => t.Nom,
            page: 99,
            pageSize: 10);

        Assert.AreEqual(1, totalCount);
        Assert.AreEqual(0, items.Count);
    }

    [TestMethod]
    public async Task QueryPagedAsync_projects_to_dto_type()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "Dto Test", Type = TypeTiers.Client });

        var (items, totalCount) = await repo.QueryPagedAsync(
            predicate: null,
            orderBy: q => q.OrderBy(t => t.Id),
            selector: t => new TiersNameDto(t.Nom),
            page: 1,
            pageSize: 10);

        Assert.AreEqual(1, totalCount);
        Assert.AreEqual("Dto Test", items[0].Nom);
    }

    // ══════════════════════════════════════════════════════════════════════
    // AddAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    //
    // Insert: Set.Add + SaveChanges. Returns same entity with Id assigned.
    //
    // Covered:
    //   [x] entity persisted + Id > 0
    //   [x] audit timestamps set (CreatedAt, UpdatedAt via AppDbContext)
    //   [x] duplicate ICE allowed when no unique index (current schema)
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task AddAsync_persists_entity_and_assigns_id()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var entity = new Tiers { Nom = "Client A", Type = TypeTiers.Client };
        var saved = await repo.AddAsync(entity);

        Assert.IsTrue(saved.Id > 0);
        Assert.AreEqual(1, await db.Tiers.CountAsync());
    }

    [TestMethod]
    public async Task AddAsync_sets_audit_timestamps()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var before = DateTime.UtcNow.AddSeconds(-1);
        var saved = await repo.AddAsync(new Tiers { Nom = "Audited", Type = TypeTiers.Client });
        var after = DateTime.UtcNow.AddSeconds(1);

        Assert.IsTrue(saved.CreatedAt >= before && saved.CreatedAt <= after);
        Assert.IsTrue(saved.UpdatedAt >= before && saved.UpdatedAt <= after);
    }

    [TestMethod]
    public async Task AddAsync_allows_duplicate_ice_without_unique_constraint()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.AddAsync(new Tiers { Nom = "A", Type = TypeTiers.Client, ICE = "123456789123456" });
        await repo.AddAsync(new Tiers { Nom = "B", Type = TypeTiers.Client, ICE = "123456789123456" });

        Assert.AreEqual(2, await db.Tiers.CountAsync());
    }

    // ══════════════════════════════════════════════════════════════════════
    // UpdateAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    //
    // Update tracked or detached entity + SaveChanges.
    // If detached → Set.Update before save.
    //
    // Covered:
    //   [x] modified fields persisted on reload (tracked entity)
    //   [x] detached entity updated via Set.Update
    //   [x] partial update (unchanged fields preserved)
    //   [x] non-existent id throws DbUpdateConcurrencyException
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task UpdateAsync_persists_changes()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var entity = await repo.AddAsync(new Tiers { Nom = "Before", Type = TypeTiers.Client });
        entity.Nom = "After";
        await repo.UpdateAsync(entity);

        var reloaded = await repo.GetByIdAsync(entity.Id);
        Assert.AreEqual("After", reloaded!.Nom);
    }

    [TestMethod]
    public async Task UpdateAsync_persists_detached_entity()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var entity = await repo.AddAsync(new Tiers { Nom = "Detached Before", Type = TypeTiers.Client, Ville = "Rabat" });
        db.Entry(entity).State = EntityState.Detached;
        entity.Nom = "Detached After";

        await repo.UpdateAsync(entity);

        var reloaded = await repo.GetByIdAsync(entity.Id);
        Assert.AreEqual("Detached After", reloaded!.Nom);
    }

    [TestMethod]
    public async Task UpdateAsync_preserves_unchanged_fields()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var entity = await repo.AddAsync(new Tiers
        {
            Nom = "Name",
            Type = TypeTiers.Client,
            Ville = "Tétouan",
            ICE = "123456789123456"
        });
        entity.Nom = "Renamed";
        await repo.UpdateAsync(entity);

        var reloaded = await repo.GetByIdAsync(entity.Id);
        Assert.AreEqual("Renamed", reloaded!.Nom);
        Assert.AreEqual("Tétouan", reloaded.Ville);
        Assert.AreEqual("123456789123456", reloaded.ICE);
    }

    [TestMethod]
    public async Task UpdateAsync_throws_when_detached_entity_id_not_found()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var ghost = new Tiers { Id = 999, Nom = "Ghost", Type = TypeTiers.Client };

        await Assert.ThrowsExceptionAsync<DbUpdateConcurrencyException>(
            () => repo.UpdateAsync(ghost));
    }

    // ══════════════════════════════════════════════════════════════════════
    // DeleteAsync
    // ──────────────────────────────────────────────────────────────────────
    // Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    //
    // Find by id → Remove + SaveChanges. No-op (no throw) when id missing.
    //
    // Covered:
    //   [x] id exists              → row removed
    //   [x] id missing             → silent no-op, count unchanged
    //   [x] delete same id twice   → second call still no-op
    //   [x] FK dependents (Restrict) → throws (InMemory: InvalidOperationException)
    // ══════════════════════════════════════════════════════════════════════

    [TestMethod]
    public async Task DeleteAsync_removes_entity()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var entity = await repo.AddAsync(new Tiers { Nom = "To delete", Type = TypeTiers.Client });
        await repo.DeleteAsync(entity.Id);

        Assert.IsNull(await repo.GetByIdAsync(entity.Id));
    }

    [TestMethod]
    public async Task DeleteAsync_is_no_op_when_entity_missing()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        await repo.DeleteAsync(404);

        Assert.AreEqual(0, await db.Tiers.CountAsync());
    }

    [TestMethod]
    public async Task DeleteAsync_second_call_is_no_op()
    {
        await using var db = TestDbContextFactory.Create();
        var repo = new Repository<Tiers>(db);

        var entity = await repo.AddAsync(new Tiers { Nom = "Twice", Type = TypeTiers.Client });
        await repo.DeleteAsync(entity.Id);
        await repo.DeleteAsync(entity.Id);

        Assert.AreEqual(0, await db.Tiers.CountAsync());
    }

    [TestMethod]
    public async Task DeleteAsync_throws_when_fk_dependents_exist()
    {
        await using var db = TestDbContextFactory.Create();
        var tiersRepo = new Repository<Tiers>(db);
        var devisRepo = new Repository<DevisClient>(db);

        var client = await tiersRepo.AddAsync(new Tiers { Nom = "Linked", Type = TypeTiers.Client });
        await devisRepo.AddAsync(new DevisClient
        {
            Numero = "DV-FK",
            ClientId = client.Id,
            Date = DateTime.UtcNow,
            DateValidite = DateTime.UtcNow.AddDays(30)
        });

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => tiersRepo.DeleteAsync(client.Id));
    }
}
