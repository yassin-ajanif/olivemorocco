# OliveMorocco — Unit testing

> **Scope:** DataAccess and Business layers only. No Web/controller tests in this guide.  
> **Framework:** MSTest · **Run:** `dotnet test` from repo root.

Reference implementation: `tests/OliveMorocco.DataAccess.Tests/Repositories/RepositoryTests.cs`.

---

## 1. Projects

```
tests/
├── OliveMorocco.DataAccess.Tests/   MSTest + EF Core InMemory
└── OliveMorocco.Business.Tests/     MSTest + Moq
```

| Layer | Test project | Dependencies | What to test |
|-------|--------------|--------------|--------------|
| **DataAccess** | `OliveMorocco.DataAccess.Tests` | `Microsoft.EntityFrameworkCore.InMemory` | `Repository<T>`, custom repos |
| **Business** | `OliveMorocco.Business.Tests` | `Moq` | Services, FluentValidation validators |
| **Web** | — | — | Out of scope for unit tests |

Do **not** test against PostgreSQL in unit tests. Use InMemory DB (DataAccess) or mocks (Business).

---

## 2. Folder layout (mirror source)

```
DataAccess.Tests/
├── Helpers/
│   └── TestDbContextFactory.cs
└── Repositories/
    └── RepositoryTests.cs

Business.Tests/
├── Helpers/
│   └── MapperFactory.cs
├── Services/
│   ├── TiersUsageServiceTests.cs
│   └── Vente/
│       └── ClientServiceTests.cs
└── Validation/
    └── Vente/
        └── CreateClientDtoValidatorTests.cs
```

- One `[TestClass]` per **class under test** (e.g. `RepositoryTests`, `ClientServiceTests`).
- Multiple `[TestMethod]` per behavior — **not** one file per method.
- Group tests in the **same order as the public API** (interface or class).

---

## 3. Method group header (required)

Before each group of tests for one method, add a comment block with:

1. Method name (section title)
2. Full signature (parameters + return type)
3. One-line behavior summary
4. **Covered** checklist — `[x]` for each scenario with a test

```csharp
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
```

Rules:

- **No** “Gaps to consider later” sections — either add a test or omit the scenario.
- Update **Covered** when adding tests.
- Keep groups in **interface order** for generic repos/services.

---

## 4. Test naming

Pattern: `{MethodName}_{scenario}`

```csharp
GetByIdAsync_returns_null_when_missing
CreateClientAsync_throws_when_nom_empty
QueryPagedAsync_beyond_last_page_returns_empty_items
```

Use `snake_case` after the method name for readability in Test Explorer.

---

## 5. DataAccess tests

### InMemory database

```csharp
await using var db = TestDbContextFactory.Create();
var repo = new Repository<Tiers>(db);
```

- Fresh DB per test (`Guid.NewGuid()` database name).
- `await using` disposes context after test — no real PostgreSQL writes.

### What to assert

- CRUD persistence and counts
- Paging math and normalization
- Include/navigation (`Expression<Func<T, object>>` — **navigation properties only**, not scalars/FKs)
- Edge ids: `0`, missing, negative (same as missing for identity PKs)
- FK restrict behavior (InMemory may throw `InvalidOperationException` vs PostgreSQL `DbUpdateException`)

### Helpers

| File | Purpose |
|------|---------|
| `Helpers/TestDbContextFactory.cs` | `AppDbContext` with `UseInMemoryDatabase` |

---

## 6. Business tests

### Mock repositories

```csharp
private Mock<IRepository<Tiers>> _repo = null!;

[TestInitialize]
public void Setup()
{
    _repo = new Mock<IRepository<Tiers>>();
    _sut = new ClientService(_repo.Object, /* … */, MapperFactory.Create(), validators);
}
```

### Real validators and AutoMapper

- Use **real** FluentValidation validators (not mocked).
- Use **real** AutoMapper via `MapperFactory.Create()` (AutoMapper 16 requires `NullLoggerFactory.Instance`).

### What to assert

- Business rules (e.g. `Type = Client` on create, fournisseur filtered out)
- Validation throws `ValidationException`
- Service orchestration (`Verify` mock calls, delete guard via `ITiersUsageService`)

### Validators

Test in isolation — no mocks:

```csharp
var validator = new CreateClientDtoValidator();
var result = validator.Validate(dto);
Assert.IsFalse(result.IsValid);
```

---

## 7. What not to unit test

| Skip | Reason |
|------|--------|
| Web / controllers | Out of scope |
| `CancellationToken` on Repository | EF handles it; InMemory is too fast |
| PostgreSQL-specific (`ILike`, migrations) | Integration tests later |
| Domain entities with no logic | No behavior to test |

---

## 8. Adding tests for a new module

Example: new `DevisService`.

1. Add `Business.Tests/Services/Vente/DevisServiceTests.cs`.
2. Mock `IRepository<DevisClient>` (and related repos).
3. Use real validators + `MapperFactory` if Vente profile maps apply.
4. One header block per public method on the service interface.
5. Run `dotnet test`.

Only add **DataAccess** tests when introducing a **custom repository** beyond `Repository<T>`. Generic `Repository<T>` is tested once in `RepositoryTests.cs`.

---

## 9. Commands

```powershell
dotnet test                                    # all tests
dotnet test tests/OliveMorocco.Business.Tests  # business only
dotnet test tests/OliveMorocco.DataAccess.Tests
```

---

## 10. Checklist (PR)

- [ ] New service has `*Tests.cs` with method-group headers and Covered lists
- [ ] Validators have validator tests (no mocks)
- [ ] DataAccess custom repo uses `TestDbContextFactory`
- [ ] Test names follow `{Method}_{scenario}`
- [ ] `dotnet test` passes
- [ ] No Web layer tests added unless explicitly requested
