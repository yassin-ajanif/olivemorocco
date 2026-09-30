/* Arabic-gloss switch. Loaded once by _DashboardLayout; the preference itself is read
   earlier, by the inline boot script in the layout's <head>, so there is no flash. */
(function () {
    var KEY = 'om.translations';
    var root = document.documentElement;

    var setState = function (on) {
        root.dataset.tr = on ? 'on' : 'off';
        document.querySelectorAll('[data-tr-toggle]').forEach(function (btn) {
            btn.setAttribute('aria-pressed', on ? 'true' : 'false');
        });
    };

    document.querySelectorAll('[data-tr-toggle]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var on = root.dataset.tr !== 'on';
            setState(on);
            try {
                localStorage.setItem(KEY, on ? 'on' : 'off');
            } catch {
                /* Private browsing or a full quota: the switch still works for this page,
                   it just will not be remembered next time. */
            }
        });
    });

    // The head already decided; the button's aria-pressed has to match it.
    setState(root.dataset.tr === 'on');
})();