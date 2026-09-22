# ZAHO — Design System

> Huile d'olive extra vierge · Had Touabet, Essaouira, Maroc  
> Derived from the ZAHO marketing site (single-page HTML). Use this document as the source of truth for color, type, spacing, motion, and UI patterns.

---

## 1. Brand identity

| Attribute | Value |
|-----------|-------|
| **Brand name** | ZAHO |
| **Meaning** | « Fleur sauvage » — signification tamazight |
| **Product** | Huile d'olive extra vierge, production limitée |
| **Terroir** | AOP Tyout Chiadma · Had Touabet, Chiadma, Essaouira |
| **Tone** | Premium, terroir-driven, restrained luxury — not volume-oriented |
| **Audience (primary)** | Restaurateurs, importateurs, distributeurs B2B |
| **Audience (secondary)** | Épiceries, détaillants, particuliers curieux |

### Voice & copy principles

- Lead with **origin and traceability**, not generic superlatives.
- Prefer concrete facts (36 hectares, lot numbers, récolte year) over vague claims.
- French copy; place names and AOP references stay authentic.
- CTAs are invitation-based: *Demander un échantillon*, *Découvrir l'histoire*, not aggressive sales language.

---

## 2. Design tokens

### 2.1 Color palette

#### Core CSS variables

| Token | Hex | Role |
|-------|-----|------|
| `--pine` | `#14261C` | Primary dark — nav (scrolled), dark sections, button text on gold |
| `--pine-mid` | `#2C4229` | Mid-tone green (reserved / secondary dark) |
| `--gold` | `#C7A24A` | Primary accent — CTAs, borders, eyebrows on light bg |
| `--gold-soft` | `#E4C878` | Hover states, highlights, eyebrows on dark bg |
| `--cream` | `#F4EFE1` | Page background, light text on dark surfaces |
| `--ink` | `#1E1B12` | Body text on light backgrounds |
| `--line` | `#DCD3B8` | Borders, dividers, grid gutters |

#### Extended palette (contextual)

| Name | Hex | Usage |
|------|-----|-------|
| Pine gradient stop | `#1C3226` | Hero radial gradient, CTA section gradient |
| Footer bg | `#0F1C14` | Footer only — deeper than `--pine` |
| Photo slot bg | `#EAE3CE` | Image placeholders |
| Decorative olive stroke | `#3E5A3A` | Hero branch SVG |
| Hero body text | `#D9D6C6` | Secondary copy on dark hero |
| Scroll cue | `#B9C4AE` | Tertiary / muted on dark |
| Muted label (light) | `#6B6650` | Stats, pills, spec labels |
| Muted label (dark) | `#C7CDBE` | Process step descriptions |
| Card body (light) | `#5C5844` | Differentiator card paragraphs |
| Placeholder text | `#8C8365` | Photo slot instructions |
| Footer links | `#8FA085` | Footer body and links |
| Form option text | `#111111` | `<select>` dropdown options (light context) |

#### Semantic mapping

```
Background (default)     → --cream
Background (dark)        → --pine / gradient to #1C3226
Background (footer)      → #0F1C14
Text (primary, light bg) → --ink
Text (primary, dark bg)  → --cream
Text (accent)            → --gold / --gold-soft
Border / divider         → --line
Interactive primary      → --gold → hover --gold-soft
Interactive secondary    → ghost border rgba(244,239,225,.35)
Focus ring               → --gold-soft
```

#### Gradients

| Name | Definition |
|------|------------|
| **Hero** | `radial-gradient(ellipse at 30% 20%, #1C3226 0%, var(--pine) 60%)` |
| **CTA section** | `linear-gradient(135deg, var(--pine) 0%, #1C3226 100%)` |

#### Opacity & overlays

| Context | Value |
|---------|-------|
| Nav (default) | `rgba(20, 38, 28, 0)` — transparent |
| Nav (scrolled) | `rgba(20, 38, 28, 0.92)` + `backdrop-filter: blur(6px)` |
| Hero branch SVG | `opacity: 0.5` |
| Nav links | `opacity: 0.8` → `1` on hover |
| Process steps (inactive) | `opacity: 0.35` → `1` when `.in` |
| Ghost button border | `rgba(244, 239, 225, 0.35)` |
| Form input border | `rgba(244, 239, 225, 0.3)` |
| Footer divider | `rgba(255, 255, 255, 0.08)` |

