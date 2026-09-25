(() => {
    const form = document.querySelector("[data-secteur-form]");
    if (!form) {
        return;
    }

    const lignesBody = document.getElementById("secteur-varietes-body");
    const addLineBtn = document.getElementById("add-variete-line");
    const lignesEmpty = document.getElementById("secteur-varietes-empty");
    const lineTemplate = document.getElementById("secteur-variete-line-template");
    const superficieTotaleInput = document.getElementById("superficie-totale");
    const allocationCurrent = document.getElementById("allocation-current");
    const allocationMax = document.getElementById("allocation-max");

    const toggleVarieteBtn = document.getElementById("toggleVarieteAdd");
    const varietePanel = document.getElementById("varieteAddPanel");
    const nomInput = document.getElementById("newVarieteNom");
    const codeInput = document.getElementById("newVarieteCode");
    const regionInput = document.getElementById("newVarieteRegion");
    const varieteError = document.getElementById("varieteAddError");
    const saveVarieteBtn = document.getElementById("saveVarieteBtn");
    const token = form.querySelector('input[name="__RequestVerificationToken"]')?.value;
    const varieteUrl = form.dataset.varieteUrl;

    function parseNumber(value) {
        const parsed = Number.parseFloat(value);
        return Number.isFinite(parsed) ? parsed : 0;
    }

    function formatHa(value) {
        return Number.isInteger(value) ? String(value) : value.toFixed(2).replace(/\.?0+$/, "");
    }

    function reindexLignes() {
        if (!lignesBody) {
            return;
        }

        const rows = lignesBody.querySelectorAll(".secteur-variete-line");
        rows.forEach((row, index) => {
            row.querySelectorAll("[name]").forEach((input) => {
                input.name = input.name.replace(/Lignes\[\d+\]/, `Lignes[${index}]`);
            });
        });

        if (lignesEmpty) {
            lignesEmpty.classList.toggle("hidden", rows.length > 0);
        }

        updateAllocationTotal();
    }

    function updateAllocationTotal() {
        if (!lignesBody || !allocationCurrent || !allocationMax) {
            return;
        }

        let allocated = 0;
        lignesBody.querySelectorAll(".line-superficie").forEach((input) => {
            allocated += parseNumber(input.value);
        });

        const total = parseNumber(superficieTotaleInput?.value ?? "0");
        allocationCurrent.textContent = formatHa(allocated);
        allocationMax.textContent = formatHa(total);

        const over = total > 0 && allocated > total;
        allocationCurrent.parentElement?.classList.toggle("secteur-allocation-over", over);
    }

    function syncLineState(row) {
        const select = row.querySelector(".line-variete-id");
        const superficieInput = row.querySelector(".line-superficie");
        if (!select || !superficieInput) {
            return;
        }

        const hasVariete = select.value !== "";
        superficieInput.disabled = !hasVariete;
        if (!hasVariete) {
            superficieInput.value = "";
        }
    }

    addLineBtn?.addEventListener("click", () => {
        if (!lignesBody || !lineTemplate) {
            return;
        }

        const index = lignesBody.querySelectorAll(".secteur-variete-line").length;
        const html = lineTemplate.innerHTML.replace(/__index__/g, String(index));
        const wrapper = document.createElement("tbody");
        wrapper.innerHTML = html.trim();
        const row = wrapper.firstElementChild;
        if (!row) {
            return;
        }

        lignesBody.appendChild(row);
        syncLineState(row);
        reindexLignes();
        row.querySelector(".line-variete-id")?.focus();
    });

    lignesBody?.addEventListener("click", (event) => {
        const target = event.target;
        if (!(target instanceof HTMLElement) || !target.classList.contains("remove-variete-line")) {
            return;
        }

        target.closest(".secteur-variete-line")?.remove();
        reindexLignes();
    });

    lignesBody?.addEventListener("change", (event) => {
        const target = event.target;
        if (!(target instanceof HTMLElement)) {
            return;
        }

        if (target.classList.contains("line-variete-id")) {
            syncLineState(target.closest(".secteur-variete-line"));
        }
    });

    lignesBody?.addEventListener("input", (event) => {
        const target = event.target;
        if (target instanceof HTMLInputElement && target.classList.contains("line-superficie")) {
            updateAllocationTotal();
        }
    });

    superficieTotaleInput?.addEventListener("input", updateAllocationTotal);

    lignesBody?.querySelectorAll(".secteur-variete-line").forEach(syncLineState);
    updateAllocationTotal();

    const clearVarieteError = () => {
        if (!varieteError) {
            return;
        }
        varieteError.textContent = "";
        varieteError.hidden = true;
    };

    const showVarieteError = (message) => {
        if (!varieteError) {
            return;
        }
        varieteError.textContent = message;
        varieteError.hidden = false;
    };

    const resetVarietePanel = () => {
        if (nomInput) {
            nomInput.value = "";
        }
        if (codeInput) {
            codeInput.value = "";
        }
        if (regionInput) {
            regionInput.value = "";
        }
        clearVarieteError();
    };

    const closeVarietePanel = () => {
        if (!varietePanel || !toggleVarieteBtn) {
            return;
        }
        varietePanel.hidden = true;
        toggleVarieteBtn.setAttribute("aria-expanded", "false");
        toggleVarieteBtn.textContent = "+ Créer une variété";
        resetVarietePanel();
    };

    toggleVarieteBtn?.addEventListener("click", () => {
        if (!varietePanel) {
            return;
        }

        if (varietePanel.hidden) {
            varietePanel.hidden = false;
            toggleVarieteBtn.setAttribute("aria-expanded", "true");
            toggleVarieteBtn.textContent = "Fermer";
            nomInput?.focus();
            return;
        }

        closeVarietePanel();
    });

    saveVarieteBtn?.addEventListener("click", async () => {
        if (!varieteUrl || !token) {
            return;
        }

        clearVarieteError();

        const nom = nomInput?.value.trim() ?? "";
        if (!nom) {
            showVarieteError("Le nom de la variété est obligatoire.");
            nomInput?.focus();
            return;
        }

        saveVarieteBtn.disabled = true;

        const body = new FormData();
        body.append("nom", nom);
        body.append("code", codeInput?.value.trim() ?? "");
        body.append("regionOrigine", regionInput?.value.trim() ?? "");
        body.append("__RequestVerificationToken", token);

        try {
            const response = await fetch(varieteUrl, {
                method: "POST",
                headers: { "X-Requested-With": "XMLHttpRequest" },
                body,
            });

            if (!response.ok) {
                let message = "Impossible d'enregistrer la variété.";
                try {
                    const payload = await response.json();
                    if (payload?.error) {
                        message = payload.error;
                    }
                } catch {
                    /* ignore */
                }
                showVarieteError(message);
                return;
            }

            const created = await response.json();
            form.querySelectorAll(".line-variete-id").forEach((select) => {
                const option = document.createElement("option");
                option.value = String(created.id);
                option.textContent = created.nom;
                select.appendChild(option);
            });

            const templateSelect = lineTemplate instanceof HTMLTemplateElement
                ? lineTemplate.content.querySelector(".line-variete-id")
                : lineTemplate?.querySelector(".line-variete-id");
            if (templateSelect) {
                const templateOption = document.createElement("option");
                templateOption.value = String(created.id);
                templateOption.textContent = created.nom;
                templateSelect.appendChild(templateOption);
            }

            closeVarietePanel();
        } catch {
            showVarieteError("Erreur réseau. Réessayez.");
        } finally {
            saveVarieteBtn.disabled = false;
        }
    });
})();
