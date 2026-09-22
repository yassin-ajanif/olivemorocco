/**
 * ZAHO Gestion — client-side data store (localStorage, no backend).
 */
const ZahoStore = (() => {
  const STORAGE_KEY = 'zaho_gestion_data';
  const DATA_VERSION = 2;

  const PREFIXES = {
    devis: 'DEV',
    'bons-commande': 'BCC',
    'bons-livraison': 'BL',
    facturation: 'FAC',
    avoirs: 'AVC',
    'bons-commande-achat': 'BC',
    'bons-reception': 'BR',
    'factures-fournisseurs': 'FF',
    'avoir-fournisseur': 'AVF',
    charges: 'CHG',
  };

  function docTotals(lines) {
    if (typeof ZahoDocLines !== 'undefined') {
      const t = ZahoDocLines.calcTotals(lines);
      return { lines, ht: t.ht, tva: t.tva, ttc: t.ttc };
    }
    return { lines, ht: 0, tva: 0, ttc: 0 };
  }

  const seed = () => {
    const L = {
      zaho500: { reference: 'ZAHO-500', designation: 'Huile ZAHO Extra Vierge 500ml', quantite: 24, unite: 'btl', prixUnitaireHt: 166.67, remise: 0, tauxTva: 20 },
      zaho750: { reference: 'ZAHO-750', designation: 'Huile ZAHO Extra Vierge 750ml', quantite: 120, unite: 'btl', prixUnitaireHt: 208.33, remise: 0, tauxTva: 20 },
      zaho5L: { reference: 'ZAHO-5L', designation: 'Huile ZAHO Extra Vierge 5L', quantite: 20, unite: 'bidon', prixUnitaireHt: 750, remise: 0, tauxTva: 20 },
      zahoCad: { reference: 'ZAHO-CAD', designation: 'Coffret cadeau ZAHO (2×500ml)', quantite: 6, unite: 'u', prixUnitaireHt: 350, remise: 0, tauxTva: 20 },
      zaho500ret: { reference: 'ZAHO-500', designation: 'Huile ZAHO Extra Vierge 500ml', quantite: 6, unite: 'btl', prixUnitaireHt: 166.67, remise: 0, tauxTva: 20 },
      ocp: { reference: 'OCP-NPK', designation: 'Engrais NPK 15-15-15 (sac 50 kg)', quantite: 10, unite: 'sac', prixUnitaireHt: 450, remise: 0, tauxTva: 20 },
      btl: { reference: 'EMB-BTL500', designation: 'Bouteille verre 500ml', quantite: 500, unite: 'u', prixUnitaireHt: 8.5, remise: 0, tauxTva: 20 },
      btlRet: { reference: 'EMB-BTL500', designation: 'Bouteille verre 500ml', quantite: 50, unite: 'u', prixUnitaireHt: 8.5, remise: 0, tauxTva: 20 },
    };

    const d1 = docTotals([L.zaho500]);
    const d2 = docTotals([L.zaho750, L.zaho5L]);
    const bc1 = docTotals([L.zaho500]);
    const bl1 = docTotals([L.zaho500]);
    const fac1 = docTotals([L.zaho500, L.zahoCad]);
    const av1 = docTotals([L.zaho500ret]);
    const bca1 = docTotals([L.ocp, L.btl]);
    const br1 = docTotals([L.ocp, L.btl]);
    const ff1 = docTotals([L.ocp, L.btl]);
    const avf1 = docTotals([L.btlRet]);

    return {
    version: DATA_VERSION,
    clients: [
      { id: 'c1', nom: 'Restaurant Taros', ice: '', ville: 'Essaouira', adresse: '', telephone: '', email: '', actif: true },
      { id: 'c2', nom: 'Importateur Oliva EU', ice: 'FR123456789', ville: 'Marseille', adresse: '', telephone: '', email: '', actif: true },
    ],
    fournisseurs: [
      { id: 'f1', nom: 'OCP Engrais', ice: 'MA001234567', ville: 'Casablanca', adresse: '', telephone: '', email: '', actif: true },
    ],
    devis: [
      {
        id: 'd1', numero: 'DEV-2026-001', clientId: 'c1', clientNom: 'Restaurant Taros',
        date: '2026-01-12', validite: '2026-02-12', note: 'Commande mensuelle — ZAHO 500ml',
        ...d1,
      },
      {
        id: 'd2', numero: 'DEV-2026-002', clientId: 'c2', clientNom: 'Importateur Oliva EU',
        date: '2026-01-20', validite: '2026-03-20', note: 'Export palette — 750ml + 5L',
        ...d2,
      },
    ],
    'bons-commande': [
      {
        id: 'bc1', numero: 'BCC-2026-001', clientId: 'c1', clientNom: 'Restaurant Taros',
        date: '2026-01-15', note: 'Confirmé suite devis DEV-2026-001',
        ...bc1,
      },
    ],
    'bons-livraison': [
      {
        id: 'bl1', numero: 'BL-2026-001', clientId: 'c1', clientNom: 'Restaurant Taros',
        date: '2026-01-18', note: 'Livraison Essaouira', facture: true,
        ...bl1,
      },
    ],
    facturation: [
      {
        id: 'fac1', numero: 'FAC-2026-001', clientId: 'c1', clientNom: 'Restaurant Taros',
        date: '2026-01-20', echeance: '2026-02-20', payee: true, note: 'Facture BL-2026-001 + coffrets',
        ...fac1,
      },
    ],
    avoirs: [
      {
        id: 'av1', numero: 'AVC-2026-001', clientId: 'c1', clientNom: 'Restaurant Taros',
        date: '2026-01-25', note: 'Retour 6 bouteilles endommagées',
        ...av1,
      },
    ],
    'bons-commande-achat': [
      {
        id: 'bca1', numero: 'BC-2026-001', fournisseurId: 'f1', fournisseurNom: 'OCP Engrais',
        date: '2026-01-10', note: 'Intrants campagne 2026',
        ...bca1,
      },
    ],
    'bons-reception': [
      {
        id: 'br1', numero: 'BR-2026-001', fournisseurId: 'f1', fournisseurNom: 'OCP Engrais',
        date: '2026-01-14', note: 'Réception entrepôt',
        ...br1,
      },
    ],
    'factures-fournisseurs': [
      {
        id: 'ff1', numero: 'FF-2026-001', fournisseurId: 'f1', fournisseurNom: 'OCP Engrais',
        date: '2026-01-16', note: 'Facture fournisseur BC-2026-001',
        ...ff1,
      },
    ],
    'avoir-fournisseur': [
      {
        id: 'avf1', numero: 'AVF-2026-001', fournisseurId: 'f1', fournisseurNom: 'OCP Engrais',
        date: '2026-01-22', note: 'Avoir bouteilles cassées à la livraison',
        ...avf1,
      },
    ],
    charges: [
      { id: 'chg1', type: 'Transport', date: '2026-01-19', libelle: 'Transport Casablanca → Essaouira', beneficiaire: 'Trans Atlas', ttc: 850, note: 'Palette huile' },
    ],
    counters: {
      'devis-2026': 2,
      'bons-commande-2026': 1,
      'bons-livraison-2026': 1,
      'facturation-2026': 1,
      'avoirs-2026': 1,
      'bons-commande-achat-2026': 1,
      'bons-reception-2026': 1,
      'factures-fournisseurs-2026': 1,
      'avoir-fournisseur-2026': 1,
      'charges-2026': 1,
    },
  };
  };

  let data = null;

  function mergeSeedIfNeeded() {
    if ((data.version || 0) >= DATA_VERSION) return;
    const fresh = seed();
    if (!data.counters || !Object.keys(data.counters).length) data.counters = fresh.counters;
    const collections = [
      'devis', 'bons-commande', 'bons-livraison', 'facturation', 'avoirs',
      'bons-commande-achat', 'bons-reception', 'factures-fournisseurs', 'avoir-fournisseur', 'charges',
    ];
    collections.forEach(key => {
      if (!data[key]?.length && fresh[key]?.length) data[key] = fresh[key];
    });
    data.version = DATA_VERSION;
    persist();
  }

  function load() {
    try {
      const raw = localStorage.getItem(STORAGE_KEY);
      data = raw ? JSON.parse(raw) : seed();
      if (!data.counters) data.counters = {};
      if (!data.version) data.version = 0;
      mergeSeedIfNeeded();
    } catch {
      data = seed();
    }
    return data;
  }

  function persist() {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
  }

  function uid() {
    return `${Date.now()}-${Math.random().toString(36).slice(2, 9)}`;
  }

  function init() {
    if (!data) load();
    return data;
  }

  const DOC_COLLECTIONS = [
    'devis', 'bons-commande', 'bons-livraison', 'facturation', 'avoirs',
    'bons-commande-achat', 'bons-reception', 'factures-fournisseurs', 'avoir-fournisseur',
  ];

  function getAll(collection) {
    init();
    const list = [...(data[collection] || [])];
    if (DOC_COLLECTIONS.includes(collection) && typeof ZahoDocLines !== 'undefined') {
      return list.map(d => ZahoDocLines.migrateDocument(d));
    }
    return list;
  }

  function getById(collection, id) {
    const item = getAll(collection).find(i => i.id === id) || null;
    return item;
  }

  function nextNumero(moduleKey) {
    init();
    const prefix = PREFIXES[moduleKey];
    if (!prefix) return '';
    const year = new Date().getFullYear();
    const key = `${moduleKey}-${year}`;
    const n = (data.counters[key] || 0) + 1;
    data.counters[key] = n;
    persist();
    return `${prefix}-${year}-${String(n).padStart(3, '0')}`;
  }

  function create(collection, item) {
    init();
    if (!data[collection]) data[collection] = [];
    const record = { id: uid(), ...item };
    data[collection].push(record);
    persist();
    return record;
  }

  function update(collection, id, patch) {
    init();
    const list = data[collection] || [];
    const i = list.findIndex(x => x.id === id);
    if (i === -1) return null;
    list[i] = { ...list[i], ...patch };
    persist();
    return list[i];
  }

  function remove(collection, id) {
    init();
    const list = data[collection] || [];
    const i = list.findIndex(x => x.id === id);
    if (i === -1) return false;
    list.splice(i, 1);
    persist();
    return true;
  }

  function resolveClientName(clientId) {
    const c = getById('clients', clientId);
    return c ? c.nom : '';
  }

  function resolveFournisseurName(fournisseurId) {
    const f = getById('fournisseurs', fournisseurId);
    return f ? f.nom : '';
  }

  function reset() {
    data = seed();
    persist();
  }

  function getDemoProducts(type) {
    if (typeof ZahoDocLines !== 'undefined') return ZahoDocLines.getDemoProducts(type);
    return [];
  }

  return {
    init, getAll, getById, create, update, remove, nextNumero,
    resolveClientName, resolveFournisseurName, reset, PREFIXES, getDemoProducts,
  };
})();

if (typeof window !== 'undefined') window.ZahoStore = ZahoStore;
