
window.OpenSeadragonClient = {
    initDZI: function (viewerElement, tileSourcePath) {
        OpenSeadragon({
            element: viewerElement,
            prefixUrl: "Javascript/dist/openseadragon-bin-2.4.2/images/",
            tileSources: tileSourcePath,
            ajaxWithCredentials: true
        });
    }
}