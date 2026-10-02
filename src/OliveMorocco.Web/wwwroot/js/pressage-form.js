(function () {
    const form = document.querySelector('[data-pressage-form]');
    if (!form) return;

    const fournisseurSelect = form.querySelector('[data-pressage-fournisseur]');
    const factureSelect = form.querySelector('[data-pressage-facture]');
    const olivesInput = form.querySelector('[data-pressage-olives]');
    const rendementInput = form.querySelector('[data-pressage-rendement]');
    const huileInput = form.querySelector('[data-pressage-huile]');
    const facturesUrl = form.dataset.facturesUrl;

    // rendement = huile obtenue / olives * 100. The press yields oil, so those two figures
    // are the measured inputs and the ratio is what you read off them.
    //
    // The service recomputes this on save whatever arrives here, so this box is a preview,
    // not the source of truth — but keeping it live means the user sees the yield they are
    // about to record instead of having to trust it.
    function calcRendement() {
        if (!olivesInput || !rendementInput || !huileInput) return;

        const olives = parseFloat(olivesInput.value);
        const huile = parseFloat(huileInput.value);

        if (!Number.isFinite(olives) || !Number.isFinite(huile) || olives <= 0 || huile <= 0) {
            rendementInput.value = '';
            return;
        }

        const rendement = Math.round(huile / olives * 100 * 100) / 100;
        rendementInput.value = Number.isFinite(rendement) ? String(rendement) : '';
    }

    function resetFactures() {
        if (!factureSelect) return;
        factureSelect.innerHTML = '<option value="">— Aucune —</option>';
    }

    async function loadFactures(fournisseurId) {
        if (!factureSelect || !facturesUrl) return;

        const selected = factureSelect.value;
        resetFactures();

        if (!fournisseurId) return;

        try {
            const response = await fetch(`${facturesUrl}?fournisseurId=${encodeURIComponent(fournisseurId)}`);
            if (!response.ok) return;

            const items = await response.json();
            for (const item of items) {
                const option = document.createElement('option');
                option.value = String(item.id);
                const date = item.date ? new Date(item.date).toLocaleDateString('fr-FR') : '';
                option.textContent = `${item.numero}${date ? ` (${date})` : ''}`;
                if (String(item.id) === selected) option.selected = true;
                factureSelect.appendChild(option);
            }
        } catch {
            /* ignore network errors */
        }
    }

    if (huileInput) {
        huileInput.addEventListener('input', calcRendement);
    }

    if (olivesInput) olivesInput.addEventListener('input', calcRendement);

    if (fournisseurSelect) {
        fournisseurSelect.addEventListener('change', () => {
            loadFactures(fournisseurSelect.value);
        });
    }

    calcRendement();
})();
