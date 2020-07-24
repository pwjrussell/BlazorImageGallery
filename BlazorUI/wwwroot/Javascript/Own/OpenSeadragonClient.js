
window.OpenSeadragonClient = {
    initDZI: function (viewerElement, tileSourcePaths) {
        var viewer = OpenSeadragon({
            element: viewerElement,
            prefixUrl: "Javascript/dist/openseadragon-bin-2.4.2/images/",
            tileSources: tileSourcePaths,
            sequenceMode: true,
            showNavigator: true,
            ajaxWithCredentials: true
        });
    }
}