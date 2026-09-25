(() => {
    const form = document.getElementById("br-to-facture-form");
    const button = document.getElementById("br-to-facture-btn");
    const body = document.getElementById("br-list-body");

    if (!form || !button || !body)
        return;

    const updateButton = () => {
        const checked = [...body.querySelectorAll(".br-select:checked")];
        button.disabled = checked.length === 0;
    };

    const validateSubmit = (event) => {
        const checked = [...body.querySelectorAll(".br-select:checked")];
        if (checked.length === 0) {
            event.preventDefault();
            alert("Sélectionnez au moins un bon de réception.");
            return;
        }

        const fournisseurIds = new Set(
            checked
                .map((input) => input.closest(".list-row")?.dataset.fournisseurId)
                .filter(Boolean));

        if (fournisseurIds.size > 1) {
            event.preventDefault();
            alert("Les bons sélectionnés doivent appartenir au même fournisseur.");
        }
    };

    body.addEventListener("change", (event) => {
        if (event.target.classList.contains("br-select"))
            updateButton();
    });

    form.addEventListener("submit", validateSubmit);
    updateButton();
})();
