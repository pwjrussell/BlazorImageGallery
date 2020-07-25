
window.OpenSeadragonClient = {
    initDZI: function (viewerElement, tileSourcePaths, annotationPaths) {
        this.viewer = OpenSeadragon({
            element: viewerElement,
            prefixUrl: "Javascript/dist/openseadragon-bin-2.4.2/images/",
            tileSources: tileSourcePaths,
            sequenceMode: true,
            showNavigator: true,
            ajaxWithCredentials: true
        });

        this.annotationPaths = annotationPaths;

        this.anno = OpenSeadragon.Annotorious(this.viewer, {
            readOnly: true,
            locale: 'auto'
        });

        this.anno.loadAnnotations(this.annotationPaths[0]);

        var _this = this;
        this.viewer.addHandler("page", function (e) {
            _this.anno.loadAnnotations(_this.annotationPaths[e.page]);
        });
    },
    addAnnotation: function () {
        //
    }
}