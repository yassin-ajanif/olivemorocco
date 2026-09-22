# OliveMorocco

ASP.NET Core 9 MVC application for olive estate and commercial management, styled with **Tailwind CSS** and the **ZAHO** design system.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (Tailwind build)

## Quick start

```bash
cd src/OliveMorocco.Web
npm install
npm run css:build
dotnet run
```

Open https://localhost:7270 (or the URL shown in the terminal).

| Route | Page |
|-------|------|
| `/` or `/Home` | Site ZAHO (accueil) |
| `/Dashboard` | Gestion commerciale |

During UI work, run Tailwind in watch mode in a second terminal:

```bash
cd src/OliveMorocco.Web
npm run css:watch
```

`dotnet build` also runs `npm run css:build` automatically when `package.json` is present.

From the repo root you can also run:

```bash
dotnet run --project src/OliveMorocco.Web
```

## Project structure

| Path | Description |
|------|-------------|
| `src/OliveMorocco.Web/` | MVC host — Controllers, Views, Styles, wwwroot |
| `src/OliveMorocco.Domain/` | Entities, enums (no dependencies) |
| `src/OliveMorocco.DataAccess/` | EF Core, DbContext, configurations |
| `src/OliveMorocco.Business/` | Services, DTOs, validation (client CRUD wired) |
| `src/OliveMorocco.Web/Styles/app.css` | Tailwind entry + ZAHO design tokens |
| `src/OliveMorocco.Web/wwwroot/css/site.css` | Generated CSS (do not edit by hand) |
| `src/OliveMorocco.Web/wwwroot/images/` | Brand photography |
| `docs/ARCHITECTURE.md` | Clean Architecture (4 layers), folder layout, conventions |
| `docs/DATABASE_SCHEMA.md` | Database schema |
| `docs/DESIGN-SYSTEM.md` | Full UI specification |
| `docs/GIT-WORKFLOW.md` | Branching model (`main`, `dev`, feature branches) & PR process |

## Design system

Colors, typography, and components follow `docs/DESIGN-SYSTEM.md`. Cursor enforces tokens via `.cursor/rules/design-system-colors.mdc`.

**Fonts:** Fraunces (display) + Work Sans (UI) — loaded from Google Fonts in `_Layout.cshtml`.

## Documentation

- [Architecture](docs/ARCHITECTURE.md)
- [Database schema](docs/DATABASE_SCHEMA.md)
- [Design system](docs/DESIGN-SYSTEM.md)
- [Git workflow](docs/GIT-WORKFLOW.md)
