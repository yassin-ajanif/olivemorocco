# OliveMorocco — Project architecture

> **Reference implementation:** [FaturatiWeb](../FaturatiWeb) (`c:\Users\yassin\Desktop\FaturatiWeb`) — Clean Architecture, ASP.NET Core MVC, EF Core, PostgreSQL.  
> **Database:** [`DATABASE_SCHEMA.md`](DATABASE_SCHEMA.md)  
> **UI tokens:** [`DESIGN-SYSTEM.md`](DESIGN-SYSTEM.md)  
> **Git workflow:** [`GIT-WORKFLOW.md`](GIT-WORKFLOW.md)

---

## 1. Overview

**OliveMorocco** (brand **ZAHO**) is an ASP.NET Core web application for:

- **Exploitation oléicole** — secteurs, variétés, intrants, interventions, récoltes, pressages
- **Gestion commerciale** — ventes, achats, stock, charges (derived from sonlightining / GestionCommerciale, without *Bon de Préparation*)

The backend follows the same **4-layer Clean Architecture** as FaturatiWeb. The web host is MVC + Razor + Tailwind CSS 4 (ZAHO design system).

---

## 2. Layers

```
Web (MVC) ──► Business ──► DataAccess ──► Domain
                  │                          ▲
                  └──────────────────────────┘
```

| Layer | Project | Responsibility |
|-------|---------|----------------|
| **Domain** | `OliveMorocco.Domain` | Entities, enums, domain interfaces — **no dependencies** |
| **DataAccess** | `OliveMorocco.DataAccess` | `AppDbContext`, Fluent API configs, migrations, repositories — **only layer that knows EF/PostgreSQL** |
| **Business** | `OliveMorocco.Business` | Services, DTOs, AutoMapper profiles, FluentValidation — entities never leave this layer |
| **Web** | `OliveMorocco.Web` | MVC host: controllers, Razor views, static assets, auth, DI composition root |

### Dependency rules

| Project | May reference | Must NOT reference |
|---------|---------------|-------------------|
| Domain | *(nothing)* | — |
| DataAccess | Domain | Business, Web |
| Business | Domain, DataAccess | Web |
| Web | Business | DataAccess, EF Core directly |

**Single host registration:**

```csharp
// Program.cs (Web)
builder.Services.AddBusiness(connectionString);
```

`AddBusiness` internally calls `AddDataAccess` — the Web project never configures Npgsql or `DbContext` itself.

---

## 3. Target solution layout

