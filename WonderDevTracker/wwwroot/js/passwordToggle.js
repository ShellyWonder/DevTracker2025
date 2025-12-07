//**passwordToggle.js */

//Toggles the password "eye" icon
//to hide/reveal password on RegisterByInvite.razor
//& Register.razor
function togglePassword(id, btn) {
    const input = document.getElementById(id);
    if (!input) return;

    const isPassword = input.type === "password";
    input.type = isPassword ? "text" : "password";

    const icon = btn.querySelector(".material-icons");
    if (icon) {
        icon.textContent = isPassword ? "visibility_off" : "visibility";
    }

    btn.setAttribute("aria-pressed", (!isPassword).toString());
    btn.setAttribute("aria-label", isPassword ? "Hide password" : "Show password");
}


