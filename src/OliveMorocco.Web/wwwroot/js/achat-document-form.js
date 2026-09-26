(() => {
    window.Zaho = window.Zaho || {};

    window.Zaho.initAchatDocumentForm = (config) => {
        const fournisseurIdInput = document.getElementById(config.fournisseurIdInputId ?? "fournisseur-id");
        const fournisseurSearchInput = document.getElementById(config.fournisseurSearchInputId ?? "fournisseur-search");
        const fournisseurSuggestions = document.getElementById(config.fournisseurSuggestionsId ?? "fournisseur-suggestions");
        const intrantSearchInput = document.getElementById(config.intrantSearchInputId ?? "intrant-search");
        const intrantSuggestions = document.getElementById(config.intrantSuggestionsId ?? "intrant-suggestions");
        const body = document.getElementById(config.bodyId);
        const template = document.getElementById(config.templateId);
        const remiseGlobaleInput = config.remiseGlobaleId
            ? document.getElementById(config.remiseGlobaleId)
            : null;
        const emptyHint = config.emptyHintId
            ? document.getElementById(config.emptyHintId)
            : null;
        const form = document.getElementById(config.formId);
        const lineRowClass = config.lineRowClass ?? "devis-line";

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

        const fournisseurLocked = fournisseurSearchInput?.hasAttribute("readonly") ?? false;

        fournisseurSearchInput?.addEventListener("input", () => {
            if (fournisseurLocked)
                return;
            if (fournisseurIdInput)
                fournisseurIdInput.value = "";
            fetchSuggestions("/Achat/Suggestions/Fournisseurs", fournisseurSearchInput.value.trim(), fournisseurSuggestions);
        });

        fournisseurSuggestions?.addEventListener("click", (event) => {
            const btn = event.target.closest(".fournisseur-suggestion");
            if (!btn)
                return;

            if (fournisseurIdInput)
                fournisseurIdInput.value = btn.dataset.fournisseurId || "";
            if (fournisseurSearchInput)
                fournisseurSearchInput.value = btn.dataset.fournisseurNom || "";
            showSuggestions(fournisseurSuggestions, "");
        });

        intrantSearchInput?.addEventListener("input", () => {
            fetchSuggestions("/Achat/Suggestions/Intrants", intrantSearchInput.value.trim(), intrantSuggestions);
        });

        document.addEventListener("click", (event) => {
            if (!event.target.closest(".doc-suggest-wrap")) {
                showSuggestions(fournisseurSuggestions, "");
                showSuggestions(intrantSuggestions, "");
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

        const billQty = (row) => {
            const recue = row.querySelector(".line-qte-recue");
            if (recue)
                return parseNum(recue.value);
            return parseNum(row.querySelector(".line-qte")?.value);
        };

        const updateEmptyHint = () => {
            const hasLines = body.querySelectorAll(`.${lineRowClass}`).length > 0;
            if (emptyHint)
                emptyHint.classList.toggle("hidden", hasLines);
        };

        const reindexLines = () => {
            body.querySelectorAll(`.${lineRowClass}`).forEach((row, index) => {
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

            body.querySelectorAll(`.${lineRowClass}`).forEach((row) => {
                const qte = billQty(row);
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

        const findLineByIntrantId = (intrantId) => {
            const id = String(intrantId);
            for (const row of body.querySelectorAll(`.${lineRowClass}`)) {
                if (row.querySelector(".intrant-id")?.value !== id)
                    continue;
                const brInput = row.querySelector('input[name*="BonReceptionId"]');
                if (brInput?.value)
                    continue;
                return row;
            }
            return null;
        };

        const incrementQty = (row) => {
            const qteInput = row.querySelector(".line-qte");
            if (qteInput)
                qteInput.value = String(parseNum(qteInput.value) + 1);
            const recueInput = row.querySelector(".line-qte-recue");
            if (recueInput)
                recueInput.value = String(parseNum(recueInput.value) + 1);
        };

        const addLine = (data = {}) => {
            const designation = (data.designation || "").trim();
            const intrantId = data.intrantId || "";
            if (!intrantId || !designation) {
                alert("Impossible d'ajouter cet intrant : données incomplètes.");
                return;
            }

            const existingRow = findLineByIntrantId(intrantId);
            if (existingRow) {
                incrementQty(existingRow);
                recalculate();
                return;
            }

            const index = body.querySelectorAll(`.${lineRowClass}`).length;
            const html = template.innerHTML.replaceAll("__index__", String(index));
            if (emptyHint)
                emptyHint.insertAdjacentHTML("beforebegin", html);
            else
                body.insertAdjacentHTML("beforeend", html);

            const row = emptyHint?.previousElementSibling ?? body.lastElementChild;
            if (!row)
                return;

            row.querySelector(".intrant-id").value = intrantId;
            row.querySelector(".line-ref").value = data.unite || "";
            row.querySelector(".line-designation").value = designation;
            const uniteInput = row.querySelector(".line-unite");
            if (uniteInput)
                uniteInput.value = data.unite || "U";
            row.querySelector(".line-pu").value = data.prix || "0";
            row.querySelector(".line-tva").value = data.tva || "20";

            reindexLines();
            updateEmptyHint();
            recalculate();
        };

        intrantSuggestions?.addEventListener("click", (event) => {
            const btn = event.target.closest(".intrant-suggestion");
            if (!btn)
                return;

            addLine({
                intrantId: btn.dataset.intrantId || "",
                designation: btn.dataset.nom || "",
                unite: btn.dataset.unite || "",
                prix: "0",
                tva: "20",
            });

            if (intrantSearchInput)
                intrantSearchInput.value = "";
            showSuggestions(intrantSuggestions, "");
        });

        document.getElementById(config.removeLineButtonId ?? "remove-selected-line")?.addEventListener("click", () => {
            const selected = body.querySelector(".line-select:checked")?.closest(`.${lineRowClass}`);
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

            const row = toggle.closest(`.${lineRowClass}`);
            if (!row)
                return;

            const collapsed = row.classList.toggle("is-collapsed");
            toggle.setAttribute("aria-expanded", collapsed ? "false" : "true");
        });

        if (config.validateFournisseur !== false) {
            form?.addEventListener("submit", (event) => {
                const fournisseurId = Number.parseInt(fournisseurIdInput?.value ?? "", 10);
                if (!Number.isFinite(fournisseurId) || fournisseurId <= 0) {
                    event.preventDefault();
                    alert("Sélectionnez un fournisseur depuis la recherche.");
                    return;
                }

                const rows = [...body.querySelectorAll(`.${lineRowClass}`)];
                if (rows.length === 0) {
                    event.preventDefault();
                    alert("Ajoutez au moins une ligne via la recherche d'intrants.");
                }
            });
        }

        updateEmptyHint();
        recalculate();
    };
})();
