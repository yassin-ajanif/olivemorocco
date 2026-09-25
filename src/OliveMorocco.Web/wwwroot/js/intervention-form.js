(function () {
    const form = document.getElementById('intervention-form');
    const lignesBody = document.getElementById('intervention-lignes-body');
    const addIntrantBtn = document.getElementById('add-intrant-line');
    const lignesEmpty = document.getElementById('intervention-lines-empty');
    const ligneTemplate = document.getElementById('intervention-line-template');

    const chargesBody = document.getElementById('intervention-charges-body');
    const addChargeBtn = document.getElementById('add-charge-line');
    const chargesEmpty = document.getElementById('intervention-charges-empty');
    const chargeTemplate = document.getElementById('intervention-charge-template');
    const chargesTotal = document.getElementById('charges-total');

    const debitInput = document.getElementById('debit-eau');
    const dureeInput = document.getElementById('duree-eau');
    const quantiteEauInput = document.getElementById('quantite-eau');

    if (!form) {
        return;
    }

    function reindexRows(tbody, prefix) {
        const rows = tbody.querySelectorAll('tr');
        rows.forEach((row, index) => {
            row.querySelectorAll('[name]').forEach((input) => {
                input.name = input.name.replace(new RegExp(`${prefix}\\[\\d+\\]`), `${prefix}[${index}]`);
            });
        });
    }

    function reindexLignes() {
        if (!lignesBody) {
            return;
        }

        reindexRows(lignesBody, 'Lignes');
        if (lignesEmpty) {
            const count = lignesBody.querySelectorAll('.intervention-line').length;
            lignesEmpty.classList.toggle('hidden', count > 0);
        }
    }

    function reindexCharges() {
        if (!chargesBody) {
            return;
        }

        reindexRows(chargesBody, 'Charges');
        if (chargesEmpty) {
            const count = chargesBody.querySelectorAll('.intervention-charge-line').length;
            chargesEmpty.classList.toggle('hidden', count > 0);
        }

        updateChargesTotal();
    }

    function syncLineUnite(row) {
        const select = row.querySelector('.line-intrant-id');
        const qtyInput = row.querySelector('.line-quantite');
        const hint = row.querySelector('.line-unite-hint');
        if (!select || !qtyInput) {
            return;
        }

        const selected = select.options[select.selectedIndex];
        const hasIntrant = select.value !== '';

        qtyInput.disabled = !hasIntrant;
        if (!hasIntrant) {
            qtyInput.value = '';
        }

        if (hint) {
            const unite = selected?.dataset?.unite;
            hint.textContent = hasIntrant && unite ? unite : '';
        }
    }

    function parseNumber(value) {
        const parsed = Number.parseFloat(value);
        return Number.isFinite(parsed) ? parsed : 0;
    }

    function formatMoney(value) {
        return value.toLocaleString('fr-FR', {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
        }) + ' DH';
    }

    function updateChargesTotal() {
        if (!chargesBody || !chargesTotal) {
            return;
        }

        let total = 0;
        chargesBody.querySelectorAll('.line-charge-montant').forEach((input) => {
            total += parseNumber(input.value);
        });

        chargesTotal.textContent = formatMoney(total);
    }

    function updateQuantiteEauFromCalc() {
        if (!debitInput || !dureeInput || !quantiteEauInput) {
            return;
        }

        const debit = parseNumber(debitInput.value);
        const duree = parseNumber(dureeInput.value);

        if (debit > 0 && duree > 0) {
            const result = Math.round(debit * duree * 10) / 10;
            quantiteEauInput.value = String(result);
        }
    }

    function bindIntrantRow(row) {
        const select = row.querySelector('.line-intrant-id');
        const removeBtn = row.querySelector('.remove-intrant-line');

        if (select) {
            select.addEventListener('change', () => syncLineUnite(row));
        }

        if (removeBtn) {
            removeBtn.addEventListener('click', () => {
                row.remove();
                reindexLignes();
            });
        }

        syncLineUnite(row);
    }

    function bindChargeRow(row) {
        const removeBtn = row.querySelector('.remove-charge-line');
        const montantInput = row.querySelector('.line-charge-montant');

        if (removeBtn) {
            removeBtn.addEventListener('click', () => {
                row.remove();
                reindexCharges();
            });
        }

        if (montantInput) {
            montantInput.addEventListener('input', updateChargesTotal);
        }
    }

    if (addIntrantBtn && lignesBody && ligneTemplate) {
        addIntrantBtn.addEventListener('click', () => {
            const index = lignesBody.querySelectorAll('.intervention-line').length;
            const html = ligneTemplate.innerHTML.replace(/__index__/g, String(index));
            const wrapper = document.createElement('tbody');
            wrapper.innerHTML = html.trim();
            const row = wrapper.firstElementChild;
            lignesBody.appendChild(row);
            bindIntrantRow(row);
            reindexLignes();
        });

        lignesBody.querySelectorAll('.intervention-line').forEach(bindIntrantRow);
        reindexLignes();
    }

    if (addChargeBtn && chargesBody && chargeTemplate) {
        addChargeBtn.addEventListener('click', () => {
            const index = chargesBody.querySelectorAll('.intervention-charge-line').length;
            const html = chargeTemplate.innerHTML.replace(/__index__/g, String(index));
            const wrapper = document.createElement('tbody');
            wrapper.innerHTML = html.trim();
            const row = wrapper.firstElementChild;
            chargesBody.appendChild(row);
            bindChargeRow(row);
            reindexCharges();
        });

        chargesBody.querySelectorAll('.intervention-charge-line').forEach(bindChargeRow);
        reindexCharges();
    }

    if (debitInput && dureeInput) {
        debitInput.addEventListener('input', updateQuantiteEauFromCalc);
        dureeInput.addEventListener('input', updateQuantiteEauFromCalc);
    }
})();
