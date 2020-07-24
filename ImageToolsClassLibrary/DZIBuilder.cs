using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace ImageToolsClassLibrary
{
    public static class DZIBuilder
    {
        public delegate Task OnTileBuilt(string fileName, string contentType, Stream tileImageStream);
        public delegate Task OnXMLBuilt(string fileName, string xml);

        /// <summary>
        /// Builds the DZI image pyramid files and .xml metadata file in memory and passes them on to the callbacks provided.
        /// </summary>
        /// <param name="imageName">The name of the image file, including the file extension.</param>
        /// <param name="imageStream">The stream of the image file.</param>
        /// <param name="tileSize">The side length of the images generated in the image pyramid, excluding overlap.</param>
        /// <param name="overlap">The overlap of the images generated in the image pyramid.</param>
        /// <param name="onTileBuilt">The callback called when a tile in the image pyramid is built.</param>
        /// <param name="onXMLBuilt">The callback called when the .xml metadata file is built.</param>
        /// <returns>The task that builds the DZI image folder with the callbacks provided.</returns>
        public static async Task Build(
            string imageName,
            Stream imageStream,
            int tileSize,
            int overlap,
            OnTileBuilt onTileBuilt,
            OnXMLBuilt onXMLBuilt)
        {
            Bitmap imageBitmap = new Bitmap(imageStream);
            new FileExtensionContentTypeProvider().TryGetContentType(imageName, out string contentType);
            ImageFormat format = GetImageFormat(contentType);

            string folderName = $"{imageName.Substring(0, imageName.LastIndexOf('.'))}_files";
            string fileExtension = imageName.Substring(imageName.LastIndexOf('.'));

            await onXMLBuilt($"{folderName.Substring(0, folderName.Length - 6)}.xml", 
                BuildXML(fileExtension, overlap, tileSize, imageBitmap.Width, imageBitmap.Height));

            int indexOfCurrentLevel = (int)Math.Ceiling(Math.Log(Math.Max(imageBitmap.Width, imageBitmap.Height), 2));
            for (int i = indexOfCurrentLevel; i > -1; i--)
            {
                await BuildTilesOnLevel(imageBitmap, format, contentType, i, tileSize, overlap, folderName, fileExtension, onTileBuilt);
                imageBitmap = imageBitmap.ScaleBy(0.5);
            }
        }

        private static async Task BuildTilesOnLevel(
            Image levelImage,
            ImageFormat format,
            string contentType,
            int level,
            int tileSize,
            int overlap,
            string folderName,
            string fileExtension,
            OnTileBuilt onTileBuilt)
        {
            int columns = (int)Math.Ceiling((double)levelImage.Width / tileSize);
            int rows = (int)Math.Ceiling((double)levelImage.Height / tileSize);
            int sliceWidth;
            int sliceHeight;
            Rectangle destRect;
            Rectangle srcRect;
            string prefix = $"{folderName}/{level}/";

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    sliceWidth = ((i == columns - 1) ? levelImage.Width - i * tileSize : tileSize + overlap) + ((i > 0) ? overlap : 0);
                    sliceHeight = ((j == rows - 1) ? levelImage.Height - j * tileSize : tileSize + overlap) + ((j > 0) ? overlap : 0);

                    destRect = new Rectangle(0, 0, sliceWidth, sliceHeight);
                    srcRect = new Rectangle(
                        (i == 0) ? 0 : i * tileSize - overlap,
                        (j == 0) ? 0 : j * tileSize - overlap,
                        sliceWidth, sliceHeight);

                    using (Bitmap tileBitmap = new Bitmap(sliceWidth, sliceHeight))
                    using (Graphics graphics = Graphics.FromImage(tileBitmap))
                    using (MemoryStream stream = new MemoryStream())
                    {
                        graphics.DrawImage(levelImage, destRect, srcRect, GraphicsUnit.Pixel);
                        tileBitmap.Save(stream, format);
                        await onTileBuilt($"{prefix}{i}_{j}{fileExtension}", contentType, stream);
                    }
                }
            }
        }

        private static string BuildXML(string fileExtension, int overlap, int tileSize, int imageWidth, int imageHeight)
        {
            return $"<? xml version=\"1.0\" encoding=\"UTF-8\"?>\n<Image xmlns=\"http://schemas.microsoft.com/deepzoom/2009\" Format=\"{fileExtension.Substring(1)}\" Overlap=\"{overlap}\" ServerFormat=\"Default\" TileSize=\"{tileSize}\"><Size Height=\"{imageHeight}\" Width=\"{imageWidth}\"/></Image>";
        }

        private static ImageFormat GetImageFormat(string contentType)
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
