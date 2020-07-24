var DZIBuilder = /** @class */ (function () {
    function DZIBuilder(options) {
        this.file = options.file;
        this.folderName = this.file.name.substring(0, this.file.name.lastIndexOf('.')) + "_files";
        this.fileExtension = this.file.name.substring(this.file.name.lastIndexOf('.'));
        this.tileSize = options.tileSize;
        this.overlap = options.overlap;
        this.onTileBuilt = options.onTileBuilt;
        this.onXMLBuilt = options.onXMLBuilt;
        this.onComplete = options.onComplete;
        var _this = this;
        var reader = new FileReader();
        reader.onload = function () {
            var image = document.createElement('img');
            image.onload = function () {
                /* IE8 fix since it has no naturalWidth and naturalHeight */
                _this.imageWidth = Object.prototype.hasOwnProperty.call(image, 'naturalWidth') ? image.naturalWidth : image.width;
                _this.imageHeight = Object.prototype.hasOwnProperty.call(image, 'naturalHeight') ? image.naturalHeight : image.height;
                _this.build(image);
            };
            image.src = reader.result;
        };
        reader.readAsDataURL(this.file);
    }
    DZIBuilder.prototype.build = function (image) {
        var currentWidth = this.imageWidth;
        var currentHeight = this.imageHeight;
        var indexOfCurrentLevel = Math.ceil(Math.log(Math.max(currentWidth, currentHeight)) / Math.log(2));
        var bigCanvas = document.createElement("canvas");
        var bigContext = bigCanvas.getContext("2d");
        bigCanvas.width = currentWidth;
        bigCanvas.height = currentHeight;
        bigContext.drawImage(image, 0, 0);
        this.buildTilesOnLevel({
            index: indexOfCurrentLevel--,
            context2D: bigContext,
            width: currentWidth,
            height: currentHeight
        });
        // We build smaller levels until both width and height become
        // 1 pixel wide.
        var smallCanvas;
        var smallContext;
        while (currentWidth > 1 || currentHeight > 1) {
            currentWidth = Math.ceil(currentWidth / 2);
            currentHeight = Math.ceil(currentHeight / 2);
            smallCanvas = document.createElement("canvas");
            smallContext = smallCanvas.getContext("2d");
            smallCanvas.width = currentWidth;
            smallCanvas.height = currentHeight;
            smallContext.drawImage(bigCanvas, 0, 0, currentWidth, currentHeight);
            this.buildTilesOnLevel({
                index: indexOfCurrentLevel--,
                context2D: smallContext,
                width: currentWidth,
                height: currentHeight
            });
            bigCanvas = smallCanvas;
            bigContext = smallContext;
        }
        this.onXMLBuilt(this.buildXML());
        this.onComplete();
    };
    DZIBuilder.prototype.buildTilesOnLevel = function (level) {
        var sourceContext = level.context2D;
        var columns = Math.ceil(level.width / this.tileSize);
        var rows = Math.ceil(level.height / this.tileSize);
        var tileCanvas;
        var sliceWidth;
        var sliceHeight;
        var prefix = this.folderName + "/" + level.index + "/";
        for (var i = 0; i < columns; i++) {
            for (var j = 0; j < rows; j++) {
                sliceWidth = ((i == columns - 1) ? level.width - i * this.tileSize : this.tileSize + 1) + ((i > 0) ? 1 : 0);
                sliceHeight = ((j == rows - 1) ? level.height - j * this.tileSize : this.tileSize + 1) + ((j > 0) ? 1 : 0);
                tileCanvas = document.createElement('canvas');
                tileCanvas.width = sliceWidth;
                tileCanvas.height = sliceHeight;
                tileCanvas.getContext('2d').drawImage(sourceContext.canvas, (i == 0) ? 0 : i * this.tileSize - this.overlap, (j == 0) ? 0 : j * this.tileSize - this.overlap, sliceWidth, sliceHeight, 0, 0, sliceWidth, sliceHeight);
                this.onTileBuilt({
                    name: "" + prefix + i + "_" + j + this.fileExtension,
                    canvas: tileCanvas
                });
            }
        }
    };
    DZIBuilder.prototype.buildXML = function () {
        var xmlString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Image xmlns=\"http://schemas.microsoft.com/deepzoom/2009\"\n    Format=\"" + this.fileExtension.substring(1) + "\" \n    Overlap=\"" + this.overlap + "\" \n    ServerFormat=\"Default\"\n    TileSize=\"" + this.tileSize + "\" >\n<Size Height=\"" + this.imageHeight + "\" \n        Width=\"" + this.imageWidth + "\"/>\n</Image>";
        return xmlString;
    };
    return DZIBuilder;
}());
//# sourceMappingURL=DZIBuilder.js.map