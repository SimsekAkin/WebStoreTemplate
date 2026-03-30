//After 3seconds the message will dissappear

document.addEventListener("DOMContentLoaded", function () {
	var alerts = document.querySelectorAll(".alert[data-auto-dismiss]");

	alerts.forEach(function (alert) {
		// Read timeout from HTML attribute, default to 3 seconds.
		var delay = Number(alert.getAttribute("data-auto-dismiss"));
		var timeout = Number.isNaN(delay) ? 3000 : delay;// Set timeout to hide the alert

		setTimeout(function () {
			alert.style.transition = "opacity 0.4s ease";// Start fade out transition
			alert.style.opacity = "0";

			setTimeout(function () {
				alert.style.display = "none";
			}, 400);
		}, timeout);
	});

	// Toggle between light and dark mode and store preference.
	var themeToggle = document.getElementById("themeToggle");
	if (themeToggle) {
		var root = document.documentElement;
		var currentTheme = root.getAttribute("data-theme") || "light";
		themeToggle.textContent = currentTheme === "dark" ? "Light Mode" : "Dark Mode";

		themeToggle.addEventListener("click", function () {
			var activeTheme = root.getAttribute("data-theme") || "light";
			var nextTheme = activeTheme === "dark" ? "light" : "dark";

			root.setAttribute("data-theme", nextTheme);
			localStorage.setItem("theme", nextTheme);
			themeToggle.textContent = nextTheme === "dark" ? "Light Mode" : "Dark Mode";
		});
	}
});
