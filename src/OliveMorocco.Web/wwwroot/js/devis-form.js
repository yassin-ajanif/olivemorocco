(() => {
    const clientIdInput = document.getElementById("client-id");
    const clientSearchInput = document.getElementById("client-search");
    const clientSuggestions = document.getElementById("client-suggestions");
    const articleSearchInput = document.getElementById("article-search");
    const articleSuggestions = document.getElementById("article-suggestions");
    const body = document.getElementById("devis-lignes-body");
    const template = document.getElementById("devis-line-template");
    const remiseGlobaleInput = document.getElementById("remise-globale");
    const emptyHint = document.getElementById("devis-empty-hint");
    const form = document.getElementById("devis-form");

    const debounce = (fn, ms) => {
        let timer;
        return (...args) => {
            clearTimeout(timer);
            timer = setTimeout(() => fn(...args), ms);
        };
    };

    const showSuggestions = (container, html) => {
        if (!container)
            return;
        container.innerHTML = html;
        container.hidden = !html.trim();
    };

    const fetchSuggestions = debounce(async (url, query, container) => {
        const params = new URLSearchParams();
        if (query)
            params.set("search", query);

        try {
            const response = await fetch(`${url}?${params.toString()}`, {
                headers: { "X-Requested-With": "XMLHttpRequest" },
            });
            if (!response.ok)
                return;
            showSuggestions(container, await response.text());
        } catch {
            /* ignore network errors while typing */
        }
    }, 300);

    clientSearchInput?.addEventListener("input", () => {
        if (clientIdInput)
            clientIdInput.value = "";
        fetchSuggestions("/Vente/Suggestions/Clients", clientSearchInput.value.trim(), clientSuggestions);
    });

    clientSuggestions?.addEventListener("click", (event) => {
        const btn = event.target.closest(".client-suggestion");
        if (!btn)
            return;

        if (clientIdInput)
            clientIdInput.value = btn.dataset.clientId || "";
        if (clientSearchInput)
            clientSearchInput.value = btn.dataset.clientNom || "";
        showSuggestions(clientSuggestions, "");
    });

    articleSearchInput?.addEventListener("input", () => {
        fetchSuggestions("/Vente/Suggestions/Articles", articleSearchInput.value.trim(), articleSuggestions);
    });

    document.addEventListener("click", (event) => {
        if (!event.target.closest(".doc-suggest-wrap")) {
            showSuggestions(clientSuggestions, "");
            showSuggestions(articleSuggestions, "");
        }
    });

    if (!body || !template)
        return;

    const formatMoney = (value) =>
        value.toLocaleString("fr-FR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });

    const parseNum = (value) => {
        const n = Number.parseFloat(value);
        return Number.isFinite(n) ? n : 0;
    };

    const totals = () => window.Zaho?.DocumentTotals;

    const updateEmptyHint = () => {
        const hasLines = body.querySelectorAll(".devis-line").length > 0;
        if (emptyHint)
            emptyHint.classList.toggle("hidden", hasLines);
    };

    const reindexLines = () => {
        body.querySelectorAll(".devis-line").forEach((row, index) => {
            row.querySelectorAll("[name]").forEach((input) => {
                input.name = input.name.replace(/Lignes\[\d+\]/, `Lignes[${index}]`);
            });
        });
    };

    const recalculate = () => {
        const docTotals = totals();
        if (!docTotals)
            return;

        let totalHt = 0;
        let totalTva = 0;

        body.querySelectorAll(".devis-line").forEach((row) => {
            const qte = parseNum(row.querySelector(".line-qte")?.value);
            const pu = parseNum(row.querySelector(".line-pu")?.value);
            const remise = parseNum(row.querySelector(".line-remise")?.value);
            const tva = parseNum(row.querySelector(".line-tva")?.value);

            const { ht, ttc } = docTotals.lineTotalsForRow(row, qte, pu, remise, tva);
            totalHt += ht;
            totalTva += ttc - ht;

            docTotals.renderLineTtcCells(
                row.querySelector(".line-pu-ttc"),
                row.querySelector(".line-ttc"),
                qte,
                pu,
                remise,
                tva,
                formatMoney);

            const headTtc = row.querySelector(".devis-line-head-ttc");
            const lineTtc = row.querySelector(".line-ttc");
            if (headTtc && lineTtc)
                headTtc.textContent = lineTtc.textContent;
        });

        const remiseGlobale = parseNum(remiseGlobaleInput?.value);
        totalHt = Math.max(0, totalHt - remiseGlobale);
        const totalTtc = totalHt + totalTva;

        const htEl = document.getElementById("total-ht");
        const tvaEl = document.getElementById("total-tva");
        const ttcEl = document.getElementById("total-ttc");
        if (htEl) htEl.textContent = `${formatMoney(totalHt)} DH`;
        if (tvaEl) tvaEl.textContent = `${formatMoney(totalTva)} DH`;
        if (ttcEl) ttcEl.textContent = `${formatMoney(totalTtc)} DH`;
    };

    const findLineByProduitId = (produitId) => {
        const id = String(produitId);
        for (const row of body.querySelectorAll(".devis-line")) {
            if (row.querySelector(".produit-id")?.value === id)
                return row;
        }
        return null;
    };

    const addLine = (data = {}) => {
        const designation = (data.designation || "").trim();
        const produitId = data.produitId || "";
        if (!produitId || !designation) {
            alert("Impossible d'ajouter cet article : données incomplètes.");
            return;
        }

        const existingRow = findLineByProduitId(produitId);
        if (existingRow) {
            const qteInput = existingRow.querySelector(".line-qte");
            if (qteInput)
                qteInput.value = String(parseNum(qteInput.value) + 1);
            recalculate();
            return;
        }

        const index = body.querySelectorAll(".devis-line").length;
        const html = template.innerHTML.replaceAll("__index__", String(index));
        if (emptyHint)
            emptyHint.insertAdjacentHTML("beforebegin", html);
        else
            body.insertAdjacentHTML("beforeend", html);

        const row = emptyHint?.previousElementSibling ?? body.lastElementChild;
        if (!row)
            return;

        row.querySelector(".produit-id").value = produitId;
        row.querySelector(".line-ref").value = data.reference || "";
        row.querySelector(".line-designation").value = designation;
        row.querySelector(".line-unite").value = data.unite || "U";
        if (data.prix)
            row.querySelector(".line-pu").value = data.prix;
        if (data.tva)
            row.querySelector(".line-tva").value = data.tva;

        reindexLines();
        updateEmptyHint();
        recalculate();
    };

    articleSuggestions?.addEventListener("click", (event) => {
        const btn = event.target.closest(".article-suggestion");
        if (!btn)
            return;

        addLine({
            produitId: btn.dataset.produitId || "",
            reference: btn.dataset.reference || "",
            designation: btn.dataset.designation || "",
            unite: btn.dataset.unite || "",
            prix: btn.dataset.prix || "0",
            tva: btn.dataset.tva || "0",
        });

        if (articleSearchInput)
            articleSearchInput.value = "";
        showSuggestions(articleSuggestions, "");
    });

    document.getElementById("remove-selected-line")?.addEventListener("click", () => {
        const selected = body.querySelector(".line-select:checked")?.closest(".devis-line");
        if (!selected)
            return;

        selected.remove();
        reindexLines();
        updateEmptyHint();
        recalculate();
    });

    body.addEventListener("input", (event) => {
        if (event.target.classList.contains("calc-trigger"))
            recalculate();
    });

    body.addEventListener("click", (event) => {
        const toggle = event.target.closest(".devis-line-toggle");
        if (!toggle)
            return;

        const row = toggle.closest(".devis-line");
        if (!row)
            return;

        const collapsed = row.classList.toggle("is-collapsed");
        toggle.setAttribute("aria-expanded", collapsed ? "false" : "true");
    });

    form?.addEventListener("submit", (event) => {
        const clientId = Number.parseInt(clientIdInput?.value ?? "", 10);
        if (!Number.isFinite(clientId) || clientId <= 0) {
            event.preventDefault();
            alert("Sélectionnez un client depuis la recherche.");
            return;
        }

        const rows = [...body.querySelectorAll(".devis-line")];
        if (rows.length === 0) {
            event.preventDefault();
            alert("Ajoutez au moins une ligne via la recherche d'articles.");
        }
    });

    updateEmptyHint();
    recalculate();
})();
