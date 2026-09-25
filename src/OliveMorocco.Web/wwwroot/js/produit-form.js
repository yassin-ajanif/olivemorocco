(() => {
    const form = document.querySelector("[data-produit-form]");
    if (!form)
        return;

    const select = form.querySelector("#VarieteId");
    const editBtn = form.querySelector("#editVarieteBtn");
    const deleteBtn = form.querySelector("#deleteVarieteBtn");
    const toggleBtn = form.querySelector("#toggleVarieteAdd");
    const panel = form.querySelector("#varieteManagePanel");
    const panelTitle = form.querySelector("#varieteManagePanelTitle");
    const nomInput = form.querySelector("#varieteNom");
    const codeInput = form.querySelector("#varieteCode");
    const regionInput = form.querySelector("#varieteRegion");
    const errorEl = form.querySelector("#varieteManageError");
    const saveBtn = form.querySelector("#saveVarieteBtn");
    const cancelBtn = form.querySelector("#cancelVarieteBtn");
    const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;
    const varieteBase = form.dataset.varieteBase?.replace(/\/$/, "");

    /** @type {"create" | "edit" | null} */
    let panelMode = null;
    /** @type {number | null} */
    let editingId = null;

    const hasSelection = () => Boolean(select?.value);

    const varieteUrl = (id) => `${varieteBase}/${id}`;
    const varieteDeleteUrl = (id) => `${varieteBase}/Delete/${id}`;

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

    const resetFields = () => {
        if (nomInput)
            nomInput.value = "";
        if (codeInput)
            codeInput.value = "";
        if (regionInput)
            regionInput.value = "";
        clearError();
    };

    const syncActionButtons = () => {
        const enabled = hasSelection();
        if (editBtn)
            editBtn.disabled = !enabled;
        if (deleteBtn)
            deleteBtn.disabled = !enabled;
    };

    const closePanel = () => {
        if (!panel || !toggleBtn)
            return;

        panel.hidden = true;
        toggleBtn.hidden = false;
        toggleBtn.setAttribute("aria-expanded", "false");
        toggleBtn.textContent = "+ Ajouter une variété";
        panelMode = null;
        editingId = null;
        resetFields();
    };

    const openPanel = (mode) => {
        if (!panel || !toggleBtn || !panelTitle || !saveBtn)
            return;

        panelMode = mode;
        panel.hidden = false;
        toggleBtn.hidden = true;
        toggleBtn.setAttribute("aria-expanded", "true");

        if (mode === "edit") {
            panelTitle.textContent = "Modifier la variété";
            saveBtn.textContent = "Enregistrer les modifications";
        } else {
            panelTitle.textContent = "Nouvelle variété";
            saveBtn.textContent = "Enregistrer";
            editingId = null;
        }

        nomInput?.focus();
    };

    const readFormValues = () => ({
        nom: nomInput?.value.trim() ?? "",
        code: codeInput?.value.trim() ?? "",
        regionOrigine: regionInput?.value.trim() ?? "",
    });

    const appendVarieteFields = (body) => {
        const values = readFormValues();
        body.append("nom", values.nom);
        body.append("code", values.code);
        body.append("regionOrigine", values.regionOrigine);
        body.append("__RequestVerificationToken", token ?? "");
    };

    const parseError = async (response, fallback) => {
        try {
            const payload = await response.json();
            if (payload?.error)
                return payload.error;
        } catch {
            /* ignore */
        }
        return fallback;
    };

    const upsertOption = (id, nom) => {
        if (!select)
            return;

        const value = String(id);
        let option = select.querySelector(`option[value="${value}"]`);
        if (!option) {
            option = document.createElement("option");
            option.value = value;
            select.appendChild(option);
        }

        option.textContent = nom;
        select.value = value;
        syncActionButtons();
    };

    const removeOption = (id) => {
        if (!select)
            return;

        select.querySelector(`option[value="${String(id)}"]`)?.remove();
        select.value = "";
        syncActionButtons();
    };

    select?.addEventListener("change", syncActionButtons);

    toggleBtn?.addEventListener("click", () => {
        resetFields();
        openPanel("create");
    });

    cancelBtn?.addEventListener("click", closePanel);

    editBtn?.addEventListener("click", async () => {
        if (!select?.value || !varieteBase || !token)
            return;

        const id = Number.parseInt(select.value, 10);
        if (!Number.isFinite(id))
            return;

        editBtn.disabled = true;

        try {
            const response = await fetch(varieteUrl(id), {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            });

            if (!response.ok) {
                alert(await parseError(response, "Impossible de charger la variété."));
                return;
            }

            const variete = await response.json();
            editingId = variete.id;
            if (nomInput)
                nomInput.value = variete.nom ?? "";
            if (codeInput)
                codeInput.value = variete.code ?? "";
            if (regionInput)
                regionInput.value = variete.regionOrigine ?? "";
            clearError();
            openPanel("edit");
        } catch {
            alert("Erreur réseau. Réessayez.");
        } finally {
            editBtn.disabled = !hasSelection();
        }
    });

    deleteBtn?.addEventListener("click", async () => {
        if (!select?.value || !varieteBase || !token)
            return;

        const id = Number.parseInt(select.value, 10);
        if (!Number.isFinite(id))
            return;

        const label = select.options[select.selectedIndex]?.textContent?.trim() ?? "cette variété";
        if (!window.confirm(`Supprimer la variété « ${label} » ?`))
            return;

        deleteBtn.disabled = true;

        const body = new FormData();
        body.append("__RequestVerificationToken", token);

        try {
            const response = await fetch(varieteDeleteUrl(id), {
                method: "POST",
                headers: { "X-Requested-With": "XMLHttpRequest" },
                body,
            });

            if (!response.ok) {
                alert(await parseError(response, "Impossible de supprimer la variété."));
                return;
            }

            removeOption(id);
            closePanel();
        } catch {
            alert("Erreur réseau. Réessayez.");
        } finally {
            deleteBtn.disabled = !hasSelection();
        }
    });

    saveBtn?.addEventListener("click", async () => {
        if (!select || !varieteBase || !token || !panelMode)
            return;

        clearError();

        const values = readFormValues();
        if (!values.nom) {
            showError("Le nom de la variété est obligatoire.");
            nomInput?.focus();
            return;
        }

        saveBtn.disabled = true;

        const body = new FormData();
        appendVarieteFields(body);

        const isEdit = panelMode === "edit" && editingId !== null;
        const url = isEdit ? varieteUrl(editingId) : varieteBase;

        try {
            const response = await fetch(url, {
                method: "POST",
                headers: { "X-Requested-With": "XMLHttpRequest" },
                body,
            });

            if (!response.ok) {
                showError(await parseError(response, "Impossible d'enregistrer la variété."));
                return;
            }

            const saved = await response.json();
            upsertOption(saved.id, saved.nom);
            closePanel();
        } catch {
            showError("Erreur réseau. Réessayez.");
        } finally {
            saveBtn.disabled = false;
        }
    });

    syncActionButtons();
})();
