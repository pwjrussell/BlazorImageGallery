
window.addCustomOSDButtons = function (viewer) {
    var infoButton = new OpenSeadragon.Button({
        tooltip: "Information",
        srcRest: "Images/info_rest.png",
        srcGroup: "Images/info_grouphover.png",
        srcHover: "Images/info_hover.png",
        srcDown: "Images/info_pressed.png",
        onClick: function () {
            document.getElementById("infoButton").click();
        }
    });

    var adminButton = new OpenSeadragon.Button({
        tooltip: "Admin",
        srcRest: "Images/admin_rest.png",
        srcGroup: "Images/admin_grouphover.png",
        srcHover: "Images/admin_hover.png",
        srcDown: "Images/admin_pressed.png",
        onClick: function () {
            document.getElementById("adminButton").click();
        }
    });

    var darkModeButton = new OpenSeadragon.Button({
        tooltip: "Dark mode",
        srcRest: "Images/darkmode_rest.png",
        srcGroup: "Images/darkmode_grouphover.png",
        srcHover: "Images/darkmode_hover.png",
        srcDown: "Images/darkmode_pressed.png",
        onClick: function () {
            document.getElementById("darkModeButton").click();
        }
    });

    var bottomRightButtonGroup = new OpenSeadragon.ButtonGroup({
        buttons: [infoButton, adminButton, darkModeButton]
    });

    viewer.addControl(bottomRightButtonGroup.element, {
        anchor: OpenSeadragon.ControlAnchor.BOTTOM_RIGHT
    });

    var annotationsToggleButton = new OpenSeadragon.Button({
        tooltip: "Toggle annotations",
        srcRest: "Images/annotations_rest.png",
        srcGroup: "Images/annotations_grouphover.png",
        srcHover: "Images/annotations_hover.png",
        srcDown: "Images/annotations_pressed.png",
        onClick: function () {
            document.getElementById("annotationsToggleButton").click();
        }
    });
    viewer.buttons.buttons.push(annotationsToggleButton);
    viewer.buttons.element.appendChild(annotationsToggleButton.element);

    var annotationsUploadButton = new OpenSeadragon.Button({
        tooltip: "Upload annotations",
        srcRest: "Images/cloud_rest.png",
        srcGroup: "Images/cloud_grouphover.png",
        srcHover: "Images/cloud_hover.png",
        srcDown: "Images/cloud_pressed.png",
        onClick: function () {
            document.getElementById("annotationsUploadButton").click();
        }
    });
    viewer.buttons.buttons.push(annotationsUploadButton);
    viewer.buttons.element.appendChild(annotationsUploadButton.element);

    var filtersButton = new OpenSeadragon.Button({
        tooltip: "Apply filters to the image",
        srcRest: "Images/filters_grouphover.png",
        srcGroup: "Images/filters_grouphover.png",
        srcHover: "Images/filters_hover.png",
        srcDown: "Images/filters_pressed.png",
        onClick: function () {
            document.getElementById("filterButton").click();
        }
    });

    viewer.buttons.buttons.push(filtersButton);
    viewer.buttons.element.appendChild(filtersButton.element);

    viewer.addHandler('open', function () {
        try {
            if (location.href.includes("/admin")) {
                adminButton.element.style.display = "none";
                annotationsUploadButton.element.style.display = "inline-block";
                infoButton.element.style.display = "none";
            } else {
                adminButton.element.style.display = "inline-block";
                annotationsUploadButton.element.style.display = "none";
                infoButton.element.style.display = "inline-block";
            }
        } catch (e) { // IE fix
            adminButton.element.style.display = "inline-block";
            annotationsUploadButton.element.style.display = "none";
            infoButton.element.style.display = "inline-block";
        }
    });
};