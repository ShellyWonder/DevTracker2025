//site.js
//allows rendering before scroll when usings anchor tags; 
window.scrollToFragment = (fragment) => {
    if (!fragment) return;

    const id = fragment.startsWith("#")
        ? fragment.substring(1)
        : fragment;

    const element = document.getElementById(id);

    if (element) {
        element.scrollIntoView({
            behavior: "smooth",
            block: "start"
        });
    }
};