```
OliveMorocco.sln
│
├── src/
│   ├── OliveMorocco.Domain/
│   │   ├── Common/
│   │   │   └── BaseEntity.cs              (Id, CreatedAt, UpdatedAt, CreatedByUserId)
│   │   ├── Entities/
│   │   │   ├── Common/                    (shared reference data — not a sidebar section)
│   │   │   │   ├── Tiers.cs
│   │   │   │   ├── AppSettings.cs
│   │   │   │   └── Identity/              (ApplicationUser — when auth is added)
│   │   │   ├── Operationnel/              (exploitation oléicole — sidebar Opérationnel)
│   │   │   │   ├── Secteur.cs
│   │   │   │   ├── SecteurVariete.cs
│   │   │   │   ├── Variete.cs
│   │   │   │   ├── Intrant.cs
│   │   │   │   ├── Intervention.cs
│   │   │   │   ├── Recolte.cs
│   │   │   │   └── Pressage.cs
│   │   │   ├── Vente/                     (sidebar Vente)
│   │   │   │   ├── Produit.cs
│   │   │   │   ├── MouvementStock.cs
│   │   │   │   ├── DevisClient.cs + DevisClientLigne.cs
│   │   │   │   ├── BonCommandeClient.cs + …
│   │   │   │   ├── BonLivraisonClient.cs + …
│   │   │   │   ├── FactureClient.cs + …
│   │   │   │   ├── PaiementClient.cs
│   │   │   │   └── AvoirClient.cs + …
│   │   │   └── Achat/                     (sidebar Achat)
│   │   │       ├── Service.cs
│   │   │       ├── TypeCharge.cs / Charge.cs
│   │   │       ├── BonCommandeFournisseur.cs + …
│   │   │       ├── BonReception.cs + …
│   │   │       ├── FactureFournisseur.cs + …
│   │   │       ├── PaiementFournisseur.cs
│   │   │       └── AvoirFournisseur.cs + …
│   │   └── Enums/
│   │       ├── TypeTiers.cs
│   │       ├── TypeMouvement.cs
│   │       └── ModePaiement.cs
│   │
│   ├── OliveMorocco.DataAccess/
│   │   ├── AppDbContext.cs
│   │   ├── Configurations/
│   │   │   ├── Common/
│   │   │   ├── Operationnel/
│   │   │   ├── Vente/
│   │   │   └── Achat/
│   │   ├── Repositories/
│   │   │   ├── IRepository.cs
│   │   │   ├── Repository.cs
│   │   │   └── (custom repos only when needed)
│   │   ├── Migrations/
│   │   ├── DatabaseInitializer.cs
│   │   └── DependencyInjection.cs         (UseNpgsql)
│   │
│   ├── OliveMorocco.Business/
│   │   ├── DTOs/
│   │   │   ├── Common/
│   │   │   ├── Operationnel/
│   │   │   ├── Vente/
│   │   │   └── Achat/
│   │   ├── Mapping/                       (AutoMapper profiles per domain)
│   │   ├── Validation/                    (FluentValidation — Operationnel / Vente / Achat)
│   │   ├── Services/
│   │   │   ├── GenericService.cs / IGenericService.cs
│   │   │   ├── Operationnel/              (SecteurService, RecolteService, …)
│   │   │   ├── Vente/
│   │   │   └── Achat/
│   │   └── DependencyInjection.cs
│   │
│   └── OliveMorocco.Web/                  (MVC host — current monolith migrates here)
│       ├── Controllers/
│       ├── Models/                        (ViewModels — UI binding only)
│       ├── Views/
│       ├── Styles/                        (Tailwind source, mirrors Views/)
│       ├── wwwroot/
│       ├── Routing/
│       │   └── AppSections.cs             (URL section prefixes)
│       ├── Program.cs
│       └── appsettings.json
│
├── docs/                                  (project documentation — this folder)
└── tests/                                 (deferred)
    ├── OliveMorocco.Business.Tests/
    └── OliveMorocco.DataAccess.Tests/
```

---

## 4. Current state vs target

