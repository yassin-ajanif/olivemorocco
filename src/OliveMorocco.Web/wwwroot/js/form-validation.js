(() => {
    const escapeHtml = (value) =>
        String(value)
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;");

    const normalizeMessages = (messages) =>
        (Array.isArray(messages) ? messages : [messages]).filter(Boolean);

    const findSummary = (root) => {
        if (!root)
            return null;
        if (root.classList?.contains("form-validation-summary"))
            return root;
        return root.querySelector?.(".form-validation-summary")
            ?? root.closest?.("form")?.querySelector(".form-validation-summary")
            ?? null;
    };

    const findForm = (root) => {
        if (!root)
            return null;
        return root.tagName === "FORM" ? root : root.closest?.("form") ?? root;
    };

    window.Zaho = window.Zaho || {};
    window.Zaho.FormValidation = {
        show(scope, messages) {
            const msgs = normalizeMessages(messages);
            if (msgs.length === 0)
                return;

            const root = typeof scope === "string" ? document.getElementById(scope) : scope;
            if (!root)
                return;

            const summary = findSummary(root);
            if (summary) {
                summary.classList.add("validation-summary-errors");
                summary.innerHTML = `<ul>${msgs.map((m) => `<li>${escapeHtml(m)}</li>`).join("")}</ul>`;
                summary.scrollIntoView({ behavior: "smooth", block: "nearest" });
                return;
            }

            const form = findForm(root);
            if (!form)
                return;

            let alert = form.querySelector(".js-form-client-alert");
            if (!alert) {
                alert = document.createElement("div");
                alert.className = "dash-alert dash-alert-error js-form-client-alert";
                alert.setAttribute("role", "alert");
                form.insertBefore(alert, form.firstChild);
            }

            alert.innerHTML = msgs.length === 1
                ? escapeHtml(msgs[0])
                : `<ul>${msgs.map((m) => `<li>${escapeHtml(m)}</li>`).join("")}</ul>`;
            alert.scrollIntoView({ behavior: "smooth", block: "nearest" });
        },

        clear(scope) {
            const root = typeof scope === "string" ? document.getElementById(scope) : scope;
            if (!root)
                return;

            const summary = findSummary(root);
            if (summary)
                summary.innerHTML = "";

            const form = findForm(root);
            form?.querySelector(".js-form-client-alert")?.remove();
        },
    };
})();
