
window.QuillClient = {
    initQuill: function (toolbarElement, editorElement) {
        new Quill(editorElement, {
            modules: {
                toolbar: toolbarElement
            },
            theme: 'snow'
        });
    },
    setHTML: function (editorElement, markup) {
        var delta = editorElement.__quill.clipboard.convert(markup)
        editorElement.__quill.setContents(delta)
    },
    getHTML: function (editorElement) {
        return editorElement.__quill.root.innerHTML;
    }
};
