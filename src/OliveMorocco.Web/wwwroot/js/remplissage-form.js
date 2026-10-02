(function () {
    const form = document.getElementById('remplissage-form');
    if (!form) {
        return;
    }

    const varieteSelect = document.getElementById('remplissage-variete');
    const perteInput = document.getElementById('remplissage-perte');
    const lignesBody = document.getElementById('remplissage-lignes-body');
    const addBtn = document.getElementById('add-remplissage-line');
    const emptyMsg = document.getElementById('remplissage-lines-empty');
    const template = document.getElementById('remplissage-line-template');

    const summaryDisponible = document.getElementById('summary-disponible');
    const summaryLitres = document.getElementById('summary-litres');
    const summaryTotal = document.getElementById('summary-total');
    const summaryReste = document.getElementById('summary-reste');
    const summaryWarning = document.getElementById('summary-warning');

    const varieteInitiale = form.dataset.varieteInitiale || '';
    const huileInitiale = parseNumber(form.dataset.huileInitiale);

    function parseNumber(value) {
        const parsed = Number.parseFloat(String(value ?? '').replace(',', '.'));
        return Number.isFinite(parsed) ? parsed : 0;
    }

    function formatLitres(value) {
        return value.toLocaleString('fr-FR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
        }) + ' L';
    }

    function formatContenance(value) {
        return value.toLocaleString('fr-FR', { maximumFractionDigits: 3 }) + ' L';
    }

    function currentVarieteId() {
        return varieteSelect ? varieteSelect.value : '';
    }

    function stockDisponible() {
        if (!varieteSelect || !varieteSelect.value) {
            return null;
        }

        const option = varieteSelect.options[varieteSelect.selectedIndex];
        let stock = parseNumber(option?.dataset?.stock);
        if (varieteInitiale && varieteSelect.value === varieteInitiale) {
            stock += huileInitiale;
        }

        return stock;
    }

    function reindex() {
        lignesBody.querySelectorAll('tr').forEach((row, index) => {
            row.querySelectorAll('[name]').forEach((input) => {
                input.name = input.name.replace(/Lignes\[\d+\]/, `Lignes[${index}]`);
            });
        });

        if (emptyMsg) {
            emptyMsg.classList.toggle('hidden', lignesBody.querySelectorAll('.remplissage-line').length > 0);
        }
    }

    function filterProduits(row) {
        const select = row.querySelector('.line-produit-id');
        if (!select) {
            return;
        }

        const varieteId = currentVarieteId();
        Array.from(select.options).forEach((option) => {
            if (!option.value) {
                return;
            }

            const visible = varieteId !== '' && option.dataset.variete === varieteId;
            option.hidden = !visible;
            option.disabled = !visible;
        });

        const selected = select.options[select.selectedIndex];
        if (selected && selected.value && selected.disabled) {
            select.value = '';
        }
    }

    function syncRow(row) {
        const select = row.querySelector('.line-produit-id');
        const qtyInput = row.querySelector('.line-quantite');
        const uniteHint = row.querySelector('.line-unite-hint');
        const contenanceCell = row.querySelector('.line-contenance');
        const litresCell = row.querySelector('.line-litres');

        const selected = select?.options[select.selectedIndex];
        const hasProduit = !!(select && select.value);
        const contenance = hasProduit ? parseNumber(selected.dataset.contenance) : 0;

        if (qtyInput) {
            qtyInput.disabled = !hasProduit;
            if (!hasProduit) {
                qtyInput.value = '';
            }
        }

        if (uniteHint) {
            uniteHint.textContent = hasProduit ? (selected.dataset.unite || '') : '';
        }

        if (contenanceCell) {
            contenanceCell.textContent = hasProduit ? formatContenance(contenance) : '—';
        }

        const litres = hasProduit ? Math.round(parseNumber(qtyInput?.value) * contenance * 10000) / 10000 : 0;
        if (litresCell) {
            litresCell.textContent = hasProduit ? formatLitres(litres) : '—';
        }

        return litres;
    }

    function updateSummary() {
        let litres = 0;
        lignesBody.querySelectorAll('.remplissage-line').forEach((row) => {
            litres += syncRow(row);
        });

        const perte = Math.max(0, parseNumber(perteInput?.value));
        const total = litres + perte;
        const disponible = stockDisponible();

        summaryLitres.textContent = formatLitres(litres);
        summaryTotal.textContent = formatLitres(total);

        if (disponible === null) {
            summaryDisponible.textContent = '—';
            summaryReste.textContent = '—';
            summaryWarning.classList.add('hidden');
            return;
        }

        const reste = disponible - total;
        summaryDisponible.textContent = formatLitres(disponible);
        summaryReste.textContent = formatLitres(reste);
        summaryReste.classList.toggle('is-negative', reste < 0);
        summaryWarning.classList.toggle('hidden', reste >= 0);
    }

    function bindRow(row) {
        const select = row.querySelector('.line-produit-id');
        const qtyInput = row.querySelector('.line-quantite');
        const removeBtn = row.querySelector('.remove-remplissage-line');

        select?.addEventListener('change', updateSummary);
        qtyInput?.addEventListener('input', updateSummary);
        removeBtn?.addEventListener('click', () => {
            row.remove();
            reindex();
            updateSummary();
        });

        bindCollapseToggle(row);
        filterProduits(row);
    }

    // Same folding as the intervention lines: a filled row collapses to its product select
    // once a new, empty row is opened, so a Remplissage of several products is a list of
    // summaries rather than a stack of identical forms.
    function bindCollapseToggle(row) {
        const toggle = row.querySelector('.row-collapse-toggle');
        if (!toggle) {
            return;
        }

        toggle.addEventListener('click', () => {
            const collapsed = row.classList.toggle('is-collapsed');
            toggle.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
        });
    }

    function collapseRow(row) {
        const toggle = row.querySelector('.row-collapse-toggle');
        row.classList.add('is-collapsed');
        if (toggle) {
            toggle.setAttribute('aria-expanded', 'false');
        }
    }

    addBtn?.addEventListener('click', () => {
        const index = lignesBody.querySelectorAll('.remplissage-line').length;
        const wrapper = document.createElement('tbody');
        wrapper.innerHTML = template.innerHTML.replace(/__index__/g, String(index)).trim();
        const row = wrapper.firstElementChild;
        lignesBody.appendChild(row);

        // Same behavior as the other two line tables: the recorded rows fold away when a
        // fresh one is opened, so the next product is always the one at eye level.
        lignesBody.querySelectorAll('.remplissage-line').forEach((r) => {
            if (r !== row) collapseRow(r);
        });

        bindRow(row);
        reindex();
        updateSummary();
    });

    varieteSelect?.addEventListener('change', () => {
        lignesBody.querySelectorAll('.remplissage-line').forEach(filterProduits);
        updateSummary();
    });

    perteInput?.addEventListener('input', updateSummary);

    lignesBody.querySelectorAll('.remplissage-line').forEach(bindRow);
    reindex();
    updateSummary();
})();
