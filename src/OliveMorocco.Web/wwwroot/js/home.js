(() => {
  const nav = document.getElementById('mainNav');
  if (nav) {
    window.addEventListener(
      'scroll',
      () => nav.classList.toggle('scrolled', window.scrollY > 60),
      { passive: true }
    );
  }

  const revealEls = document.querySelectorAll('.reveal');
  const stepEls = document.querySelectorAll('.p-step');
  const io = new IntersectionObserver(
    (entries) => entries.forEach((e) => { if (e.isIntersecting) e.target.classList.add('in'); }),
    { threshold: 0.15 }
  );
  revealEls.forEach((el) => io.observe(el));
  stepEls.forEach((el) => io.observe(el));

  // Variety hover cards. The CSS reveals them on :hover / :focus-within, but the badge is
  // a sibling of the card inside .shop-line, and touch devices have no hover at all — so
  // wire the toggle in JS for pointer:coarse and keyboard users.
  const coarser = window.matchMedia('(pointer: coarse)');
  document.querySelectorAll('.variete-badge').forEach((badge) => {
    const card = document.getElementById(badge.getAttribute('aria-describedby'));
    if (!card) return;
    const line = badge.closest('.shop-line, .shop-detail-variete');
    if (!line) return;

    const show = () => {
      if (line.classList.contains('shop-detail-variete')) return; // CSS already handles it
      card.style.opacity = '1';
      card.style.visibility = 'visible';
      card.style.transform = 'none';
    };
    const hide = () => {
      if (line.classList.contains('shop-detail-variete')) return;
      card.style.opacity = '';
      card.style.visibility = '';
      card.style.transform = '';
    };

    if (coarser.matches) {
      // Tap to toggle, tap elsewhere to dismiss.
      badge.addEventListener('click', (e) => {
        e.preventDefault();
        const open = card.style.visibility === 'visible';
        document.querySelectorAll('.variete-card').forEach(hide);
        if (!open) show();
      });
      document.addEventListener('click', (e) => {
        if (!line.contains(e.target)) hide();
      });
    } else {
      // Mouse: keep the CSS hover, but hold the card open while it is being read.
      badge.addEventListener('focus', show);
      badge.addEventListener('blur', hide);
      card.addEventListener('mouseenter', show);
      card.addEventListener('mouseleave', hide);
    }
  });

  const path = document.getElementById('processPath');
  if (path) {
    const pathIO = new IntersectionObserver(
      (entries) => entries.forEach((e) => { if (e.isIntersecting) path.classList.add('in'); }),
      { threshold: 0.3 }
    );
    pathIO.observe(path);
  }

  const form = document.getElementById('form');
  const formMsg = document.getElementById('formMsg');
  if (form && formMsg) {
    form.addEventListener('submit', (e) => {
      e.preventDefault();
      formMsg.textContent = 'Merci — votre demande a été notée. Nous revenons vers vous rapidement.';
      form.reset();
    });
  }
})();
