(() => {
    const form = document.querySelector("[data-produit-form]");
    if (!form)
        return;

    const select = form.querySelector("#VarieteId");
    const toggleBtn = form.querySelector("#toggleVarieteAdd");
    const panel = form.querySelector("#varieteAddPanel");
    const nomInput = form.querySelector("#newVarieteNom");
    const codeInput = form.querySelector("#newVarieteCode");
    const regionInput = form.querySelector("#newVarieteRegion");
    const errorEl = form.querySelector("#varieteAddError");
    const saveBtn = form.querySelector("#saveVarieteBtn");
    const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;
    const url = form.dataset.varieteUrl;

    const clearError = () => {
        if (!errorEl)
            return;
        errorEl.textContent = "";
        errorEl.hidden = true;
    };

    const showError = (message) => {
        if (!errorEl)
            return;
        errorEl.textContent = message;
        errorEl.hidden = false;
    };

    const resetPanel = () => {
        if (nomInput)
            nomInput.value = "";
        if (codeInput)
            codeInput.value = "";
        if (regionInput)
            regionInput.value = "";
        clearError();
    };

    const closePanel = () => {
        if (!panel || !toggleBtn)
            return;
        panel.hidden = true;
        toggleBtn.setAttribute("aria-expanded", "false");
        toggleBtn.textContent = "+ Ajouter une variété";
        resetPanel();
    };

    toggleBtn?.addEventListener("click", () => {
        if (!panel)
            return;

        if (panel.hidden) {
            panel.hidden = false;
            toggleBtn.setAttribute("aria-expanded", "true");
            toggleBtn.textContent = "Fermer";
            nomInput?.focus();
            return;
        }

        closePanel();
    });

    saveBtn?.addEventListener("click", async () => {
        if (!select || !url || !token)
            return;

        clearError();

        const nom = nomInput?.value.trim() ?? "";
        if (!nom) {
            showError("Le nom de la variété est obligatoire.");
            nomInput?.focus();
            return;
        }

        saveBtn.disabled = true;

        const body = new FormData();
        body.append("nom", nom);
        body.append("code", codeInput?.value.trim() ?? "");
        body.append("regionOrigine", regionInput?.value.trim() ?? "");
        body.append("__RequestVerificationToken", token);

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: { "X-Requested-With": "XMLHttpRequest" },
                body,
            });

            if (!response.ok) {
                let message = "Impossible d'enregistrer la variété.";
                try {
                    const payload = await response.json();
                    if (payload?.error)
                        message = payload.error;
                } catch {
                    /* ignore parse errors */
                }
                showError(message);
                return;
            }

            const created = await response.json();
            const option = document.createElement("option");
            option.value = String(created.id);
            option.textContent = created.nom;
            select.appendChild(option);
            select.value = String(created.id);
            closePanel();
        } catch {
            showError("Erreur réseau. Réessayez.");
        } finally {
            saveBtn.disabled = false;
        }
    });
})();
