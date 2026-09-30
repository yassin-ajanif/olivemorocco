/* Arabic-gloss switch. Loaded once by _DashboardLayout; the preference itself is read
   earlier, by the inline boot script in the layout's <head>, so there is no flash. */
(function () {
    var KEY = 'om.translations';
    var root = document.documentElement;

    /* The attributes a label can hide inside. data-label is not among them: the list rows
       draw it with content: attr(data-label), which is content, and content may not wait
       for this file at the end of the body — see _DashboardTranslations.css. */
    var SWAPPABLE = ['title', 'placeholder', 'aria-label'];

    /* What each element was sent with, taken before the switch ever touches it.
       The Arabic arrives as a second attribute rather than as a swap of the first, so the
       view states its French once and the script remembers the value it displaced instead
       of being told it twice. Turning the switch off therefore restores exactly what the
       server sent, and an untranslated label was never given a data-tr-ar in the first
       place, so it is left alone. */
    var sent = new WeakMap();

    var remember = function () {
        document.querySelectorAll('[data-tr-ar]').forEach(function (el) {
            var was = {};

            SWAPPABLE.forEach(function (attr) {
                if (el.hasAttribute(attr)) {
                    was[attr] = el.getAttribute(attr);
                }
            });

            // An <option> carries its label as text, not as an attribute: its content model
            // is text and nothing else, which is why it could never have held a _TrLabel.
            if (el.tagName === 'OPTION') {
                was.text = el.textContent;
            }

            if (Object.keys(was).length) {
                sent.set(el, was);
            }
        });
    };

    var swap = function (on) {
        document.querySelectorAll('[data-tr-ar]').forEach(function (el) {
            var ar = el.getAttribute('data-tr-ar');
            var was = sent.get(el);

            if (ar === null || was === undefined) {
                return;
            }

            if (el.tagName === 'OPTION') {
                el.textContent = on ? ar : was.text;
                return;
            }

            SWAPPABLE.forEach(function (attr) {
                if (attr in was) {
                    el.setAttribute(attr, on ? ar : was[attr]);
                }
            });
        });
    };

    var setState = function (on) {
        root.dataset.tr = on ? 'on' : 'off';

        document.querySelectorAll('[data-tr-toggle]').forEach(function (btn) {
            btn.setAttribute('aria-pressed', on ? 'true' : 'false');
        });

        swap(on);
    };

    remember();

    document.querySelectorAll('[data-tr-toggle]').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var on = root.dataset.tr !== 'on';
            setState(on);

            try {
                localStorage.setItem(KEY, on ? 'on' : 'off');
            } catch {
                /* Private browsing, or a full quota. The switch still works for this page;
                   it just will not be remembered next time. */
            }
        });
    });

    // The head already decided the state; the button and the attributes have to catch up.
    setState(root.dataset.tr === 'on');
})();