(() => {
    const roundMoney = (value) => Math.round(value * 100) / 100;

    const parseNum = (value) => {
        const n = Number.parseFloat(value);
        return Number.isFinite(n) ? n : 0;
    };

    const puTtc = (pu, tva) => roundMoney(pu * (1 + tva / 100));

    const lineTtc = (qte, pu, remise, tva) =>
        roundMoney(Math.max(0, roundMoney(qte * puTtc(pu, tva)) - remise));

    const lineHt = (qte, pu, remise, tva) => {
        const rate = 1 + tva / 100;
        return rate === 0 ? 0 : roundMoney(lineTtc(qte, pu, remise, tva) / rate);
    };

    const lineTotalsForRow = (row, qte, pu, remise, tva) => ({
        ht: lineHt(qte, pu, remise, tva),
        ttc: lineTtc(qte, pu, remise, tva),
    });

    const renderLineTtcCells = (puTtcCell, ttcCell, qte, pu, remise, tva, formatMoney) => {
        const unitTtc = puTtc(pu, tva);
        const lineTtcValue = lineTtc(qte, pu, remise, tva);
        const unitAfter = qte > 0 ? roundMoney(lineTtcValue / qte) : unitTtc;

        if (puTtcCell)
            puTtcCell.textContent = formatMoney(unitAfter);
        if (ttcCell)
            ttcCell.textContent = formatMoney(lineTtcValue);

        // Mirror onto the collapsed mobile header, which sits next to the designation in
        // the card header rather than in a cell of its own. Reached from the TTC cell
        // because the head is a sibling of the row's cells, not a descendant of one.
        // Cleared when there is no cell to read from, so a document that does not render
        // a per-line TTC cannot leave a stale figure sitting in the header.
        const head = ttcCell?.closest("tr")?.querySelector(".devis-line-head-ttc");
        if (head)
            head.textContent = formatMoney(lineTtcValue);
    };

    window.Zaho = window.Zaho || {};
    window.Zaho.DocumentTotals = {
        roundMoney,
        puTtc,
        lineTtc,
        lineHt,
        lineTotalsForRow,
        renderLineTtcCells,
    };
})();
