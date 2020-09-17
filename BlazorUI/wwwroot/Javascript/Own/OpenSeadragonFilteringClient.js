
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
            if (document.getElementById("thresholdingNumberInput").disabled === false) {
                var thresholdingValue = document.getElementById("thresholdingNumberInput").value;
                filterOptions.filters.processors.push(OpenSeadragon.Filters.THRESHOLDING(thresholdingValue));
            }
        } catch (e) { console.log(e); }
        try {
            if (document.getElementById("brightnessNumberInput").disabled === false) {
                var brightnessValue = document.getElementById("brightnessNumberInput").value;
                filterOptions.filters.processors.push(OpenSeadragon.Filters.BRIGHTNESS(brightnessValue));
            }
        } catch (e) { console.log(e); }
        try {
            if (document.getElementById("contrastNumberInput").disabled === false) {
                var contrastValue = document.getElementById("contrastNumberInput").value;
                filterOptions.filters.processors.push(OpenSeadragon.Filters.CONTRAST(contrastValue));
            }
        } catch (e) { console.log(e); }
        try {
            if (document.getElementById("gammaNumberInput").disabled === false) {
                var gammaValue = document.getElementById("gammaNumberInput").value;
                filterOptions.filters.processors.push(OpenSeadragon.Filters.GAMMA(gammaValue));
            }
        } catch (e) { console.log(e); }
        try {
            if (document.getElementById("grayScaleInput").checked) {
                filterOptions.filters.processors.push(OpenSeadragon.Filters.GREYSCALE());
            }
        } catch (e) { console.log(e); }
        try {
            if (document.getElementById("invertInput").checked) {
                filterOptions.filters.processors.push(OpenSeadragon.Filters.INVERT());
            }
        } catch (e) { console.log(e); }

        window.OpenSeadragonClient.viewer.setFilterOptions(filterOptions);
    }
};