| Today (monolith) | Target |
|------------------|--------|
| Single project `olivemorocco.csproj` at repo root | `src/OliveMorocco.Web` + 3 class libraries |
| Controllers/, Views/, Styles/ at root | Same folders under `OliveMorocco.Web` |
| Client-side demo data (`wwwroot/js/store.js`) | EF Core + Business services + PostgreSQL |
| No EF Core | PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL` |

Migration is incremental: extract Domain → DataAccess → Business, then move the MVC host into `OliveMorocco.Web` without breaking routes.

---

## 5. Domain modules — three business categories

Code, DTOs, services, EF configurations, and controllers are grouped into **three domains**, matching the dashboard sidebar ([`DATABASE_SCHEMA.md`](DATABASE_SCHEMA.md)):

| Domain | Folder (all layers) | Sidebar | Main entities |
|--------|---------------------|---------|---------------|
| **Opérationnel** | `Operationnel/` | Opérationnel | `Secteur`, `SecteurVariete`, `Variete`, `Intrant`, `Intervention`, `Recolte`, `Pressage` |
| **Vente** | `Vente/` | Vente | `Produit`, `MouvementStock`, devis / BC / BL / factures / avoirs **client** + lignes, paiements client |
| **Achat** | `Achat/` | Achat | `Service`, `TypeCharge`, `Charge`, BC / BR / factures / avoirs **fournisseur** + lignes, paiements fournisseur |

**Shared reference data** (used by several domains) lives in `Common/` — not a fourth sidebar section:

| Folder | Entities |
|--------|----------|
| `Common/` | `Tiers`, `AppSettings`, `Identity/` |

**Excluded** (by design): Bon de Préparation and related tables.

**Cross-domain flows** (services may call across folders; entities stay in their domain):

- **Pressage:** `Recolte` → `Pressage` (Opérationnel) → `FactureFournisseur` (Achat, service line via `Services`)
- **Intervention costs:** `Intervention` (Opérationnel) → `Charge` (Achat) via `Charges.InterventionId`
- **Stock:** `MouvementStock` (Vente) triggered from BL / BR / avoirs

---

## 6. Web layer (MVC)

### URL sections (not MVC Areas)

Same pattern as FaturatiWeb: controllers use **route prefixes** matching the dashboard sidebar.

| Section | URL prefix | Examples |
|---------|------------|----------|
| Public site | `/`, `/Home` | ZAHO marketing page |
| Dashboard | `/Dashboard` | Gestion shell (today) |
| Vente | `/Vente/...` | `/Vente/Clients`, `/Vente/Devis` |
| Achat | `/Achat/...` | `/Achat/Fournisseurs`, `/Achat/BonsReception` |
| Opérationnel | `/Operationnel/...` | `/Operationnel/Secteurs`, `/Operationnel/Recoltes` |

Defined in `Web/Routing/AppSections.cs` (to be created).

### Controllers

Organized under the **three domains** (mirrors Business services):

```
Controllers/
├── Operationnel/      (SecteursController, RecoltesController, …)
├── Vente/             (ClientsController, DevisController, …)
└── Achat/             (FournisseursController, ChargesController, …)
```

- **`sealed` primary-constructor** controllers
- Inject **`I*Service`** from Business only — never `DbContext`
- Map **ViewModel ↔ DTO** in the controller
- Optional **HTMX partials** for list/search (`_*Results.cshtml`) — same as FaturatiWeb

### Views

```
Views/
├── Home/                    ← public marketing site (_HomeLayout)
├── Dashboard/
├── Shared/
├── Operationnel/            ← Secteurs/, Recoltes/, …
├── Vente/                   ← Clients/, Devis/, …
└── Achat/                   ← Fournisseurs/, Charges/, …
```

### ViewModels vs DTOs

| Type | Location | Purpose |
|------|----------|---------|
| **DTO** | `Business/DTOs/` | Service contract — flat read models + Create/Update commands |
| **ViewModel** | `Web/Models/` | Razor binding, pagination, form state, HTMX partials |

Entities never reach controllers or views.

### Frontend

| Concern | Location |
|---------|----------|
| Tailwind entry | `Web/Styles/app.css` |
| Page CSS (mirrors Views) | `Web/Styles/Home/`, `Shared/`, `Dashboard/`, … |
| Compiled CSS | `wwwroot/css/site.css` (generated — do not edit) |
| Public site JS | `wwwroot/js/home.js` |
| Dashboard JS | `wwwroot/js/dashboard-app.js`, `store.js`, … (temporary until server-backed CRUD) |

Build: `npm run css:build` / MSBuild target on `dotnet build`.

Design tokens: [`DESIGN-SYSTEM.md`](DESIGN-SYSTEM.md) — pine, gold, cream; Fraunces + Work Sans.

---

## 7. Data access

| Item | Location |
|------|----------|
| DbContext | `DataAccess/AppDbContext.cs` |
| Fluent API | `DataAccess/Configurations/{Common,Operationnel,Vente,Achat}/` |
| Migrations | `DataAccess/Migrations/` |
| Seed / migrate on startup | `DatabaseInitializer` + optional `IdentitySeeder` |

**Conventions:**

- `DECIMAL(18,2)` for money
- Enum → string conversion in Fluent API where needed
- `BaseEntity` audit fields stamped in `SaveChanges`
- PostgreSQL via connection string `ConnectionStrings:Default`

**Startup:**

```csharp
await scope.ServiceProvider
    .GetRequiredService<IAppDatabaseInitializer>()
    .InitializeAsync();  // MigrateAsync + seed
```

---

## 8. Business layer patterns

### Generic repository

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
```

Registered once: `services.AddScoped(typeof(IRepository<>), typeof(Repository<>));`

Custom repositories only when generic CRUD is insufficient (complex includes, reports).

### Generic service (simple CRUD entities)

For lookup tables (`Variete`, `TypeCharge`, `Intrant`, …):

```csharp
services.AddScoped<
    IGenericService<Variete, VarieteDto, CreateVarieteDto, UpdateVarieteDto>,
    GenericService<Variete, VarieteDto, CreateVarieteDto, UpdateVarieteDto>>();
```

Uses **AutoMapper** + **FluentValidation** inside `GenericService`.

### Concrete services (documents & workflows)

Subclass or standalone services for:

