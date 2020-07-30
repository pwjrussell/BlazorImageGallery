using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageToolsClassLibrary
{
    public static class DZIBuilder
    {
        public delegate Task OnTileBuilt(string fileName, Stream tileImageStream);
        public delegate Task OnXMLBuilt(string fileName, string xml);

        public static async Task<List<TileModel>> Build(
            int width,
            int height,
            string imageName,
            int tileSize,
            int overlap,
            OnXMLBuilt onXMLBuilt)
        {
            string folderName = $"{imageName.Substring(0, imageName.LastIndexOf('.'))}_files";
            string fileExtension = imageName.Substring(imageName.LastIndexOf('.'));

            await onXMLBuilt($"{folderName.Substring(0, folderName.Length - 6)}.xml", 
                BuildXML(fileExtension, overlap, tileSize, width, height));

            int indexOfCurrentLevel = (int)Math.Ceiling(Math.Log(Math.Max(width, height), 2));
            List<TileModel> tiles = new List<TileModel>();
            for (int i = indexOfCurrentLevel; i > -1; i--)
            {
                tiles.AddRange(await BuildTilesOnLevel(width, height, i, tileSize, overlap));

                width = (int)Math.Round(0.5 * width, MidpointRounding.AwayFromZero);
                height = (int)Math.Round(0.5 * height, MidpointRounding.AwayFromZero);
            }
            return tiles;
        }
        private static async Task<TileModel[]> BuildTilesOnLevel(
            int width,
            int height,
            int level,
            int tileSize,
            int overlap)
        {
            int columns = (int)Math.Ceiling((double)width / tileSize);
            int rows = (int)Math.Ceiling((double)height / tileSize);
            int sliceWidth;
            int sliceHeight;

            TileModel[] tiles = new TileModel[rows * columns];

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    sliceWidth = ((i == columns - 1) ? width - i * tileSize : tileSize + overlap) + ((i > 0) ? overlap : 0);
                    sliceHeight = ((j == rows - 1) ? height - j * tileSize : tileSize + overlap) + ((j > 0) ? overlap : 0);

                    tiles[i * rows + j] = new TileModel()
                    {
                        Level = level,
                        LevelWidth = width,
                        LevelHeight = height,
                        Column = i,
                        Row = j,
                        TileRect = new Rectangle(
                            (i == 0) ? 0 : i * tileSize - overlap,
                            (j == 0) ? 0 : j * tileSize - overlap,
                            sliceWidth, sliceHeight)
                    };
                }
            }
            return tiles;
        }

        private static string BuildXML(string fileExtension, int overlap, int tileSize, int imageWidth, int imageHeight)
        {
            return $"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Image xmlns=\"http://schemas.microsoft.com/deepzoom/2009\" Format=\"{fileExtension.Substring(1)}\" Overlap=\"{overlap}\" ServerFormat=\"Default\" TileSize=\"{tileSize}\"><Size Height=\"{imageHeight}\" Width=\"{imageWidth}\"/></Image>";
        }

        public static ImageFormat GetImageFormat(string contentType)
        {
            switch (contentType)
            {
                case "image/bmp":
                    return ImageFormat.Bmp;
                case "image/emf":
                    return ImageFormat.Emf;
                case "image/gif":
                    return ImageFormat.Gif;
                case "image/x-icon":
                    return ImageFormat.Icon;
                case "image/jpeg":
                    return ImageFormat.Jpeg;
                case "image/png":
                    return ImageFormat.Png;
                case "image/tiff":
                    return ImageFormat.Tiff;
                case "image/wmf":
                    return ImageFormat.Wmf;
                default:
                    throw new ArgumentException("The file format provided is not supported.");
            }
        }
    }
}
