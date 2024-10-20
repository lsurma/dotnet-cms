window.updateUrl = (url) => {
    window.history.replaceState({}, document.title, url);
}