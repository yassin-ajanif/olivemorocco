/**
 * ZAHO Gestion — sidebar accordion + mobile drawer.
 */
(() => {
  function initSidebar() {
    document.querySelectorAll('.dash-section-toggle').forEach(btn => {
      btn.addEventListener('click', () => {
        const section = btn.closest('.dash-section');
        if (!section?.querySelector('.dash-section-items')) return;
        const isOpen = section.classList.toggle('open');
        btn.setAttribute('aria-expanded', isOpen);
      });
    });
  }

  function initMobileNav() {
    const toggle = document.getElementById('dashNavToggle');
    const backdrop = document.getElementById('dashNavBackdrop');
    const nav = document.getElementById('dashNav');
    if (!toggle || !backdrop) return;

    const close = () => {
      document.body.classList.remove('dash-nav-open');
      toggle.setAttribute('aria-expanded', 'false');
      toggle.setAttribute('aria-label', 'Ouvrir le menu');
      backdrop.hidden = true;
    };

    const open = () => {
      document.body.classList.add('dash-nav-open');
      toggle.setAttribute('aria-expanded', 'true');
      toggle.setAttribute('aria-label', 'Fermer le menu');
      backdrop.hidden = false;
    };

    toggle.addEventListener('click', () => {
      if (document.body.classList.contains('dash-nav-open')) close();
      else open();
    });

    backdrop.addEventListener('click', close);

    document.addEventListener('keydown', e => {
      if (e.key === 'Escape') close();
    });

    nav?.querySelectorAll('a.dash-item').forEach(link => {
      link.addEventListener('click', close);
    });

    window.matchMedia('(min-width: 901px)').addEventListener('change', e => {
      if (e.matches) close();
    });
  }

  function init() {
    initSidebar();
    initMobileNav();
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', init);
  } else {
    init();
  }
})();
