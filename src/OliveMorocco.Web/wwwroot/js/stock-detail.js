/**
 * Stock detail — collapsible manual adjustment panel.
 */
(() => {
  const panel = document.getElementById('stockAjustementPanel');
  if (!panel) return;

  const toggle = panel.querySelector('.stock-ajustement-toggle');
  const body = panel.querySelector('.stock-ajustement-body');
  const arrow = panel.querySelector('.stock-ajustement-toggle-arrow');
  if (!toggle || !body) return;

  toggle.addEventListener('click', () => {
    const isOpen = panel.classList.toggle('is-open');
    toggle.setAttribute('aria-expanded', isOpen ? 'true' : 'false');
    body.hidden = !isOpen;
    if (arrow) arrow.textContent = isOpen ? '▼' : '▶';
  });
})();
