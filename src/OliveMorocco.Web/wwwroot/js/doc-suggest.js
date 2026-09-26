(() => {
    window.Zaho = window.Zaho || {};

    /**
     * Autocomplete dropdowns for document forms (client, article, intrant, …).
     * Closes each list on mousedown outside its .doc-suggest-wrap and ignores stale fetches.
     */
    window.Zaho.initDocSuggest = (fields) => {
        const generationByContainer = new WeakMap();
        const fetchControllers = new WeakMap();

        const debounce = (fn, ms) => {
            let timer;
            const debounced = (...args) => {
                clearTimeout(timer);
                timer = setTimeout(() => fn(...args), ms);
            };
            debounced.cancel = () => clearTimeout(timer);
            return debounced;
        };

        const showSuggestions = (container, html) => {
            if (!container)
                return;
            container.innerHTML = html;
            container.hidden = !html.trim();
        };

        const hideSuggestions = (container) => {
            if (!container)
                return;
            generationByContainer.set(container, (generationByContainer.get(container) ?? 0) + 1);
            fetchControllers.get(container)?.abort();
            fetchControllers.delete(container);
            showSuggestions(container, "");
        };

        const fetchSuggestions = debounce(async (url, query, container) => {
            const requestGen = generationByContainer.get(container) ?? 0;

            fetchControllers.get(container)?.abort();
            const controller = new AbortController();
            fetchControllers.set(container, controller);

            const params = new URLSearchParams();
            if (query)
                params.set("search", query);

            try {
                const response = await fetch(`${url}?${params.toString()}`, {
                    headers: { "X-Requested-With": "XMLHttpRequest" },
                    signal: controller.signal,
                });
                if (!response.ok || requestGen !== (generationByContainer.get(container) ?? 0))
                    return;
                showSuggestions(container, await response.text());
            } catch (error) {
                if (error?.name !== "AbortError")
                    return;
            } finally {
                if (fetchControllers.get(container) === controller)
                    fetchControllers.delete(container);
            }
        }, 300);

        for (const field of fields) {
            field.input?.addEventListener("input", () => {
                if (field.isLocked?.())
                    return;
                field.onBeforeFetch?.();
                fetchSuggestions(field.url, field.input.value.trim(), field.container);
            });
        }

        document.addEventListener("mousedown", (event) => {
            let dismissed = false;
            for (const field of fields) {
                if (!field.container || field.container.hidden)
                    continue;
                const wrap = field.input?.closest(".doc-suggest-wrap");
                if (wrap?.contains(event.target))
                    continue;
                hideSuggestions(field.container);
                dismissed = true;
            }
            if (dismissed)
                fetchSuggestions.cancel();
        });

        return { showSuggestions, hideSuggestions };
    };
})();
