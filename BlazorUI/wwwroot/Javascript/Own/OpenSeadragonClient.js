
window.OpenSeadragonClient = {
    initDZI: function (viewerElement, tileSourcePaths, annotationPaths, isReadonly, dotnetHelper) {
        var _this = this;

        this.dotnetHelper = dotnetHelper;
        this.isReadonly = isReadonly;

        this.showAnnotations = false;
        window.onresize = function () {
            if ((viewerElement.offsetWidth < 1000) && (_this.showAnnotations === true)) {
                _this.setDisplayAnnotations(viewerElement.offsetWidth > 1000);
                _this.showAnnotations = false;
            }

            // Max controls width
            if (_this.viewer) {
                _this.viewer.controls[0].element.parentElement.parentElement.style.maxWidth =
                    `calc(100vw - ${_this.viewer.navigator.element.offsetWidth}px)`;
            }
        }

        // OpenSeadragon

        this.viewer = OpenSeadragon({
            element: viewerElement,
            prefixUrl: "Images/",
            tileSources: tileSourcePaths,
            animationTime: 0.25,
            sequenceMode: true,
            showNavigator: true,
            showRotationControl: true,
            ajaxWithCredentials: true,
            crossOriginPolicy: "Anonymous",

            navigatorDisplayRegionColor: "#2AFB6E",
            navigatorBackground: document.getElementsByClassName("full-page-container")[0].style.backgroundColor

            // toolbar: 'osdToolbarDiv',

            //zoomInButton: 'zoomin',
            //zoomOutButton: 'zoomout',
            //homeButton: 'gohome',
            //fullPageButton: 'togglefullpage',
            //rotateLeftButton: 'rotateleft',
            //rotateRightButton: 'rotateright',
            //previousButton: 'previous',
            //nextButton: 'next'
        });

        // Max controls width

        this.viewer.controls[0].element.parentElement.parentElement.style.maxWidth =
            `calc(100vw - ${this.viewer.navigator.element.offsetWidth}px)`;

        // Custom buttons

        window.addCustomOSDButtons(this.viewer);

        // Screenshots

        this.viewer.screenshot();

        // Annotorious

        this.annotationPaths = annotationPaths;
        this.initializeAnno();
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
        this.showAnnotations = displayAnnotations;
        this.anno.destroy();
        this.initializeAnno();
    },
    initializeAnno: function () {
        let _this = this;

        this.anno = OpenSeadragon.Annotorious(this.viewer, {
            readOnly: this.isReadonly || !this.showAnnotations,
            locale: 'auto'
        });

        if (this.showAnnotations) {
            this.anno.loadAnnotations(this.annotationPaths[this.viewer.currentPage()]).then(function () {
                _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
            }, function () {
                _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
            });
        } else {
            this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
        }

        this.viewer.addHandler("page", function (e) {
            _this.anno.selectAnnotation();
            _this.anno.setAnnotations([]);

            _this.dotnetHelper.invokeMethodAsync('NotifyPageChangedTo', e.page);

            if (_this.showAnnotations) {
                _this.anno.selectAnnotation();

                _this.anno.loadAnnotations(_this.annotationPaths[e.page]).then(function () {
                    _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
                }, function () {
                    _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
                });
            } else {
                _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', []);
            }
        });

        this.anno.on('createAnnotation', function (annotation) {
            _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });
        this.anno.on('deleteAnnotation', function (annotation) {
            _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });
        this.anno.on('updateAnnotation', function (annotation, previous) {
            _this.dotnetHelper.invokeMethodAsync('NotifyAnnotationsChanged', _this.anno.getAnnotations());
        });
    }
}