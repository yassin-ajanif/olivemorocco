/**
 * ZAHO Gestion — sidebar accordion toggles (server-rendered pages).
 */
(() => {
  function initSidebar() {
    document.querySelectorAll('.dash-section-toggle').forEach(btn => {
      btn.addEventListener('click', () => {
        const section = btn.closest('.dash-section');
        if (!section?.querySelector('.dash-section-items')) return;
        const isOpen = section.classList.toggle('open');
        btn.setAttribute('aria-expanded', isOpen);
        const arrow = btn.querySelector('.arrow');
        if (arrow) arrow.textContent = isOpen ? '▼' : '▶';
      });
    });
  }

  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', initSidebar);
  } else {
    initSidebar();
  }
})();
