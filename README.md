# OliveMorocco

ASP.NET Core 9 MVC application for olive estate and commercial management, styled with **Tailwind CSS** and the **ZAHO** design system.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (Tailwind build)

## Quick start

```bash
npm install
npm run css:build
dotnet run
```

Open https://localhost:5001 (or the URL shown in the terminal).

| Route | Page |
|-------|------|
| `/` or `/Home` | Site ZAHO (accueil) |
| `/Dashboard` | Gestion commerciale |

During UI work, run Tailwind in watch mode in a second terminal:

```bash
npm run css:watch
```

`dotnet build` also runs `npm run css:build` automatically when `package.json` is present.

## Project structure

| Path | Description |
|------|-------------|
| `Controllers/`, `Views/` | ASP.NET Core MVC |
| `Styles/app.css` | Tailwind entry + ZAHO design tokens |
| `Styles/Home/`, `Styles/Shared/`, `Styles/Dashboard/` | Page & layout CSS (mirrors `Views/`) |
| `wwwroot/css/site.css` | Generated CSS (do not edit by hand) |
| `wwwroot/images/` | Brand photography |
| `docs/DATABASE_SCHEMA.md` | Database schema |
| `docs/DESIGN-SYSTEM.md` | Full UI specification |

## Design system

Colors, typography, and components follow `docs/DESIGN-SYSTEM.md`. Cursor enforces tokens via `.cursor/rules/design-system-colors.mdc`.

**Fonts:** Fraunces (display) + Work Sans (UI) — loaded from Google Fonts in `_Layout.cshtml`.

## Documentation

- [Database schema](docs/DATABASE_SCHEMA.md)
- [Design system](docs/DESIGN-SYSTEM.md)
