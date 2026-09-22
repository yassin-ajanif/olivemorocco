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
