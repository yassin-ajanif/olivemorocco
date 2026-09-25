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
})();