- Document numbering, line totals, stock movements
- `Recolte` → `Pressage` → supplier invoice
- `Charges` linked to `Interventions`

### Validation split

| Layer | Responsibility |
|-------|----------------|
| **FluentValidation** | Shape rules (required, length, format) on Create/Update DTOs |
| **Service methods** | Business rules needing DB (uniqueness, stock, billing) |

Messages in **French**.

### DTO naming

| Pattern | Example |
|---------|---------|
| Read | `SecteurDto`, `DevisClientDto` |
| Create | `CreateSecteurDto`, `CreateDevisClientDto` |
| Update | `UpdateSecteurDto`, `UpdateDevisClientDto` |

Prefer **C# records** for immutability.

---

## 9. Naming conventions

| Layer | Convention | Example |
|-------|------------|---------|
| Projects | `OliveMorocco.{Layer}` | `OliveMorocco.Business` |
| Entities | PascalCase French terms | `BonLivraisonClient`, `SecteurVariete` |
| Services | `I{Feature}Service` / `{Feature}Service` | `IRecolteService` |
| Validators | `{DtoName}Validator` | `CreateRecolteDtoValidator` |
| Controllers | `{Feature}Controller` in domain folder | `Controllers/Operationnel/SecteursController.cs` |
| ViewModels | `{Feature}{Purpose}ViewModel` | `SecteurListViewModel` |
| Views | Match controller | `Views/Secteurs/Index.cshtml` |
| Partials | `_` prefix | `_SecteursListResults.cshtml` |
| DB tables | French plural | `Secteurs`, `Recoltes`, `DevisClients` |
| Configurations | `{Entity}Configuration.cs` | `SecteurConfiguration.cs` |

**Code style:** `sealed` classes where possible, primary constructors (C# 12), `CancellationToken` on async APIs.

---

## 10. Dependency injection summary

```csharp
// DataAccess/DependencyInjection.cs
services.AddDbContext<AppDbContext>(...);
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Business/DependencyInjection.cs
services.AddAutoMapper(typeof(OperationnelProfile).Assembly);
services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
// Generic + concrete service registrations

// Web/Program.cs
builder.Services.AddBusiness(connectionString);
builder.Services.AddScoped<ICurrentUserAccessor, HttpCurrentUserAccessor>(); // when auth exists
```

**Packages (Business):** AutoMapper, FluentValidation, FluentValidation.DependencyInjectionExtensions.

---

## 11. Implementation roadmap

Suggested order (each step = feature branch → PR → `dev`):

| Phase | Work |
|-------|------|
| **0** | Solution split: Domain, DataAccess, Business, Web projects under `src/` |
| **1** | `BaseEntity`, core enums, `AppDbContext`, first migration |
| **2** | Generic repository + generic service + one CRUD module (e.g. `Varietes` or `Secteurs`) |
| **3** | Dashboard modules wired to real services (replace `store.js` demo data) |
| **4** | Opérationnel: secteurs, récoltes, pressages |
| **5** | Vente: clients, devis, BC, BL, factures |
| **6** | Achat: fournisseurs, BR, factures, charges |
| **7** | Auth (ASP.NET Core Identity) + audit `CreatedByUserId` |

---

## 12. Related documents

| Document | Content |
|----------|---------|
| [`DATABASE_SCHEMA.md`](DATABASE_SCHEMA.md) | Tables, relationships, business rules |
| [`DESIGN-SYSTEM.md`](DESIGN-SYSTEM.md) | ZAHO colors, typography, components |
| [`GIT-WORKFLOW.md`](GIT-WORKFLOW.md) | `main` / `dev` / feature branches, PR process |
| FaturatiWeb `architecture.md` | Full reference for patterns not duplicated here |

---

## 13. Conventions summary (quick reference)

| Concern | Default | Custom when… |
|---------|---------|--------------|
| Data access | `IRepository<T>` | Complex queries, reports |
| Business CRUD | `IGenericService<…>` | Documents, stock, opérationnel workflows |
| Mapping | AutoMapper profiles | Odd fields → `ForMember` / `Ignore` |
| Input validation | FluentValidation on DTOs | DB-dependent rules → service |
| UI contract | ViewModels (Web) + DTOs (Business) | Never expose entities |
| DB | PostgreSQL in DataAccess only | — |
