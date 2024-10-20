const editors = {};

window.editors = editors;

export function init(id, dotnetObjectRef, options) {
    require(['vs/editor/editor.main'], function() {
        const editorContainer = document.getElementById(id);
        const editor = monaco.editor.create(editorContainer, {
            ...options,
            theme: 'vs-dark'
        });
        
        
        editor.onDidChangeModelContent(function (e) {
            const content = editor.getValue();
            dotnetObjectRef.invokeMethodAsync('OnContentChanged', content);
        });
        
        const staticSuggestions = [
                        {
                            label: 'TestowaSugestia',
                            kind: monaco.languages.CompletionItemKind.Class,
                            insertText: 'TestowaSugestia',
                     
                        },
                        {
                            label: 'Metoda',
                            kind: monaco.languages.CompletionItemKind.Method,
                            insertText: 'Metoda',
                          
                        }
                    ];
        
        monaco.languages.registerCompletionItemProvider('razor', {
            provideCompletionItems: function(model, position) {
                // Provide IntelliSense items here
                // Get the word until the cursor position
                const wordInfo = model.getWordUntilPosition(position);
                const word = wordInfo.word.toLowerCase();
            console.log(wordInfo);
            
                // Filter the static suggestions based on the current word
                const suggestions = staticSuggestions
                  .filter(item => item.label.toLowerCase().startsWith(word))
                  .map(item => ({
                    label: item.label,
                    kind: item.kind,
                    insertText: item.insertText,
                    range: {
                      startLineNumber: position.lineNumber,
                      startColumn: wordInfo.startColumn,
                      endLineNumber: position.lineNumber,
                      endColumn: wordInfo.endColumn
                    }
                  }));
            
                // Return the filtered suggestions
                return { suggestions: suggestions };
            }
        });
        
        const resizeObserver = new ResizeObserver(() => {
            editor.layout(); // Adjust layout on resize
        });

        // Start observing the container for resizing
        resizeObserver.observe(editorContainer);
        
        editors[id] = {
            editor: editor,
            resizeObserver: resizeObserver
        }
    });
}

export function setContent(id, content) {
    if (editors?.[id]?.editor) {
        editors[id]?.editor?.setValue(content);
    }
}

export function dispose(id) {
    if (editors?.[id]) {
        editors[id]?.editor?.dispose();
        editors[id]?.resizeObserver?.disconnect();
        editors[id] = null;
    }
}