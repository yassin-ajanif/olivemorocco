(function () {
    const form = document.querySelector('[data-pressage-form]');
    if (!form) return;

    const fournisseurSelect = form.querySelector('[data-pressage-fournisseur]');
    const factureSelect = form.querySelector('[data-pressage-facture]');
    const olivesInput = form.querySelector('[data-pressage-olives]');
    const rendementInput = form.querySelector('[data-pressage-rendement]');
    const huileInput = form.querySelector('[data-pressage-huile]');
    const facturesUrl = form.dataset.facturesUrl;

    let huileManual = huileInput && huileInput.value !== '';

    function calcHuile() {
        if (!olivesInput || !rendementInput || !huileInput || huileManual) return;

        const olives = parseFloat(olivesInput.value);
        const rendement = parseFloat(rendementInput.value);
        if (!Number.isFinite(olives) || !Number.isFinite(rendement) || olives <= 0 || rendement <= 0) {
            huileInput.value = '';
            return;
        }

        const huile = Math.round(olives * rendement / 100 * 100) / 100;
        huileInput.value = Number.isFinite(huile) ? String(huile) : '';
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
        huileInput.addEventListener('input', () => {
            huileManual = huileInput.value.trim() !== '';
        });
    }

    if (olivesInput) olivesInput.addEventListener('input', calcHuile);
    if (rendementInput) rendementInput.addEventListener('input', calcHuile);

    if (fournisseurSelect) {
        fournisseurSelect.addEventListener('change', () => {
            loadFactures(fournisseurSelect.value);
        });
    }

    calcHuile();
})();
