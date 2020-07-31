
window.OpenSeadragonClient = {
    initDZI: function (viewerElement, tileSourcePaths, annotationPaths, isReadonly, dotnetHelper) {
        var _this = this;

        this.dotnetHelper = dotnetHelper;

        this.showAnnotations = window.innerWidth > 1000;
        window.onresize = function () {
            if (_this.showAnnotations != (window.innerWidth > 1000)) {
                _this.setDisplayAnnotations(window.innerWidth > 1000);
            }
            _this.showAnnotations = window.innerWidth > 1000;
        }

        this.viewer = OpenSeadragon({
            element: viewerElement,
            // prefixUrl: "Javascript/dist/openseadragon-bin-2.4.2/images/",
            tileSources: tileSourcePaths,
            animationTime: 0.25,
            sequenceMode: true,
            showNavigator: true,
            showRotationControl: true,
            ajaxWithCredentials: true,
            crossOriginPolicy: "Anonymous",

            // toolbar: 'osdToolbarDiv',

            zoomInButton: 'zoomin',
            zoomOutButton: 'zoomout',
            homeButton: 'gohome',
            fullPageButton: 'togglefullpage',
            rotateLeftButton: 'rotateleft',
            rotateRightButton: 'rotateright',
            previousButton: 'previous',
            nextButton: 'next'
        });

        // Screenshots

        this.viewer.screenshot();

        // Annotorious

        this.annotationPaths = annotationPaths;

        this.anno = OpenSeadragon.Annotorious(this.viewer, {
            readOnly: isReadonly,
            locale: 'auto'
        });

        if (this.showAnnotations) {
            this.anno.loadAnnotations(this.annotationPaths[0]).then(function () {
                dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
            }, function () {
                dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
            });
        }
        
        this.viewer.addHandler("page", function (e) {
            _this.anno.selectAnnotation();
            for (let a of _this.anno.getAnnotations()) {
                _this.anno.removeAnnotation(a);
            }
            dotnetHelper.invokeMethodAsync('NotifyPageChangedTo', e.page);

            if (_this.showAnnotations) {
                _this.anno.selectAnnotation();

                _this.anno.loadAnnotations(_this.annotationPaths[e.page]).then(function () {
                    dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
                }, function () {
                    dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
                });
            } else {
                dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
            }
        });

        this.anno.on('createAnnotation', function (annotation) {
            dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });
        this.anno.on('deleteAnnotation', function (annotation) {
            dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });
        this.anno.on('updateAnnotation', function (annotation, previous) {
            dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });
    },
    getIfViewerExists: function () {
        return (this.viewer != null);
    },
    destroy: function () {
        this.viewer.destroy();
        this.viewer = null;

        this.anno.destroy();
        this.anno = null;
    },
    panTo: function (x, y) {
        this.viewer.viewport.panTo(new OpenSeadragon.Point(x, y));
    },
    annoPanTo: function (id) {
        this.anno.selectAnnotation(id);
        this.anno.panTo(id);
    },
    getAnnotationsOnCurrentPage: function () {
        return this.anno.getAnnotations();
    },
    goToPage: function (index) {
        this.viewer.goToPage(index);
    },
    setDisplayAnnotations: function (displayAnnotations) {
        if (displayAnnotations) {
            this.anno.selectAnnotation();
            let _this = this;

            this.anno.loadAnnotations(this.annotationPaths[this.viewer.currentPage()]).then(function () {
                _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
            }, function () {
                _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
            });
        } else {
            this.anno.selectAnnotation();
            this.anno.setAnnotations([]);
            this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
        }
    }
}