---

### 2.2 Typography

#### Font families

| Role | Family | Weights / styles loaded |
|------|--------|---------------------------|
| **Display / serif** | [Fraunces](https://fonts.google.com/specimen/Fraunces) | 300, 400 italic, 600, 600 italic |
| **UI / sans** | [Work Sans](https://fonts.google.com/specimen/Work+Sans) | 400, 500, 600 |

```html
<link href="https://fonts.googleapis.com/css2?family=Fraunces:ital,opsz,wght@0,9..144,300;0,9..144,600;1,9..144,400;1,9..144,600&family=Work+Sans:wght@400;500;600&display=swap" rel="stylesheet">
```

#### Type scale

| Element | Font | Size | Weight | Style | Line-height | Letter-spacing | Transform |
|---------|------|------|--------|-------|-------------|----------------|-----------|
| **Hero H1** | Fraunces | `clamp(64px, 11vw, 148px)` | 600 | normal | 0.92 | 1px | — |
| **Hero subtitle** | Fraunces | `clamp(18px, 2.4vw, 26px)` | 400 | italic | — | — | — |
| **Section H2** | Fraunces | `clamp(32px, 4.5vw, 54px)` | 600 | normal | 1.08 | — | — |
| **Produit H2** | Fraunces | `clamp(28px, 3.5vw, 42px)` | 600 | normal | — | — | — |
| **CTA H2** | Fraunces | `clamp(28px, 4.5vw, 46px)` | 400 | italic | 1.3 | — | — |
| **Card H3** | Fraunces | 19–20px | 600 | normal | — | — | — |
| **Stat number** | Fraunces | 38px | — | normal | — | — | — |
| **Eyebrow** | Work Sans | 12px | 600 | normal | — | 3.5px | uppercase |
| **Body (default)** | Work Sans | 16–16.5px | 400 | normal | 1.7–1.85 | — | — |
| **Body (small)** | Work Sans | 14–14.5px | 400 | normal | 1.6–1.7 | — | — |
| **Nav links** | Work Sans | 13px | 400 | normal | — | 1px | — |
| **Nav CTA / buttons** | Work Sans | 12–14px | 600 (gold btn) | normal | — | 0.5–1.5px | — |
| **Pills / stats label** | Work Sans | 12px | 400 | normal | — | 0.5–1px | uppercase (stats) |
| **Form labels** | Work Sans | 12px | 400 | normal | — | 1px | uppercase |
| **Form inputs** | Work Sans | 15px | 400 | normal | — | — | — |
| **Footer H4** | Work Sans | 12px | 600 | normal | — | 1.5px | uppercase |
| **Footer body** | Work Sans | 13.5px | 400 | normal | 2 (line-height) | — | — |
| **Process step num** | Fraunces | 15px | — | italic | — | — | — |
| **Scroll cue** | Work Sans | 11px | 400 | normal | — | 2px | uppercase |

#### Brand wordmark (nav / footer)

| Property | Nav | Footer |
|----------|-----|--------|
| Font | Fraunces | Fraunces |
| Size | 22px | 20px |
| Letter-spacing | 4px | 3px |
| Color | `--cream` | `--cream` |

---

### 2.3 Spacing & layout

#### Page rhythm

| Token | Value | Notes |
|-------|-------|-------|
| **Horizontal gutter** | `6vw` | Nav, sections, hero, footer |
| **Section padding (desktop)** | `120px 6vw` | Default vertical rhythm |
| **Section padding (mobile ≤860px)** | `80px 6vw` | — |
| **CTA section padding** | `140px 6vw` | Contact block |
| **Section head margin-bottom** | `64px` | Below eyebrow + H2 group |
| **Max content width (hero)** | `760px` | Hero inner |
| **Max content width (section head)** | `640px` | — |
| **Max content width (hero desc)** | `480px` | — |
| **Max content width (origine text)** | `480px` per paragraph | — |
| **Max form width** | `520px` | Centered in CTA |

#### Grid gaps

| Layout | Columns | Gap |
|--------|---------|-----|
| Origine | `1fr 1fr` | 70px (40px mobile) |
| Produit | `0.85fr 1fr` | 70px (40px mobile) |
| Process steps | `repeat(5, 1fr)` | 22px (30px stacked mobile) |
| Differentiators | `repeat(3, 1fr)` | 1px (grid line technique) |
| Stat row | flex | 50px (34px mobile) |
| Nav links | flex | 34px |
| Hero CTAs | flex | 18px |
| CTA row | flex | 18px |
| Footer cols | flex | 60px |

#### Component internal spacing

| Component | Padding / margin |
|-----------|------------------|
| Nav (default) | `22px 6vw` |
| Nav (scrolled) | `14px 6vw` |
| Hero inner top | `padding-top: 90px` (clears fixed nav) |
| Hero eyebrow → H1 | `margin-bottom: 26px` |
| Hero sub | `margin-top: 18px` |
| Hero desc | `margin-top: 26px` |
| Hero CTAs | `margin-top: 42px` |
| Scroll cue | `bottom: 34px` |
| Photo slot | `padding: 30px` |
| Origine paragraph | `margin-top: 20px` |
| Stat row | `margin-top: 44px` |
| Diff card | `padding: 44px 34px` |
| Diff mark → H3 | `margin-bottom: 22px` (mark), `12px` (H3 → p) |
| Pill | `padding: 7px 16px` |
| Spec list item | `padding: 14px 0` |
| Spec list | `margin-top: 28px` |
| Tag row | `margin: 22px 0 26px` |
| Form | `gap: 14px`, `margin-top: 56px` |
| Footer | `padding: 60px 6vw 34px` |
| Foot top | `padding-bottom: 40px` |
| Foot bottom | `padding-top: 24px` |

---

### 2.4 Border radius & borders

| Element | Radius | Border |
|---------|--------|--------|
| Buttons / nav CTA | `2px` | 1px solid where applicable |
| Photo slot | `4px` | `1.5px dashed var(--line)` |
| Pills | `20px` | `1px solid var(--line)` |
| Diff mark (circle) | `50%` | `1.5px solid var(--gold)` |
| Diff grid | — | `1px solid var(--line)` outer |
| Form inputs | — | bottom only: `1px solid` |
| Spec list items | — | bottom: `1px solid var(--line)` |

---

### 2.5 Shadows

| Element | Shadow |
|---------|--------|
| Nav (scrolled) | `0 8px 24px rgba(0, 0, 0, 0.15)` |
| `.btn-gold` | `0 10px 30px -12px rgba(199, 162, 74, 0.6)` |

---

### 2.6 Motion & animation

#### Global

| Rule | Value |
|------|-------|
| `html` scroll | `scroll-behavior: smooth` |
| Reduced motion | Disable smooth scroll; force `0.01ms` animation/transition duration |

#### Named animations

| Name | Duration | Easing | Delay pattern | Effect |
|------|----------|--------|---------------|--------|
| `riseIn` | 0.9–1s | ease | Staggered 0.2s → 1.3s | `opacity 0→1`, `translateY(18px→0)` |
| `cueMove` | 2.2s | ease-in-out, infinite | — | Gold line sweep in scroll cue |
| Nav transition | 0.4s | ease | — | background, padding, box-shadow |
| Reveal (`.reveal.in`) | 0.8s | ease | — | `opacity`, `translateY(28px→0)` |
| Process path draw | 1.6s | `cubic-bezier(.22,.7,.2,1)` | — | `stroke-dashoffset 1400→0` |
| Process step fade | 0.5s | ease | — | `opacity .35→1` |
| Button hover | 0.25s | ease | — | transform, background, border |
| Link hover | 0.25s | — | — | opacity, color |

#### Intersection Observer thresholds

| Target | Threshold | Class added |
|--------|-----------|-------------|
| `.reveal` | 0.15 | `.in` |
| `.p-step` | 0.15 | `.in` |
| `#processPath` | 0.3 | `.in` |

#### Scroll-triggered nav

- Add `.scrolled` to `<nav>` when `window.scrollY > 60`.

---

### 2.7 Focus & accessibility

```css
:focus-visible {
  outline: 2px solid var(--gold-soft);
  outline-offset: 3px;
}
```

- Images: `display: block; max-width: 100%`
- Form fields: visible focus via gold bottom border
- Respect `prefers-reduced-motion: reduce`

---

## 3. Components

### 3.1 Navigation

**Structure:** fixed top bar · wordmark left · anchor links center-right · CTA button far right.

| State | Background | Padding | Extra |
|-------|------------|---------|-------|
| Default | transparent | `22px 6vw` | — |
| Scrolled (`.scrolled`) | `rgba(20,38,28,0.92)` + blur | `14px 6vw` | box-shadow |

**Links:** Origine · Processus · Produit · Contact  
**CTA:** `Demander un échantillon` → `#contact`

**Mobile (≤860px):** `.nav-links` hidden; hamburger (`.nav-toggle`) defined but not implemented in reference HTML.

---

### 3.2 Buttons

#### Primary — `.btn-gold`

```
background: var(--gold)
color: var(--pine)
font-weight: 600
font-size: 14px
padding: 16px 32px
border-radius: 2px
letter-spacing: 0.5px
box-shadow: gold glow (see 2.5)

:hover → translateY(-2px), background var(--gold-soft)
```

Use for: primary CTAs, form submit.

#### Secondary — `.btn-ghost`

```
border: 1px solid rgba(244,239,225,.35)
color: var(--cream)
padding: 16px 30px
font-size: 14px
border-radius: 2px

:hover → border-color var(--gold-soft), color var(--gold-soft)
```

Use for: secondary hero CTA, contact section alternate action.

#### Nav CTA — `.nav-cta`

```
border: 1px solid var(--gold)
color: var(--gold-soft)
padding: 9px 20px
font-size: 12px
letter-spacing: 1.5px
border-radius: 2px

:hover → background var(--gold), color var(--pine)
```

---

### 3.3 Hero

- Full viewport min-height (`100vh`)
- Radial pine gradient background
- Decorative branch SVG (`hero-branch`): right-aligned, 56% width, 50% opacity
- Content stack: eyebrow → H1 → italic subtitle → description → dual CTAs
- Scroll cue bottom-left with animated line

---

### 3.4 Section header — `.section-head`

```
.eyebrow (gold / gold-soft on dark)
h2 (Fraunces, clamp scale)
max-width: 640px
margin-bottom: 64px
```

Pair with `.reveal` for scroll entrance.

---

### 3.5 Photo slot — `.photo-slot`

Placeholder for real photography until assets are integrated.

```
aspect-ratio: 4/5 (produit variant: 3/4)
border: 1.5px dashed var(--line)
border-radius: 4px
background: #EAE3CE
centered muted instruction text (12.5px, #8C8365)
```

**Photo briefs (from copy):**
- Origine: verger d'oliviers SHD, lumière du matin, Had Touabet
- Produit: bouteille ZAHO, fond neutre, lumière naturelle

---

### 3.6 Stats — `.stat-row` / `.stat`

```
Number: Fraunces 38px, color var(--pine)
Label: 12px uppercase, #6B6650, letter-spacing 1px
Layout: horizontal flex, gap 50px
```

Example stats: **36** Hectares · **5e** Année de production · **2026** Récolte actuelle

---

### 3.7 Process timeline — `.process-wrap`

**Signature element:** animated SVG path connecting five steps.

| Part | Behavior |
|------|----------|
| `.process-svg-track` | Full-width curved path, gold stroke, draw-on-scroll |
| `.process-path` | `stroke-dasharray/offset: 1400`; `.in` resets offset |
| `.process-steps` | 5-column grid; hidden SVG on mobile |
| `.p-step` | Roman numeral, H3, description; fades in with `.in` |

**Steps:** Récolte → Transport → Pression → Mise en bouteille → Expédition

Section uses `.section-dark` (pine background, cream text).

---

### 3.8 Product block — `.produit-grid`

- Two-column: photo slot + text
- Eyebrow: *La bouteille*
- Tag row: `.pill` components
- Spec list: label / value pairs with bottom borders

**Default pills:** Extra vierge · Première pression à froid · AOP Tyout Chiadma · 500ml

---

### 3.9 Differentiators — `.diff-grid` / `.diff-card`

- 3-column grid with 1px `--line` gutters (card-per-cell on cream)
- Numbered circle mark (gold border, Fraunces italic)
- H3 + supporting paragraph

**Themes:** Traçabilité réelle · Terroir protégé · Production limitée

---

### 3.10 Contact / CTA — `.cta-section`

- Gradient pine background, centered copy
- Eyebrow: *Restaurateurs & Importateurs*
- Italic Fraunces H2
- Dual CTAs: phone (`tel:+212661990570`) + anchor to form
- Inline form below

---

### 3.11 Form — `.inline-form`

Minimal underline-style inputs on dark background.

| Field | Type | Notes |
|-------|------|-------|
| Nom / Restaurant | text | required |
| Vous êtes | select | Restaurant, Importateur, Épicerie, Particulier |
| Email | email | required |
| Message | textarea | 3 rows |

**Submit:** `.btn-gold` button · demo handler shows confirmation in `.form-msg`  
**Success copy:** *Merci — votre demande a été notée. Nous revenons vers vous rapidement.*

---

### 3.12 Footer

- Background `#0F1C14`
- Top row: wordmark + three columns (Exploitation, Contact, Suivre)
- Bottom row: copyright + récolte line
- Links hover to `--gold-soft`

**Contact constants:**
- Email: `contact@zaho-oil.com`
- Phone: `+212 661 990 570`
- Legal entity: Blad Atouaa Elmahdi

---

## 4. Layout breakpoints

| Breakpoint | Key changes |
|------------|-------------|
| **≤ 860px** | Single-column origine/produit grids; process steps stack; SVG track hidden; diff grid stacks; reduced section padding; nav links hidden |

No tablet-specific breakpoint — design jumps at 860px only.

---

## 5. Iconography & illustration

| Asset | Style |
|-------|-------|
| Hero branch SVG | Minimal line art — pine green stem `#3E5A3A`, gold olive ellipses `#C7A24A`, 1–1.4px strokes |
| Process path SVG | Single gold curved connector, no fill |
| Diff marks | Typography-based (1, 2, 3) in gold circles — no icon set |

No external icon library. Decorative graphics are inline SVG only.

---

## 6. Photography direction

| Context | Direction |
|---------|-----------|
| **Origine** | SHD olive grove, early morning light, Had Touabet landscape |
| **Produit** | ZAHO bottle, neutral background, natural light — premium but honest |
| **General** | Warm, earthy, not over-retouched; align with cream/gold/pine palette |
| **Avoid** | Stock « Mediterranean sunset » clichés, heavy filters, busy backgrounds |

---

## 7. CSS architecture (reference)

```
:root          → design tokens
Global reset   → *, html, body
Utilities      → .eyebrow, .reveal
Layout blocks  → nav, section, footer
Components     → .btn-*, .photo-slot, .diff-*, form, etc.
Responsive     → @media (max-width: 860px)
A11y           → :focus-visible, prefers-reduced-motion
```

**JS dependencies (minimal):**
- Scroll listener for nav
- `IntersectionObserver` for reveals and process path
- Form submit preventDefault (demo only)

---

## 8. Page map & anchors

| ID | Section | Nav label |
|----|---------|-----------|
| `#origine` | Notre terroir | Origine |
| `#processus` | De l'arbre à la bouteille | Processus |
| `#produit` | La bouteille | Produit |
| `#pourquoi` | Pourquoi ZAHO | — (not in nav) |
| `#contact` | Restaurateurs & Importateurs | Contact |
| `#form` | Sample request form | — |

---

## 9. Quick reference — copy-paste tokens

```css
:root {
  --pine: #14261C;
  --pine-mid: #2C4229;
  --gold: #C7A24A;
  --gold-soft: #E4C878;
  --cream: #F4EFE1;
  --ink: #1E1B12;
  --line: #DCD3B8;
}
```

```css
/* Typography stack */
font-family: 'Fraunces', serif;   /* display */
font-family: 'Work Sans', sans-serif; /* UI / body */
```

---

## 10. Extension guidelines

When adding new pages or components:

1. **Colors:** use CSS variables only; do not introduce new accent hues without brand approval.
2. **Type:** Fraunces for headings and brand moments; Work Sans for everything else.
3. **Spacing:** maintain `6vw` horizontal rhythm and 120px/80px vertical section padding.
4. **Buttons:** only `.btn-gold`, `.btn-ghost`, or `.nav-cta` variants — no third button style.
5. **Dark sections:** pair `--pine` backgrounds with `--cream` / `--gold-soft` text; invert eyebrows accordingly.
6. **Motion:** reuse `.reveal` + IntersectionObserver pattern; avoid gratuitous animation.
7. **Imagery:** replace `.photo-slot` placeholders before launch; keep aspect ratios (4/5 or 3/4).
8. **B2B focus:** CTAs should favor sample requests and direct contact over e-commerce patterns.

---

*Document version: 1.0 · Source: ZAHO single-page HTML · © 2026 ZAHO — Blad Atouaa Elmahdi*
