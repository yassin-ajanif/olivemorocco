(() => {
    const body = document.getElementById("paiements-body");
    if (!body) return;

    const addBtn = document.getElementById("add-paiement-line");
    const template = document.getElementById("paiement-line-template");
    const emptyHint = document.getElementById("paiements-empty");
    const encaisseEl = document.getElementById("paiements-encaisse");
    const attenteEl = document.getElementById("paiements-attente");
    const totalEncaisseEl = document.getElementById("total-encaisse");
    const totalResteEl = document.getElementById("total-reste");
    const statutEl = document.getElementById("paiement-statut");
    const totalTtcEl = document.getElementById("total-ttc");

    function parseAmount(input) {
        const value = Number.parseFloat(input?.value ?? "");
        return Number.isFinite(value) && value > 0 ? value : 0;
    }

    function formatDh(amount) {
        return amount.toLocaleString("fr-FR", {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2,
        }) + " DH";
    }

    function parseTotalTtc() {
        if (!totalTtcEl) return 0;
        const text = totalTtcEl.textContent ?? "";
        const normalized = text.replace(/\s/g, "").replace("DH", "").replace(",", ".");
        const value = Number.parseFloat(normalized);
        return Number.isFinite(value) ? value : 0;
    }

    function reindexPaiements() {
        body.querySelectorAll(".paiement-line").forEach((row, index) => {
            row.querySelectorAll("[name]").forEach((input) => {
                input.name = input.name.replace(/Paiements\[\d+\]/, `Paiements[${index}]`);
            });
        });

        if (emptyHint) {
            emptyHint.classList.toggle("hidden", body.querySelectorAll(".paiement-line").length > 0);
        }

        updatePaiementsSummary();
    }

    function updatePaiementsSummary() {
        let encaisse = 0;
        let attente = 0;

        body.querySelectorAll(".paiement-line").forEach((row) => {
            const montant = parseAmount(row.querySelector(".line-paiement-montant"));
            const checked = row.querySelector(".line-paiement-encaisse")?.checked ?? false;
            if (checked) encaisse += montant;
            else attente += montant;
        });

        const totalTtc = parseTotalTtc();
        const reste = Math.max(0, totalTtc - encaisse);

        if (encaisseEl) encaisseEl.textContent = formatDh(encaisse);
        if (attenteEl) attenteEl.textContent = formatDh(attente);
        if (totalEncaisseEl) totalEncaisseEl.textContent = formatDh(encaisse);
        if (totalResteEl) totalResteEl.textContent = formatDh(reste);

        if (statutEl) {
            const paid = totalTtc > 0 && encaisse >= totalTtc;
            statutEl.textContent = paid ? "Payée" : "Non payée";
            statutEl.classList.toggle("badge-ok", paid);
            statutEl.classList.toggle("badge-warn", !paid);
        }
    }

    function bindPaiementRow(row) {
        const removeBtn = row.querySelector(".remove-paiement-line");
        const montantInput = row.querySelector(".line-paiement-montant");
        const encaisseInput = row.querySelector(".line-paiement-encaisse");

        if (removeBtn) {
            removeBtn.addEventListener("click", () => {
                row.remove();
                reindexPaiements();
            });
        }

        if (montantInput) {
            montantInput.addEventListener("input", updatePaiementsSummary);
        }

        if (encaisseInput) {
            encaisseInput.addEventListener("change", updatePaiementsSummary);
        }
    }

    if (addBtn && template) {
        addBtn.addEventListener("click", () => {
            const index = body.querySelectorAll(".paiement-line").length;
            const html = template.innerHTML.replace(/__index__/g, String(index));
            const wrapper = document.createElement("tbody");
            wrapper.innerHTML = html.trim();
            const row = wrapper.firstElementChild;
            body.appendChild(row);
            bindPaiementRow(row);
            reindexPaiements();
        });
    }

    body.querySelectorAll(".paiement-line").forEach(bindPaiementRow);
    reindexPaiements();

    if (totalTtcEl) {
        const observer = new MutationObserver(updatePaiementsSummary);
        observer.observe(totalTtcEl, { childList: true, characterData: true, subtree: true });
    }

    window.Zaho = window.Zaho || {};
    window.Zaho.updatePaiementsSummary = updatePaiementsSummary;
})();
