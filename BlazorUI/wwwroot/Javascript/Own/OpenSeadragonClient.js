
window.OpenSeadragonClient = {
    initDZI: function (viewerElement, tileSourcePaths, annotations) {
        this.viewer = OpenSeadragon({
            element: viewerElement,
            prefixUrl: "Javascript/dist/openseadragon-bin-2.4.2/images/",
            tileSources: tileSourcePaths,
            sequenceMode: true,
            showNavigator: true,
            ajaxWithCredentials: true
        });

        this.annotations = annotations;

        this.anno = OpenSeadragon.Annotorious(this.viewer, {
            readOnly: true,
            locale: 'auto'
        });

        this.anno.setAnnotations(this.annotations[0]);

        var _this = this;
        this.viewer.addHandler("page", function (e) {
            _this.anno.setAnnotations(_this.annotations[e.page]);
        });
    },
    addAnnotation: function () {
        //
    }
}