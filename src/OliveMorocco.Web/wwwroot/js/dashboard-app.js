/**
 * ZAHO Gestion — dashboard UI + CRUD (list / form views).
 */
(() => {
  const state = {
    module: 'clients',
    view: 'list',
    editId: null,
    selectedId: null,
    search: '',
    treePage: 1,
    selectedTreeId: null,
    treeQuery: '',
  };

  const TIERS_MODULES = ['clients', 'fournisseurs'];
  const DOC_CLIENT_MODULES = ['devis', 'bons-commande', 'bons-livraison', 'facturation', 'avoirs'];
  const DOC_FOURNISSEUR_MODULES = ['bons-commande-achat', 'bons-reception', 'factures-fournisseurs', 'avoir-fournisseur', 'charges'];

  const MODULE_META = {
    clients: { section: 'Vente', title: 'Clients', subtitle: 'Restaurateurs, importateurs, épiciers', collection: 'clients', crud: true },
    devis: { section: 'Vente', title: 'Devis', subtitle: 'Devis commerciaux', collection: 'devis', crud: true },
    'bons-commande': { section: 'Vente', title: 'Bons de commande', subtitle: 'Commandes clients', collection: 'bons-commande', crud: true },
    'bons-livraison': { section: 'Vente', title: 'Bons de livraison', subtitle: 'Expéditions clients', collection: 'bons-livraison', crud: true },
    facturation: { section: 'Vente', title: 'Facturation', subtitle: 'Factures clients', collection: 'facturation', crud: true },
    avoirs: { section: 'Vente', title: 'Avoirs', subtitle: 'Notes de crédit clients', collection: 'avoirs', crud: true },
    fournisseurs: { section: 'Achat', title: 'Fournisseurs', subtitle: 'Répertoire fournisseurs', collection: 'fournisseurs', crud: true },
    'bons-commande-achat': { section: 'Achat', title: 'Bons de commande', subtitle: 'Commandes fournisseurs', collection: 'bons-commande-achat', crud: true },
    'bons-reception': { section: 'Achat', title: 'Bons de réception', subtitle: 'Réceptions fournisseurs', collection: 'bons-reception', crud: true },
    'factures-fournisseurs': { section: 'Achat', title: 'Factures fournisseur', subtitle: 'Factures reçues', collection: 'factures-fournisseurs', crud: true },
    'avoir-fournisseur': { section: 'Achat', title: 'Avoir fournisseur', subtitle: 'Avoirs fournisseurs', collection: 'avoir-fournisseur', crud: true },
    charges: { section: 'Achat', title: 'Charges', subtitle: 'Charges d\'exploitation', collection: 'charges', crud: true },
    parcelles: { section: 'Opérationnel', title: 'Parcelles & blocs', subtitle: '25 ha SHD', crud: false },
    'recolte-lots': { section: 'Opérationnel', title: 'Récolte & lots', subtitle: 'Allocation ZAHO / Chiadma d\'Or', crud: false },
    intrants: { section: 'Opérationnel', title: 'Intrants', subtitle: 'Engrais par parcelle', crud: false },
    tracabilite: { section: 'Opérationnel', title: 'Traçabilité QR', subtitle: 'QR-code Tier 1', crud: false },
    'prix-marche': { section: 'Opérationnel', title: 'Prix marché', subtitle: 'Prix courants', crud: false },
  };

  function fmtDate(iso) {
    if (!iso) return '—';
    const [y, m, d] = iso.split('-');
    return `${d}/${m}/${y.slice(2)}`;
  }

  function fmtMoney(n) {
    if (n == null || n === '') return '—';
    return `<span class="cell-bold">${Number(n).toLocaleString('fr-FR')} DH</span>`;
  }

  function badgeActif(actif) {
    return actif
      ? '<span class="badge badge-ok">Actif</span>'
      : '<span class="badge badge-warn">Inactif</span>';
  }

  function todayISO() {
    return new Date().toISOString().slice(0, 10);
  }

  function addDaysISO(days) {
    const d = new Date();
    d.setDate(d.getDate() + days);
    return d.toISOString().slice(0, 10);
  }

  function filterItems(items, q) {
    if (!q) return items;
    const s = q.toLowerCase();
    return items.filter(row => JSON.stringify(row).toLowerCase().includes(s));
  }

  const LIST_SCHEMAS = {
    clients: { headers: ['Nom', 'ICE', 'Ville', 'Actif'], cols: '2fr 1fr 1fr auto' },
    fournisseurs: { headers: ['Nom', 'ICE', 'Ville', 'Actif'], cols: '2fr 1fr 1fr auto' },
    devis: { headers: ['Réf.', 'Client', 'Date', 'Validité', 'TTC', 'Note'], cols: '140px 1fr 100px 110px 100px 1fr' },
    'bons-livraison': { headers: ['Réf.', 'Client', 'Date', 'TTC', 'Note', 'Statut'], cols: '120px 1fr 100px 100px 1fr 120px' },
    facturation: { headers: ['Réf.', 'Client', 'Date', 'Échéance', 'Payée', 'TTC', 'Note'], cols: '1fr 1fr 100px 100px 90px 100px 1fr' },
    charges: { headers: ['Type', 'Date', 'Libellé', 'Bénéficiaire', 'TTC', 'Note'], cols: '110px 100px 1fr 140px 100px 1fr' },
    docParty: { headers: ['Réf.', 'Partie', 'Date', 'TTC', 'Note'], cols: '120px 1fr 100px 100px 1fr' },
  };

  function schemaFor(module) {
    if (LIST_SCHEMAS[module]) return LIST_SCHEMAS[module];
    if (DOC_FOURNISSEUR_MODULES.includes(module) && module !== 'charges') {
      return { headers: ['Réf.', 'Fournisseur', 'Date', 'TTC', 'Note'], cols: LIST_SCHEMAS.docParty.cols };
    }
    if (DOC_CLIENT_MODULES.includes(module)) {
      return { headers: ['Réf.', 'Client', 'Date', 'TTC', 'Note'], cols: LIST_SCHEMAS.docParty.cols };
    }
    return LIST_SCHEMAS.docParty;
  }

  /* ── Row mappers (store → list cells) ── */
  function tierRows(collection, items) {
    return items.map(r => ({
      id: r.id,
      cells: [r.nom, r.ice || '—', r.ville || '—', badgeActif(r.actif)],
      cols: '2fr 1fr 1fr auto',
      headers: ['Nom', 'ICE', 'Ville', 'Actif'],
    }));
  }

  function docClientRows(module, items) {
    if (module === 'devis') {
      return items.map(r => ({
        id: r.id,
        cells: [r.numero, r.clientNom || '—', fmtDate(r.date), fmtDate(r.validite), fmtMoney(r.ttc), r.note || '—'],
        cols: '140px 1fr 100px 110px 100px 1fr',
        headers: ['Réf.', 'Client', 'Date', 'Validité', 'TTC', 'Note'],
      }));
    }
    if (module === 'bons-livraison') {
      return items.map(r => ({
        id: r.id,
        cells: [r.numero, r.clientNom || '—', fmtDate(r.date), fmtMoney(r.ttc), r.note || '—',
          r.facture ? '<span class="badge badge-ok">Facturé</span>' : '<span class="badge badge-warn">En attente</span>'],
        cols: '120px 1fr 100px 100px 1fr 120px',
        headers: ['Réf.', 'Client', 'Date', 'TTC', 'Note', 'Statut'],
      }));
    }
    if (module === 'facturation') {
      return items.map(r => ({
        id: r.id,
        cells: [r.numero, r.clientNom || '—', fmtDate(r.date), fmtDate(r.echeance), r.payee ? 'Oui' : 'Non', fmtMoney(r.ttc), r.note || '—'],
        cols: '1fr 1fr 100px 100px 90px 100px 1fr',
        headers: ['Réf.', 'Client', 'Date', 'Échéance', 'Payée', 'TTC', 'Note'],
      }));
    }
    return items.map(r => ({
      id: r.id,
      cells: [r.numero, r.clientNom || '—', fmtDate(r.date), fmtMoney(r.ttc), r.note || '—'],
      cols: '120px 1fr 100px 100px 1fr',
      headers: ['Réf.', 'Client', 'Date', 'TTC', 'Note'],
    }));
  }

  function docFournisseurRows(module, items) {
    if (module === 'charges') {
      return items.map(r => ({
        id: r.id,
        cells: [r.type || '—', fmtDate(r.date), r.libelle || '—', r.beneficiaire || '—', fmtMoney(r.ttc), r.note || '—'],
        cols: '110px 100px 1fr 140px 100px 1fr',
        headers: ['Type', 'Date', 'Libellé', 'Bénéficiaire', 'TTC', 'Note'],
      }));
    }
    const partyLabel = 'Fournisseur';
    return items.map(r => ({
      id: r.id,
      cells: [r.numero, r.fournisseurNom || '—', fmtDate(r.date), fmtMoney(r.ttc), r.note || '—'],
      cols: '120px 1fr 100px 100px 1fr',
      headers: ['Réf.', partyLabel, 'Date', 'TTC', 'Note'],
    }));
  }

  function getListConfig(module) {
    const meta = MODULE_META[module];
    if (!meta?.crud) return null;
    const collection = meta.collection;
    let items = ZahoStore.getAll(collection);
    items = filterItems(items, state.search);
    const schema = schemaFor(module);

    if (TIERS_MODULES.includes(module)) {
      return { meta, items, rows: tierRows(collection, items), headers: schema.headers, cols: schema.cols };
    }
    if (DOC_CLIENT_MODULES.includes(module)) {
      return { meta, items, rows: docClientRows(module, items), headers: schema.headers, cols: schema.cols };
    }
    if (DOC_FOURNISSEUR_MODULES.includes(module)) {
      return { meta, items, rows: docFournisseurRows(module, items), headers: schema.headers, cols: schema.cols };
    }
    return null;
  }

  /* ── Form builders ── */
  function field(label, name, value, type = 'text', opts = {}) {
    const req = opts.required ? ' required' : '';
    if (type === 'textarea') {
      return `<label class="form-field"><span>${label}</span><textarea name="${name}" rows="3"${req}>${esc(value || '')}</textarea></label>`;
    }
    if (type === 'select') {
      const optsHtml = opts.options.map(o =>
        `<option value="${esc(o.value)}"${o.value === value ? ' selected' : ''}>${esc(o.label)}</option>`
      ).join('');
      return `<label class="form-field"><span>${label}</span><select name="${name}"${req}>${optsHtml}</select></label>`;
    }
    if (type === 'checkbox') {
      return `<label class="form-field form-check"><input type="checkbox" name="${name}"${value ? ' checked' : ''}><span>${label}</span></label>`;
    }
    return `<label class="form-field"><span>${label}</span><input type="${type}" name="${name}" value="${esc(value ?? '')}"${req}></label>`;
  }

  function esc(s) {
    return String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/"/g, '&quot;');
  }

  function clientOptions(selectedId) {
    return ZahoStore.getAll('clients').filter(c => c.actif).map(c => ({ value: c.id, label: c.nom }));
  }

  function fournisseurOptions(selectedId) {
    return ZahoStore.getAll('fournisseurs').filter(f => f.actif).map(f => ({ value: f.id, label: f.nom }));
  }

  function renderTierForm(module, record) {
    const isEdit = !!record;
    return `
      <form id="crudForm" class="form-grid" data-module="${module}">
        ${field('Nom', 'nom', record?.nom, 'text', { required: true })}
        ${field('ICE', 'ice', record?.ice)}
        ${field('Adresse', 'adresse', record?.adresse)}
        ${field('Ville', 'ville', record?.ville)}
        ${field('Téléphone', 'telephone', record?.telephone, 'tel')}
        ${field('Email', 'email', record?.email, 'email')}
        ${field('Actif', 'actif', record?.actif !== false, 'checkbox')}
      </form>`;
  }

  const DOC_WITH_LINES = [
    ...DOC_CLIENT_MODULES,
    'bons-commande-achat', 'bons-reception', 'factures-fournisseurs', 'avoir-fournisseur',
  ];

  function isDocWithLines(module) {
    return DOC_WITH_LINES.includes(module);
  }

  function renderLinesTable(lines, productType) {
    const products = ZahoDocLines.getDemoProducts(productType);
    const productOptions = products.map(p =>
      `<option value="${esc(p.reference)}">${esc(p.reference)} — ${esc(p.designation)}</option>`
    ).join('');

    const rows = ZahoDocLines.normalizeLines(lines, productType).map((line, i) => {
      const c = ZahoDocLines.calcLine(line);
      return `<tr data-line="${i}">
        <td class="col-ref"><input type="text" name="line_ref_${i}" value="${esc(line.reference)}" data-line-field="reference"></td>
        <td class="col-designation"><input type="text" name="line_des_${i}" value="${esc(line.designation)}" data-line-field="designation"></td>
        <td class="col-qty"><input type="number" step="any" min="0" name="line_qty_${i}" value="${line.quantite}" data-line-field="quantite"></td>
        <td class="col-unite"><input type="text" name="line_unit_${i}" value="${esc(line.unite)}" data-line-field="unite"></td>
        <td class="col-pu"><input type="number" step="0.01" min="0" name="line_pu_${i}" value="${line.prixUnitaireHt}" data-line-field="prixUnitaireHt"></td>
        <td class="col-rem"><input type="number" step="0.01" min="0" max="100" name="line_rem_${i}" value="${line.remise}" data-line-field="remise"></td>
        <td class="col-tva"><input type="number" step="0.01" min="0" name="line_tva_${i}" value="${line.tauxTva}" data-line-field="tauxTva"></td>
        <td class="col-ht" data-line-ht="${i}">${ZahoDocLines.fmt(c.ht)}</td>
        <td class="col-ttc" data-line-ttc="${i}">${ZahoDocLines.fmt(c.ttc)}</td>
        <td class="col-actions"><button type="button" class="btn-line-remove" data-action="remove-line" title="Supprimer">×</button></td>
      </tr>`;
    }).join('');

    return `<div class="doc-lines-section">
      <div class="doc-lines-toolbar">
        <button type="button" class="btn" data-action="add-line">+ Ajouter une ligne</button>
        <select class="doc-product-pick" data-action="pick-product" aria-label="Produit catalogue">
          <option value="">— Produit catalogue —</option>
          ${productOptions}
        </select>
      </div>
      <div class="doc-lines-scroll">
        <table class="doc-lines-table">
          <thead><tr>
            <th class="col-ref">Réf.</th>
            <th class="col-designation">Désignation</th>
            <th class="col-qty">Qté</th>
            <th class="col-unite">Unité</th>
            <th class="col-pu">P.U. HT</th>
            <th class="col-rem">Rem. %</th>
            <th class="col-tva">TVA %</th>
            <th class="col-ht">Montant HT</th>
            <th class="col-ttc">Montant TTC</th>
            <th class="col-actions"></th>
          </tr></thead>
          <tbody id="docLinesBody">${rows}</tbody>
        </table>
      </div>
    </div>`;
  }

  function renderTotalsBox(totals) {
    return `<div class="doc-totals" id="docTotals">
      <h3>Totaux</h3>
      <div class="doc-totals-row"><span>HT</span><span id="totalHt">${ZahoDocLines.fmt(totals.ht)} MAD</span></div>
      <div class="doc-totals-row"><span>TVA</span><span id="totalTva">${ZahoDocLines.fmt(totals.tva)} MAD</span></div>
      <div class="doc-totals-row doc-totals-ttc"><span>TTC</span><span id="totalTtc">${ZahoDocLines.fmt(totals.ttc)} MAD</span></div>
    </div>`;
  }

  function renderDocumentForm(module, record, partyType) {
    const isClient = partyType === 'client';
    const partyOptions = isClient ? clientOptions(record?.clientId) : fournisseurOptions(record?.fournisseurId);
    const partyField = isClient ? 'clientId' : 'fournisseurId';
    const partyLabel = isClient ? 'Client' : 'Fournisseur';
    const productType = isClient ? 'vente' : 'achat';
    const lines = record?.lines
      ? ZahoDocLines.normalizeLines(record.lines, productType)
      : ZahoDocLines.defaultLinesForNewDoc(productType);
    const totals = ZahoDocLines.calcTotals(lines);

    let extraMeta = '';
    if (module === 'devis') extraMeta = field('Validité', 'validite', record?.validite || addDaysISO(30), 'date');
    if (module === 'facturation') {
      extraMeta = field('Échéance', 'echeance', record?.echeance || addDaysISO(30), 'date')
        + field('Payée', 'payee', record?.payee, 'checkbox');
    }
    if (module === 'bons-livraison') extraMeta = field('Facturé', 'facture', record?.facture, 'checkbox');

    return `<form id="crudForm" class="form-grid" data-module="${module}" data-doc-form="1" data-product-type="${productType}">
      ${record?.numero ? `<p class="form-ref">Réf. document <strong>${esc(record.numero)}</strong></p>` : ''}
      <div class="doc-meta">
        ${field(partyLabel, partyField, record?.[partyField], 'select', { required: true, options: [{ value: '', label: '— Choisir —' }, ...partyOptions] })}
        ${field('Date', 'date', record?.date || todayISO(), 'date', { required: true })}
        ${extraMeta}
      </div>
      ${renderLinesTable(lines, productType)}
      <div class="doc-footer">
        <div class="doc-notes-wrap">
          <label for="docNote">Notes</label>
          <textarea id="docNote" name="note" placeholder="Conditions, remarques…">${esc(record?.note || '')}</textarea>
        </div>
        ${renderTotalsBox(totals)}
      </div>
    </form>`;
  }

  function renderDocClientForm(module, record) {
    if (isDocWithLines(module)) return renderDocumentForm(module, record, 'client');
    return `<form id="crudForm" class="form-grid" data-module="${module}"></form>`;
  }

  function renderDocFournisseurForm(module, record) {
    if (module === 'charges') {
      return `<form id="crudForm" class="form-grid" data-module="${module}">
        ${field('Type', 'type', record?.type, 'text', { required: true })}
        ${field('Date', 'date', record?.date || todayISO(), 'date', { required: true })}
        ${field('Libellé', 'libelle', record?.libelle, 'text', { required: true })}
        ${field('Bénéficiaire', 'beneficiaire', record?.beneficiaire)}
        ${field('Montant TTC (DH)', 'ttc', record?.ttc ?? '', 'number', { required: true })}
        ${field('Note', 'note', record?.note, 'textarea')}
      </form>`;
    }
    const fournisseurs = fournisseurOptions(record?.fournisseurId);
    if (isDocWithLines(module)) return renderDocumentForm(module, record, 'fournisseur');
    return `<form id="crudForm" class="form-grid" data-module="${module}">
      ${record ? `<p class="form-ref">Réf. <strong>${esc(record.numero)}</strong></p>` : ''}
      ${field('Fournisseur', 'fournisseurId', record?.fournisseurId, 'select', { required: true, options: [{ value: '', label: '— Choisir —' }, ...fournisseurs] })}
      ${field('Date', 'date', record?.date || todayISO(), 'date', { required: true })}
      ${field('Montant TTC (DH)', 'ttc', record?.ttc ?? '', 'number', { required: true })}
      ${field('Note', 'note', record?.note, 'textarea')}
    </form>`;
  }

  function renderForm(module) {
    const meta = MODULE_META[module];
    const record = state.editId ? ZahoStore.getById(meta.collection, state.editId) : null;
    const isEdit = !!record;

    let formHtml;
    if (TIERS_MODULES.includes(module)) formHtml = renderTierForm(module, record);
    else if (DOC_CLIENT_MODULES.includes(module)) formHtml = renderDocClientForm(module, record);
    else if (DOC_FOURNISSEUR_MODULES.includes(module)) formHtml = renderDocFournisseurForm(module, record);
    else return renderList(module);

    return `
      <div class="page-head">
        <p class="eyebrow">${meta.section}</p>
        <h1>${isEdit ? 'Modifier' : 'Nouveau'} — ${meta.title.replace(/s$/, '')}</h1>
        <p class="module-section">${isEdit ? record.nom || record.numero : 'Création d\'un enregistrement'}</p>
      </div>
      <div class="form-toolbar">
        <button type="button" class="btn" data-action="back">← Retour à la liste</button>
        ${isEdit ? '<button type="button" class="btn btn-danger" data-action="delete">Supprimer</button>' : ''}
        <button type="submit" form="crudForm" class="btn btn-primary">Enregistrer</button>
      </div>
      <div class="form-panel${isDocWithLines(module) ? ' form-panel-wide' : ''}">${formHtml}</div>
    `;
  }

  /* ── Opérationnel sketches (read-only demo to explain each module) ── */
  const OPS_MODULES = ['parcelles', 'recolte-lots', 'intrants', 'tracabilite', 'prix-marche'];
  const TREES_PER_PAGE = 20;
  /** Demo GPS origin — Had Touabet / Chiadma (inland), not Essaouira harbour */
  const TREE_GPS_ORIGIN = { lat: 31.902200, lng: -9.316000 };

  const DEMO_TREE_OPS = [
    { date: '12/01/26', type: 'Fertilisation', detail: 'NPK 15-15-15 · 0,5 kg' },
    { date: '28/02/26', type: 'Taille', detail: 'Taille d\'entretien rangée' },
    { date: '15/04/26', type: 'Observation', detail: 'Feuillage OK · stress hydrique faible' },
    { date: '10/06/26', type: 'Irrigation', detail: 'Cycle 4 h · goutte-à-goutte' },
    { date: '02/09/26', type: 'Pré-récolte', detail: 'Indice maturité suivi' },
  ];

  function buildDemoTrees(count = 100) {
    const trees = [];
    const cols = 10;
    for (let i = 1; i <= count; i++) {
      const row = Math.ceil(i / cols);
      const col = ((i - 1) % cols) + 1;
      const lat = TREE_GPS_ORIGIN.lat + (row - 1) * 0.000045 + (col - 1) * 0.000008;
      const lng = TREE_GPS_ORIGIN.lng + (col - 1) * 0.000052 - (row - 1) * 0.000006;
      const statuses = ['Productif', 'Productif', 'Productif', 'Suivi', 'Productif'];
      const status = i % 17 === 0 ? 'Remplacé' : statuses[i % statuses.length];
      trees.push({
        id: `A-NORD-${String(i).padStart(3, '0')}`,
        rang: row,
        position: col,
        bloc: 'Bloc A',
        parcelle: 'A-Nord',
        variete: 'Arbequina',
        planteLe: '2021-03-15',
        statut: status,
        lat: Number(lat.toFixed(6)),
        lng: Number(lng.toFixed(6)),
        qrUrl: `https://zaho.ma/arbre/A-NORD-${String(i).padStart(3, '0')}`,
        ops: DEMO_TREE_OPS.slice(0, 2 + (i % 4)),
      });
    }
    return trees;
  }

  const DEMO_TREES = buildDemoTrees(100);

  function sketchShell(meta, bodyHtml) {
    return `
      <div class="page-head">
        <p class="eyebrow">${meta.section}</p>
        <h1>${meta.title}</h1>
        <p class="module-section">${meta.subtitle} · esquisse de compréhension</p>
      </div>
      <div class="ops-sketch">${bodyHtml}</div>`;
  }

  function statusBadge(statut) {
    if (statut === 'Productif') return '<span class="badge badge-ok">Productif</span>';
    if (statut === 'Suivi') return '<span class="badge badge-gold">Suivi</span>';
    return '<span class="badge badge-warn">Remplacé</span>';
  }

  function filteredDemoTrees() {
    const q = (state.treeQuery || '').trim().toLowerCase();
    if (!q) return DEMO_TREES;
    return DEMO_TREES.filter(t =>
      t.id.toLowerCase().includes(q)
      || String(t.rang).includes(q)
      || t.statut.toLowerCase().includes(q)
    );
  }

  function renderTreeIdentityCard(tree) {
    if (!tree) {
      return `<div class="tree-id-card tree-id-empty">
        <p>Sélectionnez un arbre dans la liste pour afficher sa <strong>carte d'identité</strong> (QR + GPS + historique).</p>
      </div>`;
    }
    const opsRows = tree.ops.map(o =>
      `<tr><td>${o.date}</td><td>${esc(o.type)}</td><td>${esc(o.detail)}</td></tr>`
    ).join('');
    const gmapsEmbed = `https://maps.google.com/maps?q=${tree.lat},${tree.lng}&z=14&hl=fr&output=embed`;

    return `<div class="tree-id-card" id="treeIdCard">
      <div class="tree-id-head">
        <img
          class="tree-qr-img"
          src="https://api.qrserver.com/v1/create-qr-code/?size=140x140&margin=8&data=${encodeURIComponent(tree.qrUrl)}"
          width="88"
          height="88"
          alt="QR code ${esc(tree.id)}"
        >
        <div>
          <p class="ops-tag">Carte d'identité arbre</p>
          <h3>${esc(tree.id)}</h3>
          <p class="tree-qr-url">${esc(tree.qrUrl)}</p>
          ${statusBadge(tree.statut)}
        </div>
      </div>
      <dl class="tree-id-dl">
        <div><dt>Bloc / parcelle</dt><dd>${esc(tree.bloc)} · ${esc(tree.parcelle)}</dd></div>
        <div><dt>Variété</dt><dd>${esc(tree.variete)}</dd></div>
        <div><dt>Rangée / position</dt><dd>R${tree.rang} · P${tree.position}</dd></div>
        <div><dt>Planté le</dt><dd>${fmtDate(tree.planteLe)}</dd></div>
        <div><dt>GPS (lat, lng)</dt><dd><code>${tree.lat}, ${tree.lng}</code></dd></div>
      </dl>
      <div class="tree-map-embed">
        <iframe
          title="Localisation ${esc(tree.id)}"
          src="${gmapsEmbed}"
          loading="lazy"
          referrerpolicy="no-referrer-when-downgrade"
          allowfullscreen
        ></iframe>
      </div>
      <p class="ops-section-title" style="margin-top:16px">Historique d'opérations (démo)</p>
      <div class="ops-table-wrap" style="margin-bottom:0"><table class="ops-table" style="min-width:0">
        <thead><tr><th>Date</th><th>Type</th><th>Détail</th></tr></thead>
        <tbody>${opsRows}</tbody>
      </table></div>
    </div>`;
  }

  function renderTreesDemoSection() {
    const all = filteredDemoTrees();
    const totalPages = Math.max(1, Math.ceil(all.length / TREES_PER_PAGE));
    if (state.treePage > totalPages) state.treePage = totalPages;
    const start = (state.treePage - 1) * TREES_PER_PAGE;
    const pageTrees = all.slice(start, start + TREES_PER_PAGE);
    const selected = DEMO_TREES.find(t => t.id === state.selectedTreeId) || pageTrees[0] || null;
    if (selected && state.selectedTreeId !== selected.id) state.selectedTreeId = selected.id;

    const rows = pageTrees.map(t => {
      const sel = t.id === state.selectedTreeId ? ' selected' : '';
      return `<tr class="tree-row${sel}" data-tree-id="${t.id}">
        <td><strong>${esc(t.id)}</strong></td>
        <td>R${t.rang}</td>
        <td>P${t.position}</td>
        <td>${esc(t.variete)}</td>
        <td>${statusBadge(t.statut)}</td>
        <td><code class="tree-gps">${t.lat}, ${t.lng}</code></td>
      </tr>`;
    }).join('');

    return `
      <div class="ops-why" style="margin-top:8px">
        <span class="ops-tag">Niveau arbre · démo UI</span>
        <h2>100 arbres — QR + GPS + historique</h2>
        <p>Pilote sur la parcelle <strong>A-Nord</strong> : chaque arbre a un ID, des coordonnées GPS (stockées localement, sans API payante),
        un QR qui ouvre sa fiche, et un journal d'opérations lifetime.</p>
      </div>
      <div class="ops-stats">
        <div class="ops-stat"><div class="lbl">Arbres démo</div><div class="val">100</div><div class="sub">parcelle A-Nord</div></div>
        <div class="ops-stat"><div class="lbl">Grille</div><div class="val" style="font-size:22px">10×10</div><div class="sub">rangées × positions</div></div>
        <div class="ops-stat"><div class="lbl">GPS</div><div class="val" style="font-size:18px">lat/lng</div><div class="sub">Google Maps</div></div>
        <div class="ops-stat"><div class="lbl">QR</div><div class="val" style="font-size:18px">1 / arbre</div><div class="sub">carte d'identité</div></div>
      </div>
      <div class="tree-demo-layout">
        <div class="tree-demo-list">
          <div class="tree-demo-toolbar">
            <input type="search" class="list-search tree-search" placeholder="Filtrer ID, rangée, statut…" value="${esc(state.treeQuery)}">
            <span class="tree-count">${all.length} arbre${all.length !== 1 ? 's' : ''}</span>
          </div>
          <div class="ops-table-wrap tree-table-wrap"><table class="ops-table tree-table">
            <thead><tr><th>ID arbre</th><th>Rang</th><th>Pos.</th><th>Variété</th><th>Statut</th><th>GPS</th></tr></thead>
            <tbody>${rows || '<tr><td colspan="6">Aucun arbre</td></tr>'}</tbody>
          </table></div>
          <div class="tree-pager">
            <button type="button" class="btn" data-tree-page="prev" ${state.treePage <= 1 ? 'disabled' : ''}>‹ Préc.</button>
            <span>Page <b>${state.treePage}</b> / ${totalPages}</span>
            <button type="button" class="btn" data-tree-page="next" ${state.treePage >= totalPages ? 'disabled' : ''}>Suiv. ›</button>
          </div>
        </div>
        ${renderTreeIdentityCard(selected)}
      </div>
      <p class="ops-note">Coordonnées fictives autour de Had Touabet — carte Google Maps intégrée (iframe, sans clé API pour cette démo). En production : GPS téléphone du technicien.</p>`;
  }

  function renderParcellesSketch() {
    return `
      <div class="ops-why">
        <span class="ops-tag">À quoi ça sert ?</span>
        <h2>La carte du verger</h2>
        <p>Enregistrer les <strong>25 ha SHD</strong> découpés en <strong>4 blocs variétaux</strong>, leurs <strong>parcelles</strong>,
        puis chaque <strong>arbre</strong> (QR + GPS). Tout le reste — engrais, récolte, traçabilité — se rattache à un endroit précis.</p>
      </div>
      <div class="ops-stats">
        <div class="ops-stat"><div class="lbl">Surface</div><div class="val">25</div><div class="sub">hectares SHD</div></div>
        <div class="ops-stat"><div class="lbl">Blocs</div><div class="val">4</div><div class="sub">variétés</div></div>
        <div class="ops-stat"><div class="lbl">Parcelles</div><div class="val">8</div><div class="sub">unités de suivi</div></div>
        <div class="ops-stat"><div class="lbl">Arbres démo</div><div class="val">100</div><div class="sub">A-Nord pilote</div></div>
      </div>
      <div class="ops-flow">
        <span class="ops-flow-step">Surface</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step">Bloc</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step">Parcelle</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step active">Arbre + QR</span>
      </div>
      <p class="ops-section-title">Blocs variétaux (données démo)</p>
      <div class="ops-table-wrap"><table class="ops-table">
        <thead><tr><th>Bloc</th><th>Variété</th><th>Surface</th><th>Densité</th><th>Statut</th><th>Notes</th></tr></thead>
        <tbody>
          <tr><td><strong>Bloc A</strong></td><td>Arbequina</td><td>7,2 ha</td><td>SHD</td><td><span class="badge badge-ok">Productif</span></td><td>Cœur premium ZAHO</td></tr>
          <tr><td><strong>Bloc B</strong></td><td>Arbosana</td><td>6,5 ha</td><td>SHD</td><td><span class="badge badge-ok">Productif</span></td><td>Mixte premium / standard</td></tr>
          <tr><td><strong>Bloc C</strong></td><td>Koroneiki</td><td>6,0 ha</td><td>SHD</td><td><span class="badge badge-gold">Suivi</span></td><td>Irrigation renforcée</td></tr>
          <tr><td><strong>Bloc D</strong></td><td>Picholine marocaine</td><td>5,3 ha</td><td>SHD</td><td><span class="badge badge-ok">Productif</span></td><td>Volume Chiadma d'Or</td></tr>
        </tbody>
      </table></div>
      <p class="ops-section-title">Exemple de parcelles dans le Bloc A</p>
      <div class="ops-grid-2">
        <div class="ops-card"><h3>A-Nord</h3><p>3,6 ha · Arbequina · exposition matin · <strong>100 arbres démo</strong></p><div class="ops-meta">Dernière fertilisation · 12/01/2026</div></div>
        <div class="ops-card"><h3>A-Sud</h3><p>3,6 ha · Arbequina · zone basse</p><div class="ops-meta">Dernière fertilisation · 14/01/2026</div></div>
      </div>
      ${renderTreesDemoSection()}`;
  }

  function renderRecolteSketch() {
    return `
      <div class="ops-why">
        <span class="ops-tag">À quoi ça sert ?</span>
        <h2>Récolte → tri en lots → 2 marques</h2>
        <p>Suivre les olives récoltées et les <strong>répartir</strong> : lot premium → <strong>ZAHO</strong> (B2B/export),
        lot standard → <strong>Chiadma d'Or</strong> (épiciers). Première récolte significative : <strong>sept. 2026</strong>.</p>
      </div>
      <div class="ops-flow">
        <span class="ops-flow-step">Parcelle</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step active">Récolte</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step">Tri lot</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step">Marque</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step">Embouteillage</span>
      </div>
      <div class="ops-stats">
        <div class="ops-stat"><div class="lbl">Cible premium</div><div class="val">15%</div><div class="sub">rendement · ZAHO</div></div>
        <div class="ops-stat"><div class="lbl">Cible standard</div><div class="val">22%</div><div class="sub">rendement · Chiadma</div></div>
        <div class="ops-stat"><div class="lbl">Vol. premium</div><div class="val">~30%</div><div class="sub">du volume</div></div>
        <div class="ops-stat"><div class="lbl">Vol. standard</div><div class="val">~70%</div><div class="sub">du volume</div></div>
      </div>
      <p class="ops-section-title">Lots démo (campagne 2026)</p>
      <div class="ops-table-wrap"><table class="ops-table">
        <thead><tr><th>N° lot</th><th>Date</th><th>Bloc / parcelle</th><th>Kg olives</th><th>Rendement</th><th>Marque</th><th>Statut</th></tr></thead>
        <tbody>
          <tr><td><strong>LOT-26-P01</strong></td><td>15/09/26</td><td>Bloc A · A-Nord</td><td>2 400</td><td>15,2 %</td><td><span class="badge badge-gold">ZAHO</span></td><td>Premium</td></tr>
          <tr><td><strong>LOT-26-S01</strong></td><td>16/09/26</td><td>Bloc D · D-Est</td><td>5 100</td><td>21,8 %</td><td><span class="badge badge-ok">Chiadma d'Or</span></td><td>Standard</td></tr>
          <tr><td><strong>LOT-26-P02</strong></td><td>18/09/26</td><td>Bloc B · B-Ouest</td><td>1 850</td><td>14,9 %</td><td><span class="badge badge-gold">ZAHO</span></td><td>Premium</td></tr>
        </tbody>
      </table></div>
      <p class="ops-note">Erreur d'allocation = mélange de marques. Ce module est critique dès la récolte 2026.</p>`;
  }

  function renderIntrantsSketch() {
    return `
      <div class="ops-why">
        <span class="ops-tag">À quoi ça sert ?</span>
        <h2>Engrais & intrants par parcelle</h2>
        <p>Remplacer les classeurs Excel (FR/AR) : saisir <strong>phosphore, potassium, bore</strong> (et autres),
        quantités, coûts, et le <strong>lien au bloc/parcelle</strong> pour le gérant terrain.</p>
      </div>
      <div class="ops-stats">
        <div class="ops-stat"><div class="lbl">Coût mois</div><div class="val" style="font-size:22px">12 450</div><div class="sub">MAD · janv. 2026</div></div>
        <div class="ops-stat"><div class="lbl">Applications</div><div class="val">7</div><div class="sub">ce mois</div></div>
        <div class="ops-stat"><div class="lbl">Intrants clés</div><div class="val" style="font-size:18px">P · K · B</div><div class="sub">phosphore, potasse, bore</div></div>
      </div>
      <p class="ops-section-title">Journal d'applications (démo)</p>
      <div class="ops-table-wrap"><table class="ops-table">
        <thead><tr><th>Date</th><th>Intrant</th><th>Parcelle</th><th>Quantité</th><th>Coût</th><th>Saisi par</th></tr></thead>
        <tbody>
          <tr><td>12/01/26</td><td>NPK 15-15-15</td><td>A-Nord</td><td>8 sacs</td><td>3 600 MAD</td><td>Gérant</td></tr>
          <tr><td>12/01/26</td><td>Bore</td><td>A-Nord</td><td>25 kg</td><td>890 MAD</td><td>Gérant</td></tr>
          <tr><td>14/01/26</td><td>NPK 15-15-15</td><td>A-Sud</td><td>8 sacs</td><td>3 600 MAD</td><td>Gérant</td></tr>
          <tr><td>16/01/26</td><td>Potasse (K)</td><td>Bloc C</td><td>4 sacs</td><td>2 160 MAD</td><td>Gérant</td></tr>
          <tr><td>20/01/26</td><td>Phosphore (P)</td><td>Bloc D</td><td>5 sacs</td><td>2 200 MAD</td><td>Gérant</td></tr>
        </tbody>
      </table></div>
      <div class="ops-grid-2">
        <div class="ops-card"><h3>Lien parcelle</h3><p>Chaque application est rattachée à un bloc/parcelle — historique agronomique clair.</p></div>
        <div class="ops-card"><h3>Lien achat</h3><p>Les sacs NPK viennent des bons / factures fournisseur (module Achat).</p></div>
      </div>`;
  }

  function renderTracabiliteSketch() {
    return `
      <div class="ops-why">
        <span class="ops-tag">À quoi ça sert ?</span>
        <h2>QR-code Tier 1 — promesse marque ZAHO</h2>
        <p>Chaque lot (puis bouteille) premium porte un QR qui remonte à : <strong>parcelle, date de récolte, n° de lot, année</strong>.
        C'est ce que le site vitrine promet aux clients B2B.</p>
      </div>
      <div class="ops-flow">
        <span class="ops-flow-step">Lot récolte</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step">N° traçabilité</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step active">QR Tier 1</span><span class="ops-flow-arrow">→</span>
        <span class="ops-flow-step">Étiquette bouteille</span>
      </div>
      <p class="ops-section-title">Fiche lot scannable (démo)</p>
      <div class="ops-card" style="margin-bottom:16px">
        <div class="ops-lot-row">
          <div class="ops-qr-mock" title="Aperçu QR"></div>
          <div class="ops-lot-info">
            <h3 style="font-family:'Fraunces',serif;font-size:20px;margin:0 0 8px;color:var(--pine)">ZAHO · LOT-26-P01</h3>
            <p style="margin:0 0 6px;font-size:14px"><strong>Parcelle</strong> Bloc A · A-Nord · Arbequina</p>
            <p style="margin:0 0 6px;font-size:14px"><strong>Récolte</strong> 15/09/2026 · rendement 15,2 %</p>
            <p style="margin:0 0 6px;font-size:14px"><strong>Marque</strong> ZAHO Extra Vierge · millésime 2026</p>
            <p style="margin:0;font-size:12px;color:var(--muted)">URL publique type : zaho.ma/lot/LOT-26-P01</p>
          </div>
        </div>
      </div>
      <div class="ops-table-wrap"><table class="ops-table">
        <thead><tr><th>Code QR</th><th>Lot</th><th>Marque</th><th>Parcelle</th><th>Statut</th></tr></thead>
        <tbody>
          <tr><td>QR-26-0042</td><td>LOT-26-P01</td><td>ZAHO</td><td>A-Nord</td><td><span class="badge badge-ok">Actif</span></td></tr>
          <tr><td>QR-26-0043</td><td>LOT-26-P02</td><td>ZAHO</td><td>B-Ouest</td><td><span class="badge badge-gold">À imprimer</span></td></tr>
          <tr><td>—</td><td>LOT-26-S01</td><td>Chiadma d'Or</td><td>D-Est</td><td><span class="badge badge-warn">Hors scope Tier 1</span></td></tr>
        </tbody>
      </table></div>
      <p class="ops-note">Tier 1 = identification lot/origine. AgriEdge / IoT viendraient enrichir plus tard (phase 3).</p>`;
  }

  function renderPrixMarcheSketch() {
    return `
      <div class="ops-why">
        <span class="ops-tag">À quoi ça sert ?</span>
        <h2>Prix courants — garde-fou financier</h2>
        <p>Un incident passé : des <strong>prix obsolètes</strong> ont faussé une modélisation.
        Ce module force la saisie / vérification des <strong>prix marché à jour</strong> avant tout calcul de marge ou rentabilité.</p>
      </div>
      <div class="ops-alert"><strong>Règle métier :</strong> alerte si un prix n'a pas été mis à jour depuis plus de 30 jours — bloquer ou avertir avant calcul.</div>
      <div class="ops-stats">
        <div class="ops-stat"><div class="lbl">Huile vrac</div><div class="val" style="font-size:22px">48</div><div class="sub">MAD / L · MA</div></div>
        <div class="ops-stat"><div class="lbl">Export EU</div><div class="val" style="font-size:22px">6,2</div><div class="sub">€ / L · B2B</div></div>
        <div class="ops-stat"><div class="lbl">Dernière maj</div><div class="val" style="font-size:18px">02/09</div><div class="sub">2026</div></div>
        <div class="ops-stat"><div class="lbl">Alertes</div><div class="val">1</div><div class="sub">prix &gt; 30 j</div></div>
      </div>
      <p class="ops-section-title">Référentiel prix (démo)</p>
      <div class="ops-table-wrap"><table class="ops-table">
        <thead><tr><th>Produit / marché</th><th>Prix</th><th>Unité</th><th>Source</th><th>Maj</th><th>État</th></tr></thead>
        <tbody>
          <tr><td>Huile olive vrac — marché local</td><td><strong>48,00</strong></td><td>MAD/L</td><td>Souk Essaouira</td><td>02/09/26</td><td><span class="badge badge-ok">À jour</span></td></tr>
          <tr><td>ZAHO 500ml — prix cession B2B</td><td><strong>200,00</strong></td><td>MAD/btl</td><td>Grille interne</td><td>01/09/26</td><td><span class="badge badge-ok">À jour</span></td></tr>
          <tr><td>Export EU — vrac premium</td><td><strong>6,20</strong></td><td>€/L</td><td>Importateur Oliva</td><td>28/08/26</td><td><span class="badge badge-ok">À jour</span></td></tr>
          <tr><td>Chiadma d'Or 1L — épicier</td><td><strong>95,00</strong></td><td>MAD/btl</td><td>Grille interne</td><td>12/07/26</td><td><span class="badge badge-warn">Obsolète</span></td></tr>
        </tbody>
      </table></div>
      <p class="ops-note">Utilisé avant devis / facturation / simulations de rentabilité — pas un simple carnet de notes.</p>`;
  }

  function renderOpsSketch(module) {
    const meta = MODULE_META[module];
    const bodies = {
      parcelles: renderParcellesSketch,
      'recolte-lots': renderRecolteSketch,
      intrants: renderIntrantsSketch,
      tracabilite: renderTracabiliteSketch,
      'prix-marche': renderPrixMarcheSketch,
    };
    const fn = bodies[module];
    if (!fn) {
      return `<div class="page-head"><p class="eyebrow">${meta.section}</p><h1>${meta.title}</h1><p class="module-section">Module en construction.</p></div>`;
    }
    return sketchShell(meta, fn());
  }

  function renderList(module) {
    if (OPS_MODULES.includes(module)) return renderOpsSketch(module);

    const cfg = getListConfig(module);
    if (!cfg) {
      return `<div class="page-head"><p class="eyebrow">${MODULE_META[module].section}</p><h1>${MODULE_META[module].title}</h1><p class="module-section">Module en construction.</p></div>`;
    }

    const { meta, rows, headers, cols } = cfg;
    const n = rows.length;

    const toolbar = `
      <button type="button" class="btn btn-primary" data-action="new">Nouveau</button>
      <input type="search" class="list-search" placeholder="Rechercher…" value="${esc(state.search)}">
    `;

    let body;
    if (!n) {
      body = '<div class="list-empty">Aucun enregistrement — cliquez sur Nouveau pour commencer.</div>';
    } else {
      body = rows.map((r, i) => {
        const cells = r.cells.map(c => `<span>${c}</span>`).join('');
        const sel = r.id === state.selectedId ? ' selected' : '';
        return `<div class="list-row${sel}" data-id="${r.id}" style="grid-template-columns:${cols}">${cells}</div>`;
      }).join('');
    }

    const headerCells = headers.map(h => `<span>${h}</span>`).join('');

    return `
      <div class="page-head">
        <p class="eyebrow">${meta.section}</p>
        <h1>${meta.title}</h1>
        <p class="module-section">${meta.subtitle}</p>
      </div>
      <div class="list-toolbar">${toolbar}</div>
      <div class="list-wrap">
        <div class="list-header" style="grid-template-columns:${cols}">${headerCells}</div>
        <div class="list-body">${body}</div>
      </div>
      <div class="list-pagination">
        <div class="list-pagination-inner">
          <span class="total">${n} enregistrement${n !== 1 ? 's' : ''}</span>
          <div class="pagination-nav">
            <button type="button" disabled>⏮</button>
            <button type="button" disabled>‹</button>
            <span class="page-indicator"><b>1</b> / 1</span>
            <button type="button" disabled>›</button>
            <button type="button" disabled>⏭</button>
          </div>
        </div>
      </div>`;
  }

  function render() {
    const main = document.getElementById('dashMain');
    if (!main) return;
    main.innerHTML = state.view === 'form' ? renderForm(state.module) : renderList(state.module);
    bindMainEvents();
  }

  function readLinesFromForm(form) {
    const rows = form.querySelectorAll('#docLinesBody tr');
    return Array.from(rows).map(row => {
      const get = name => row.querySelector(`[name="${name}"]`)?.value;
      const idx = row.dataset.line;
      return {
        reference: get(`line_ref_${idx}`)?.trim() || '',
        designation: get(`line_des_${idx}`)?.trim() || '',
        quantite: parseFloat(get(`line_qty_${idx}`)) || 0,
        unite: get(`line_unit_${idx}`)?.trim() || 'u',
        prixUnitaireHt: parseFloat(get(`line_pu_${idx}`)) || 0,
        remise: parseFloat(get(`line_rem_${idx}`)) || 0,
        tauxTva: parseFloat(get(`line_tva_${idx}`)) || 0,
      };
    }).filter(l => l.designation || l.reference || l.prixUnitaireHt);
  }

  function refreshLineTotals(form) {
    const rows = form.querySelectorAll('#docLinesBody tr');
    const lines = readLinesFromForm(form);
    rows.forEach((row, i) => {
      const line = lines[i] || ZahoDocLines.emptyLine();
      const c = ZahoDocLines.calcLine(line);
      const htCell = row.querySelector(`[data-line-ht="${row.dataset.line}"]`);
      const ttcCell = row.querySelector(`[data-line-ttc="${row.dataset.line}"]`);
      if (htCell) htCell.textContent = ZahoDocLines.fmt(c.ht);
      if (ttcCell) ttcCell.textContent = ZahoDocLines.fmt(c.ttc);
    });
    const totals = ZahoDocLines.calcTotals(lines.length ? lines : [ZahoDocLines.emptyLine()]);
    const htEl = form.querySelector('#totalHt');
    const tvaEl = form.querySelector('#totalTva');
    const ttcEl = form.querySelector('#totalTtc');
    if (htEl) htEl.textContent = `${ZahoDocLines.fmt(totals.ht)} MAD`;
    if (tvaEl) tvaEl.textContent = `${ZahoDocLines.fmt(totals.tva)} MAD`;
    if (ttcEl) ttcEl.textContent = `${ZahoDocLines.fmt(totals.ttc)} MAD`;
    return totals;
  }

  function reindexLineRows(tbody) {
    Array.from(tbody.querySelectorAll('tr')).forEach((row, i) => {
      row.dataset.line = i;
      row.querySelectorAll('[data-line-field]').forEach(inp => {
        const field = inp.dataset.lineField;
        inp.name = `line_${field === 'reference' ? 'ref' : field === 'designation' ? 'des' : field === 'quantite' ? 'qty' : field === 'unite' ? 'unit' : field === 'prixUnitaireHt' ? 'pu' : field === 'remise' ? 'rem' : 'tva'}_${i}`;
      });
      const ht = row.querySelector('[data-line-ht]');
      const ttc = row.querySelector('[data-line-ttc]');
      if (ht) { ht.dataset.lineHt = i; }
      if (ttc) { ttc.dataset.lineTtc = i; }
    });
  }

  function lineRowHtml(i, line) {
    const c = ZahoDocLines.calcLine(line);
    return `
        <td class="col-ref"><input type="text" name="line_ref_${i}" value="${esc(line.reference || '')}" data-line-field="reference"></td>
        <td class="col-designation"><input type="text" name="line_des_${i}" value="${esc(line.designation || '')}" data-line-field="designation"></td>
        <td class="col-qty"><input type="number" step="any" min="0" name="line_qty_${i}" value="${line.quantite ?? 1}" data-line-field="quantite"></td>
        <td class="col-unite"><input type="text" name="line_unit_${i}" value="${esc(line.unite || 'u')}" data-line-field="unite"></td>
        <td class="col-pu"><input type="number" step="0.01" min="0" name="line_pu_${i}" value="${line.prixUnitaireHt ?? 0}" data-line-field="prixUnitaireHt"></td>
        <td class="col-rem"><input type="number" step="0.01" min="0" max="100" name="line_rem_${i}" value="${line.remise ?? 0}" data-line-field="remise"></td>
        <td class="col-tva"><input type="number" step="0.01" min="0" name="line_tva_${i}" value="${line.tauxTva ?? 20}" data-line-field="tauxTva"></td>
        <td class="col-ht" data-line-ht="${i}">${ZahoDocLines.fmt(c.ht)}</td>
        <td class="col-ttc" data-line-ttc="${i}">${ZahoDocLines.fmt(c.ttc)}</td>
        <td class="col-actions"><button type="button" class="btn-line-remove" data-action="remove-line" title="Supprimer">×</button></td>`;
  }

  function appendLineRow(form, line) {
    const tbody = form.querySelector('#docLinesBody');
    const i = tbody.querySelectorAll('tr').length;
    const tr = document.createElement('tr');
    tr.dataset.line = i;
    tr.innerHTML = lineRowHtml(i, line);
    tbody.appendChild(tr);
    tr.querySelector('[data-action="remove-line"]').addEventListener('click', () => {
      if (tbody.querySelectorAll('tr').length <= 1) return;
      tr.remove();
      reindexLineRows(tbody);
      refreshLineTotals(form);
    });
    refreshLineTotals(form);
    return tr;
  }

  function bindDocumentLineEvents(form) {
    if (!form?.dataset.docForm) return;

    form.addEventListener('input', e => {
      if (e.target.closest('#docLinesBody')) refreshLineTotals(form);
    });

    form.querySelector('[data-action="add-line"]')?.addEventListener('click', () => {
      const type = form.dataset.productType || 'vente';
      const count = form.querySelectorAll('#docLinesBody tr').length;
      appendLineRow(form, ZahoDocLines.demoLine(type, count));
    });

    form.querySelector('[data-action="pick-product"]')?.addEventListener('change', e => {
      const ref = e.target.value;
      if (!ref) return;
      const module = form.dataset.module;
      const type = DOC_CLIENT_MODULES.includes(module) ? 'vente' : 'achat';
      const product = ZahoDocLines.getDemoProducts(type).find(p => p.reference === ref);
      if (product) appendLineRow(form, ZahoDocLines.productToLine(product));
      e.target.value = '';
    });

    form.querySelectorAll('[data-action="remove-line"]').forEach(btn => {
      btn.addEventListener('click', () => {
        const tbody = form.querySelector('#docLinesBody');
        if (tbody.querySelectorAll('tr').length <= 1) return;
        btn.closest('tr')?.remove();
        reindexLineRows(tbody);
        refreshLineTotals(form);
      });
    });
  }

  function buildDocPayload(module, data, lines, totals) {
    const payload = {
      date: data.date,
      note: data.note?.trim() || '',
      lines,
      ht: totals.ht,
      tva: totals.tva,
      ttc: totals.ttc,
    };
    if (DOC_CLIENT_MODULES.includes(module)) {
      payload.clientId = data.clientId;
      payload.clientNom = ZahoStore.resolveClientName(data.clientId);
      if (module === 'devis') payload.validite = data.validite;
      if (module === 'facturation') { payload.echeance = data.echeance; payload.payee = !!data.payee; }
      if (module === 'bons-livraison') payload.facture = !!data.facture;
    } else {
      payload.fournisseurId = data.fournisseurId;
      payload.fournisseurNom = ZahoStore.resolveFournisseurName(data.fournisseurId);
    }
    return payload;
  }

  function parseForm(form) {
    const fd = new FormData(form);
    const o = {};
    for (const [k, v] of fd.entries()) o[k] = v;
    const checkboxes = form.querySelectorAll('input[type=checkbox]');
    checkboxes.forEach(cb => { o[cb.name] = cb.checked; });
    if (o.ttc !== undefined) o.ttc = parseFloat(o.ttc) || 0;
    return o;
  }

  function saveForm(module, form) {
    const meta = MODULE_META[module];
    const collection = meta.collection;
    const data = parseForm(form);

    if (TIERS_MODULES.includes(module)) {
      const payload = {
        nom: data.nom?.trim(),
        ice: data.ice?.trim() || '',
        adresse: data.adresse?.trim() || '',
        ville: data.ville?.trim() || '',
        telephone: data.telephone?.trim() || '',
        email: data.email?.trim() || '',
        actif: !!data.actif,
      };
      if (!payload.nom) { alert('Le nom est obligatoire.'); return; }
      if (state.editId) ZahoStore.update(collection, state.editId, payload);
      else ZahoStore.create(collection, payload);
    } else if (DOC_CLIENT_MODULES.includes(module)) {
      if (!data.clientId) { alert('Choisissez un client.'); return; }
      const form = document.getElementById('crudForm');
      const lines = isDocWithLines(module) ? readLinesFromForm(form) : [];
      if (isDocWithLines(module) && !lines.length) { alert('Ajoutez au moins une ligne.'); return; }
      const totals = isDocWithLines(module) ? ZahoDocLines.calcTotals(lines) : { ht: 0, tva: 0, ttc: data.ttc || 0 };
      const payload = buildDocPayload(module, data, lines, totals);
      if (state.editId) ZahoStore.update(collection, state.editId, payload);
      else {
        payload.numero = ZahoStore.nextNumero(module);
        ZahoStore.create(collection, payload);
      }
    } else if (DOC_FOURNISSEUR_MODULES.includes(module)) {
      if (module === 'charges') {
        const payload = {
          type: data.type?.trim(),
          date: data.date,
          libelle: data.libelle?.trim(),
          beneficiaire: data.beneficiaire?.trim() || '',
          ttc: data.ttc,
          note: data.note?.trim() || '',
        };
        if (!payload.libelle) { alert('Le libellé est obligatoire.'); return; }
        if (state.editId) ZahoStore.update(collection, state.editId, payload);
        else ZahoStore.create(collection, payload);
      } else {
        if (!data.fournisseurId) { alert('Choisissez un fournisseur.'); return; }
        const form = document.getElementById('crudForm');
        const lines = readLinesFromForm(form);
        if (!lines.length) { alert('Ajoutez au moins une ligne.'); return; }
        const totals = ZahoDocLines.calcTotals(lines);
        const payload = buildDocPayload(module, data, lines, totals);
        if (state.editId) ZahoStore.update(collection, state.editId, payload);
        else {
          payload.numero = ZahoStore.nextNumero(module);
          ZahoStore.create(collection, payload);
        }
      }
    }

    state.view = 'list';
    state.editId = null;
    render();
  }

  function bindMainEvents() {
    const main = document.getElementById('dashMain');

    main.querySelector('[data-action="new"]')?.addEventListener('click', () => {
      state.view = 'form';
      state.editId = null;
      render();
    });

    main.querySelector('[data-action="back"]')?.addEventListener('click', () => {
      state.view = 'list';
      state.editId = null;
      render();
    });

    main.querySelector('[data-action="delete"]')?.addEventListener('click', () => {
      if (!state.editId) return;
      const meta = MODULE_META[state.module];
      if (confirm('Supprimer cet enregistrement ?')) {
        ZahoStore.remove(meta.collection, state.editId);
        state.view = 'list';
        state.editId = null;
        state.selectedId = null;
        render();
      }
    });

    main.querySelector('#crudForm')?.addEventListener('submit', e => {
      e.preventDefault();
      saveForm(state.module, e.target);
    });

    bindDocumentLineEvents(main.querySelector('#crudForm'));

    main.querySelector('.list-search')?.addEventListener('input', e => {
      state.search = e.target.value;
      render();
    });

    main.querySelectorAll('.list-row').forEach(row => {
      row.addEventListener('click', () => {
        state.selectedId = row.dataset.id;
        state.view = 'form';
        state.editId = row.dataset.id;
        render();
      });
    });

    bindTreeDemoEvents(main);
  }

  function bindTreeDemoEvents(main) {
    if (state.module !== 'parcelles') return;

    main.querySelectorAll('.tree-row').forEach(row => {
      row.addEventListener('click', () => {
        state.selectedTreeId = row.dataset.treeId;
        render();
        document.getElementById('treeIdCard')?.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
      });
    });

    main.querySelector('.tree-search')?.addEventListener('input', e => {
      const val = e.target.value;
      const pos = e.target.selectionStart;
      state.treeQuery = val;
      state.treePage = 1;
      render();
      const input = document.querySelector('.tree-search');
      if (input) {
        input.focus();
        try { input.setSelectionRange(pos, pos); } catch (_) { /* ignore */ }
      }
    });

    main.querySelector('[data-tree-page="prev"]')?.addEventListener('click', () => {
      if (state.treePage > 1) { state.treePage -= 1; render(); }
    });

    main.querySelector('[data-tree-page="next"]')?.addEventListener('click', () => {
      state.treePage += 1;
      render();
    });
  }

  function initSidebar() {
    document.querySelectorAll('.dash-section-toggle').forEach(btn => {
      btn.addEventListener('click', () => {
        const section = btn.closest('.dash-section');
        const isOpen = section.classList.toggle('open');
        btn.setAttribute('aria-expanded', isOpen);
        btn.querySelector('.arrow').textContent = isOpen ? '▼' : '▶';
      });
    });

    document.querySelectorAll('.dash-item').forEach(item => {
      item.addEventListener('click', () => {
        document.querySelectorAll('.dash-item').forEach(i => i.classList.remove('active'));
        item.classList.add('active');
        state.module = item.dataset.module;
        state.view = 'list';
        state.editId = null;
        state.selectedId = null;
        state.search = '';
        state.treePage = 1;
        state.selectedTreeId = null;
        state.treeQuery = '';
        render();
      });
    });
  }

  function init() {
    ZahoStore.init();
    initSidebar();
    render();
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();
