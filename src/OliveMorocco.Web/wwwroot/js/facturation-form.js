(() => {
    window.Zaho?.initVenteDocumentForm({
        formId: "facturation-form",
        bodyId: "facturation-lignes-body",
        templateId: "facturation-line-template",
        emptyHintId: "facturation-empty-hint",
        remiseGlobaleId: "remise-globale",
    });
})();
