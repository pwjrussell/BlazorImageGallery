
window.OpenSeadragonClient = {
    initDZI: function (viewerElement, tileSourcePaths, annotationPaths, dotnetHelper) {
        var _this = this;

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
            readOnly: false,
            locale: 'auto'
        });

        this.anno.loadAnnotations(this.annotationPaths[0]);
        dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', this.anno.getAnnotations());

        
        this.viewer.addHandler("page", function (e) {
            for (let a of _this.anno.getAnnotations()) {
                _this.anno.removeAnnotation(a);
            }

            _this.anno.loadAnnotations(_this.annotationPaths[e.page]);
            dotnetHelper.invokeMethodAsync('NotifyPageChangedTo', e.page);
            dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });

        this.anno.on('createAnnotation', function (annotation) {
            dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());;
        });
        this.anno.on('deleteAnnotation', function (annotation) {
            dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });
        this.anno.on('updateAnnotation', function (annotation, previous) {
            dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });
    },
    panTo: function (x, y) {
        this.viewer.viewport.panTo(new OpenSeadragon.Point(x, y));
    },
    getAnnotationsOnCurrentPage: function () {
        return this.anno.getAnnotations();
    }
}