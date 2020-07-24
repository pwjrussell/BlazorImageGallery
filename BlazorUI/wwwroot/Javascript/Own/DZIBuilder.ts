
interface IDZIBuilderOptions {
    file: File;
    tileSize: number;
    overlap: number;
    onTileBuilt: (tile: ITile) => void;
    onXMLBuilt: (xml: string) => void;
    onComplete: () => void;
}

interface ILevel {
    index: number;
    context2D: CanvasRenderingContext2D;
    width: number;
    height: number;
}

interface ITile {
    name: string;
    canvas: HTMLCanvasElement;
}

class DZIBuilder {
    private readonly file: File;
    private readonly folderName: string;
    private readonly fileExtension: string;

    private readonly tileSize: number;
    private readonly overlap: number;

    private readonly onTileBuilt: (tile: ITile) => void;
    private readonly onXMLBuilt: (xml: string) => void;
    private readonly onComplete: () => void;

    private imageWidth: number;
    private imageHeight: number;

    public constructor(options: IDZIBuilderOptions) {
        this.file = options.file;

        this.folderName = `${this.file.name.substring(0, this.file.name.lastIndexOf('.'))}_files`;
        this.fileExtension = this.file.name.substring(this.file.name.lastIndexOf('.'));

        this.tileSize = options.tileSize;
        this.overlap = options.overlap;

        this.onTileBuilt = options.onTileBuilt;
        this.onXMLBuilt = options.onXMLBuilt;
        this.onComplete = options.onComplete;

        let _this = this;
        let reader = new FileReader();

        reader.onload = function () {
            let image = document.createElement('img');
            image.onload = function () {
                /* IE8 fix since it has no naturalWidth and naturalHeight */
                _this.imageWidth = Object.prototype.hasOwnProperty.call(image, 'naturalWidth') ? image.naturalWidth : image.width;
                _this.imageHeight = Object.prototype.hasOwnProperty.call(image, 'naturalHeight') ? image.naturalHeight : image.height;

                _this.build(image);
            };
            image.src = <string>reader.result;
        };
        reader.readAsDataURL(this.file);
    }

    public build(image: HTMLImageElement) {
        let currentWidth: number = this.imageWidth;
        let currentHeight: number = this.imageHeight;
        let indexOfCurrentLevel = Math.ceil(Math.log(Math.max(currentWidth, currentHeight)) / Math.log(2));

        let bigCanvas: HTMLCanvasElement = document.createElement("canvas");
        let bigContext: CanvasRenderingContext2D = bigCanvas.getContext("2d");

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
        let smallCanvas: HTMLCanvasElement;
        let smallContext: CanvasRenderingContext2D;
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
    }

    private buildTilesOnLevel(level: ILevel) {
        let sourceContext: CanvasRenderingContext2D = level.context2D;
        let columns: number = Math.ceil(level.width / this.tileSize);
        let rows: number = Math.ceil(level.height / this.tileSize);

        let tileCanvas: HTMLCanvasElement;

        let sliceWidth: number;
        let sliceHeight: number;

        let prefix: string = `${this.folderName}/${level.index}/`;

        for (let i = 0; i < columns; i++) {
            for (let j = 0; j < rows; j++) {
                sliceWidth = ((i == columns - 1) ? level.width - i * this.tileSize : this.tileSize + 1) + ((i > 0) ? 1 : 0);
                sliceHeight = ((j == rows - 1) ? level.height - j * this.tileSize : this.tileSize + 1) + ((j > 0) ? 1 : 0);

                tileCanvas = document.createElement('canvas');
                tileCanvas.width = sliceWidth;
                tileCanvas.height = sliceHeight;

                tileCanvas.getContext('2d').drawImage(
                    sourceContext.canvas,
                    (i == 0) ? 0 : i * this.tileSize - this.overlap, (j == 0) ? 0 : j * this.tileSize - this.overlap,
                    sliceWidth, sliceHeight,
                    0, 0,
                    sliceWidth, sliceHeight);

                this.onTileBuilt({
                    name: `${prefix}${i}_${j}${this.fileExtension}`,
                    canvas: tileCanvas
                });
            }
        }
    }
    private buildXML() {
        let xmlString: string =
`<?xml version="1.0" encoding="UTF-8"?>
<Image xmlns="http://schemas.microsoft.com/deepzoom/2009"
    Format="${this.fileExtension.substring(1)}" 
    Overlap="${this.overlap}" 
    ServerFormat="Default"
    TileSize="${this.tileSize}" >
<Size Height="${this.imageHeight}" 
        Width="${this.imageWidth}"/>
</Image>`;
        return xmlString;
    }
}


