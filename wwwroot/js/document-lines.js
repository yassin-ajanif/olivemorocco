/**
 * ZAHO Gestion — document line items & totals (Devis, BC, BL, Factures…).
 */
const ZahoDocLines = (() => {
  function emptyLine() {
    return {
      reference: '',
      designation: '',
      quantite: 1,
      unite: 'u',
      prixUnitaireHt: 0,
      remise: 0,
      tauxTva: 20,
    };
  }

  const DEMO_PRODUCTS_VENTE = [
    { reference: 'ZAHO-500', designation: 'Huile ZAHO Extra Vierge 500ml', unite: 'btl', prixUnitaireHt: 166.67, tauxTva: 20 },
    { reference: 'ZAHO-750', designation: 'Huile ZAHO Extra Vierge 750ml', unite: 'btl', prixUnitaireHt: 208.33, tauxTva: 20 },
    { reference: 'ZAHO-5L', designation: 'Huile ZAHO Extra Vierge 5L', unite: 'bidon', prixUnitaireHt: 750, tauxTva: 20 },
    { reference: 'CHIADMA-1L', designation: 'Huile Chiadma d\'Or 1L', unite: 'btl', prixUnitaireHt: 83.33, tauxTva: 20 },
    { reference: 'ZAHO-CAD', designation: 'Coffret cadeau ZAHO (2×500ml)', unite: 'u', prixUnitaireHt: 350, tauxTva: 20 },
  ];

  const DEMO_PRODUCTS_ACHAT = [
    { reference: 'OCP-NPK', designation: 'Engrais NPK 15-15-15 (sac 50 kg)', unite: 'sac', prixUnitaireHt: 450, tauxTva: 20 },
    { reference: 'EMB-BTL500', designation: 'Bouteille verre 500ml', unite: 'u', prixUnitaireHt: 8.5, tauxTva: 20 },
    { reference: 'ETIQ-ZAHO', designation: 'Étiquette ZAHO 500ml', unite: 'u', prixUnitaireHt: 2.2, tauxTva: 20 },
    { reference: 'FILT-HUile', designation: 'Filtre à huile industriel', unite: 'u', prixUnitaireHt: 1200, tauxTva: 20 },
    { reference: 'PAL-EURO', designation: 'Palette EUR (location)', unite: 'u', prixUnitaireHt: 85, tauxTva: 20 },
  ];

  function getDemoProducts(type) {
    return type === 'achat' ? DEMO_PRODUCTS_ACHAT : DEMO_PRODUCTS_VENTE;
  }

  function productToLine(product, quantite = 1) {
    return {
      reference: product.reference,
      designation: product.designation,
      quantite,
      unite: product.unite,
      prixUnitaireHt: product.prixUnitaireHt,
      remise: product.remise || 0,
      tauxTva: product.tauxTva ?? 20,
    };
  }

  function demoLine(type = 'vente', index = 0) {
    const products = getDemoProducts(type);
    const product = products[index % products.length];
    const qty = type === 'vente'
      ? (index === 0 ? 24 : index === 1 ? 12 : 1)
      : (index === 0 ? 10 : index === 1 ? 500 : 1);
    return productToLine(product, qty);
  }

  function defaultLinesForNewDoc(type) {
    return [demoLine(type, 0), demoLine(type, 1)];
  }

  function calcLine(line) {
    const q = parseFloat(line.quantite) || 0;
    const pu = parseFloat(line.prixUnitaireHt) || 0;
    const rem = parseFloat(line.remise) || 0;
    const tva = parseFloat(line.tauxTva) || 0;
    const ht = q * pu * (1 - rem / 100);
    const ttc = ht * (1 + tva / 100);
    return { ht, tva: ttc - ht, ttc };
  }

  function calcTotals(lines) {
    return (lines || []).reduce(
      (acc, line) => {
        const c = calcLine(line);
        acc.ht += c.ht;
        acc.tva += c.tva;
        acc.ttc += c.ttc;
        return acc;
      },
      { ht: 0, tva: 0, ttc: 0 }
    );
  }

  function fmt(n) {
    return (parseFloat(n) || 0).toLocaleString('fr-FR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  }

  function normalizeLines(lines, type = 'vente') {
    if (!lines || !lines.length) return [demoLine(type, 0)];
    return lines.map((l, i) => {
      const isEmpty = !l.reference && !l.designation && !(parseFloat(l.prixUnitaireHt) > 0);
      if (isEmpty) return demoLine(type, i);
      return { ...demoLine(type, i), ...l };
    });
  }

  /** Migrate legacy documents that only had a ttc field */
  function migrateDocument(doc) {
    if (doc.lines && doc.lines.length) {
      const hasContent = doc.lines.some(l => l.designation || l.reference || parseFloat(l.prixUnitaireHt) > 0);
      if (hasContent) return doc;
    }
    const lines = [demoLine('vente', 0)];
    if (doc.ttc) {
      lines[0].designation = doc.note || lines[0].designation;
      lines[0].prixUnitaireHt = doc.ttc / 1.2;
      lines[0].tauxTva = 20;
      lines[0].quantite = 1;
    }
    const totals = calcTotals(lines);
    return { ...doc, lines, ht: totals.ht, tva: totals.tva, ttc: totals.ttc };
  }

  return { emptyLine, calcLine, calcTotals, fmt, normalizeLines, migrateDocument,
    DEMO_PRODUCTS_VENTE, DEMO_PRODUCTS_ACHAT, getDemoProducts, productToLine, demoLine, defaultLinesForNewDoc };
})();

if (typeof window !== 'undefined') window.ZahoDocLines = ZahoDocLines;
