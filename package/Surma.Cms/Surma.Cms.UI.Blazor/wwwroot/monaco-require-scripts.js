document.addEventListener('DOMContentLoaded', (event) => {
    require.config({ paths: { 'vs': 'https://cdn.jsdelivr.net/npm/monaco-editor@0.51.0/min/vs' }});
    require(['vs/editor/editor.main'], function() {
        require(['vs/basic-languages/razor/razor'], function() {});
    });
});