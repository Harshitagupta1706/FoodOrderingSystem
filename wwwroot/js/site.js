document.addEventListener("DOMContentLoaded", () => {
  document.querySelectorAll("form").forEach(form => {
    form.addEventListener("submit", () => {
      const submit = form.querySelector('button[type="submit"], button:not([type])');
      if (submit && !submit.dataset.noDisable) {
        // Avoid double-submit on checkout while keeping the browser's normal validation.
        if (form.checkValidity()) {
          submit.dataset.originalText = submit.textContent || "";
          submit.disabled = true;
          submit.textContent = "Please wait…";
          window.setTimeout(() => {
            submit.disabled = false;
            submit.textContent = submit.dataset.originalText;
          }, 8000);
        }
      }
    });
  });
});
