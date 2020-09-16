
window.OpenSeadragonFilteringClient = {
    clearFilters: function () {
        window.OpenSeadragonClient.viewer.setFilterOptions(null);
    },
    applyFilters: function () {
        var filterOptions = {
            filters: {
                processors: []
            }
        };

        try {
            var thresholdingValue = document.getElementById("thresholdingInput").value;
            filterOptions.filters.processors.push(OpenSeadragon.Filters.THRESHOLDING(thresholdingValue));
        } catch (e) { console.log(e); }
        try {
            var brightnessValue = document.getElementById("brightnessInput").value;
            filterOptions.filters.processors.push(OpenSeadragon.Filters.BRIGHTNESS(brightnessValue));
        } catch (e) { console.log(e); }
        try {
            var contrastValue = document.getElementById("contrastInput").value;
            filterOptions.filters.processors.push(OpenSeadragon.Filters.CONTRAST(contrastValue));
        } catch (e) { console.log(e); }
        try {
            var gammaValue = document.getElementById("gammaInput").value;
            filterOptions.filters.processors.push(OpenSeadragon.Filters.GAMMA(gammaValue));
        } catch (e) { console.log(e); }
        try {
            var grayscaleValue = document.getElementById("grayScaleInput").checked;
            if (grayscaleValue) {
                filterOptions.filters.processors.push(OpenSeadragon.Filters.GREYSCALE());
            }
        } catch (e) { console.log(e); }
        try {
            var invertValue = document.getElementById("invertInput").checked;
            if (invertValue) {
                filterOptions.filters.processors.push(OpenSeadragon.Filters.INVERT());
            }
        } catch (e) { console.log(e); }

        window.OpenSeadragonClient.viewer.setFilterOptions(filterOptions);
    }
};