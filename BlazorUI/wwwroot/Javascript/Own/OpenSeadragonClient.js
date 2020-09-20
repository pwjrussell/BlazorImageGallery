
window.OpenSeadragonClient = {
    initDZI: function (viewerElement, tileSourcePaths, annotationPaths, isReadonly, dotnetHelper) {
        var _this = this;

        this.dotnetHelper = dotnetHelper;
        this.isReadonly = isReadonly;

        this.showAnnotations = false;
        window.onresize = function () {
            if ((viewerElement.offsetWidth < 1000) && (_this.showAnnotations === true)) {
                _this.setDisplayAnnotations(false);
            }

            // Max controls width
            if (_this.viewer) {
                _this.viewer.controls[0].element.parentElement.parentElement.style.maxWidth =
                    viewerElement.offsetWidth > 1000 ? "100vw" : "30px";
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
            navigatorBackground: document.getElementsByClassName("full-page-container")[0].style.backgroundColor,

            maxZoomPixelRatio: 2.5

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
            this.viewer.element.offsetWidth > 1000 ? "100vw" : "30px";

        // Custom buttons

        window.addCustomOSDButtons(this.viewer);

        // Screenshots

        this.viewer.screenshot();

        // Annotorious

        this.annotationPaths = annotationPaths;
        this.initializeAnno();

        // Scalebar

        this.getCORS(this.pixelsPerMeterPathFromAnnotationsPath(annotationPaths[0]), function () {
            var ppm = parseFloat(this.responseText);
            _this.viewer.scalebar({
                pixelsPerMeter: ppm,
                backgroundColor: "rgba(255, 255, 255, 0.5)",
                stayInsideImage: false
            });
            document.getElementById("pixelsPerMeterInput").value = ppm;
        });
    },
    getCORS: function (url, success) {
        var xhr = new XMLHttpRequest();
        if (!('withCredentials' in xhr)) xhr = new XDomainRequest(); // fix IE8/9
        xhr.open('GET', url);
        xhr.addEventListener("load", success);
        xhr.send();
        return xhr;
    },
    pixelsPerMeterPathFromAnnotationsPath: function (annotationsPath) {
        return `${annotationsPath.substring(0, annotationsPath.lastIndexOf("/"))}/PixelsPerMeter.txt`;
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
        this.showAnnotations = this.viewer.element.offsetWidth > 1000 ? displayAnnotations : false;
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

            // Reset filters
            document.getElementById("OSDFilteringResetButton").click();

            // Scalebar
            _this.getCORS(_this.pixelsPerMeterPathFromAnnotationsPath(_this.annotationPaths[e.page]), function () {
                var ppm = parseFloat(this.responseText)
                _this.viewer.scalebar({
                    pixelsPerMeter: ppm
                });
                document.getElementById("pixelsPerMeterInput").value = ppm;
            });
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