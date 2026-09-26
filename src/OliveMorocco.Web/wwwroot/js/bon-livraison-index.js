(() => {
    const form = document.getElementById("bl-to-facture-form");
    const button = document.getElementById("bl-to-facture-btn");
    const body = document.getElementById("bl-list-body");

    if (!form || !button || !body)
        return;

    const updateButton = () => {
        const checked = [...body.querySelectorAll(".bl-select:checked")];
        button.disabled = checked.length === 0;
    };

    const showError = (message) => window.Zaho?.FormValidation?.show(form, message);
    const clearErrors = () => window.Zaho?.FormValidation?.clear(form);

    const validateSubmit = (event) => {
        const checked = [...body.querySelectorAll(".bl-select:checked")];
        if (checked.length === 0) {
            event.preventDefault();
            showError("Sélectionnez au moins un bon de livraison.");
            return;
        }

        const clientIds = new Set(
            checked
                .map((input) => input.closest(".list-row")?.dataset.clientId)
                .filter(Boolean));

        if (clientIds.size > 1) {
            event.preventDefault();
            showError("Les bons sélectionnés doivent appartenir au même client.");
        }
    };

    body.addEventListener("change", (event) => {
        if (event.target.classList.contains("bl-select")) {
            updateButton();
            clearErrors();
        }
    });

    form.addEventListener("submit", validateSubmit);
    updateButton();
})();
