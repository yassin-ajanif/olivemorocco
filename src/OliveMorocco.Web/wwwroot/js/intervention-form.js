(function () {
    const intrantSelect = document.getElementById('intrant-id');
    const qtyInput = document.getElementById('quantite-intrant');
    const uniteHint = document.getElementById('intrant-unite-hint');

    if (!intrantSelect || !qtyInput) {
        return;
    }

    function syncIntrantFields() {
        const selected = intrantSelect.options[intrantSelect.selectedIndex];
        const hasIntrant = intrantSelect.value !== '';

        qtyInput.disabled = !hasIntrant;
        if (!hasIntrant) {
            qtyInput.value = '';
        }

        if (uniteHint) {
            const unite = selected?.dataset?.unite;
            uniteHint.textContent = hasIntrant && unite ? `(${unite})` : '';
        }
    }

    intrantSelect.addEventListener('change', syncIntrantFields);
    syncIntrantFields();
})();
