# Git workflow — branches & pull requests

> OliveMorocco · `main` + `dev` + feature branches  
> Use this process for day-to-day development and to practice how teams ship code.

---

## Branch model

| Branch | Role |
|--------|------|
| **`main`** | Stable, production-ready code — what you deploy or demo as “release” |
| **`dev`** | Integration branch — daily work is merged here first |
| **`feature/*`** | Short-lived branches for one feature, module, or fix |
| **`fix/*`** | Optional prefix for small bugfixes (same flow as `feature/*`) |

```text
main  ─────────────────●────────────────●  (releases)
                        \              /
dev   ───●──●──●──●──●──●──●──●──●──●──●  (integration)
          \    /      \    /
feature   ●──●        ●──●
```

### Rules of thumb

- Do **not** commit directly to `main` (protect it on GitHub when the remote exists).
- Merge features into **`dev`** via pull request.
- Merge **`dev` → `main`** when a milestone is stable and tested.
- One feature branch = one coherent change (new page, EF module, schema update, etc.).

---

## Initial setup (once)

```bash
# From a clean main with at least one commit
git checkout main
git pull origin main

git checkout -b dev
git push -u origin dev
```

On GitHub (optional but recommended):

- **Settings → Branches → Add rule** on `main`:
  - Require a pull request before merging
  - Require status checks to pass (when CI exists)

---

## Daily workflow

### 1. Start from up-to-date `dev`

```bash
git checkout dev
git pull origin dev
```

### 2. Create a feature branch

```bash
git checkout -b feature/secteurs-module
# or: git checkout -b fix/dashboard-nav-link
```

Naming examples for this project:

| Branch | Work |
|--------|------|
| `feature/zaho-home-page` | Marketing site, `_HomeLayout` |
| `feature/dashboard-shell` | Dashboard layout + client-side modules |
| `feature/ef-core-secteurs` | Entities, migrations, secteurs CRUD |
| `fix/home-css-tokens` | Small design-system fix |

### 3. Work, commit, push

```bash
git add .
git commit -m "Add secteurs list view and controller"
git push -u origin feature/secteurs-module
```

Commit often with clear messages focused on **why**, not only what.

### 4. Open a pull request → `dev`

On GitHub:

- **Base:** `dev`
- **Compare:** `feature/secteurs-module`

PR description template:

```markdown
## Summary
- Brief list of what changed and why

## Test plan
- [ ] `dotnet build` passes
- [ ] `npm run css:build` passes (if UI touched)
- [ ] `dotnet run` — describe what you clicked/checked
```

Even when working solo, read the diff as if someone else wrote it before merging.

### 5. CI & review

Before merge, verify:

- Build succeeds (`dotnet build`, `npm run css:build` when CSS/views change)
- App runs (`dotnet run`) and the affected routes work

When CI is configured on the repo, the PR must show green checks.

### 6. Merge into `dev`

Preferred merge strategy on GitHub:

| Strategy | When to use |
|----------|-------------|
| **Squash and merge** | Default for feature → `dev` — one clean commit per PR |
| **Merge commit** | When you want to preserve every commit on the branch |
| **Rebase and merge** | Linear history without a merge commit |

After merge: **delete the feature branch** on GitHub.

### 7. Sync locally

```bash
git checkout dev
git pull origin dev
```

---

## Releasing: `dev` → `main`

When a milestone is ready (e.g. “UI shell complete”, “Secteurs module done”):

```bash
git push origin dev
```

Open a PR:

- **Base:** `main`
- **Compare:** `dev`

Use the same summary + test plan. Merge (usually squash or merge commit for releases).

Then update local branches:

```bash
git checkout main
git pull origin main

git checkout dev
git merge main
git push origin dev
```

This keeps `dev` aligned with `main` after each release.

---

## Quick reference

| Goal | Command / action |
|------|-------------------|
| Start new work | `git checkout dev && git pull && git checkout -b feature/name` |
| Ship feature to integration | PR: `feature/name` → `dev` |
| Ship release | PR: `dev` → `main` |
| Update local after remote merge | `git pull` on the branch you care about |
| See current branch | `git branch` |
| See status | `git status` |

---

## OliveMorocco-specific checks

Before merging UI or app changes:

```bash
npm install          # if package.json changed
npm run css:build    # if Styles/ or Views/ changed
dotnet build
dotnet run           # smoke-test routes below
```

| Route | Expected |
|-------|----------|
| `/` or `/Home` | ZAHO marketing site |
| `/Dashboard` | Gestion dashboard (Clients module by default) |

---

## FAQ

**Do I need a PR if I’m solo?**  
No — you can merge locally. PRs are still useful for history, self-review, and matching team practice.

**Do I need a feature branch for every tiny fix?**  
For learning and consistency, yes for anything non-trivial. Very small typos can go on `dev` directly in some teams; feature branches are safer.

**Can I work directly on `dev`?**  
Possible, but feature branches keep `dev` stable and make PRs easier to review.

**What if `main` and `dev` diverge?**  
Merge `main` into `dev` (or rebase `dev` onto `main`) before opening a release PR so conflicts are resolved on `dev` first.
