using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.Storage.Blob;
using HttpRequestModelsClassLibrary;
using ImageToolsClassLibrary;
using System.Drawing;
using System.Linq;
using System.Drawing.Imaging;

namespace CrudFunctions
{
    public static class BuildDZISegment
    {
        [FunctionName("BuildDZISegment")]
        public static async Task Run(
            [ActivityTrigger] IDurableActivityContext context,
            [Blob("staged-images", FileAccess.Read)] CloudBlobContainer stagingContainer,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            ILogger log)
        {
            try
            {
                TileSegmentCreationRequest request = context.GetInput<TileSegmentCreationRequest>();
                CloudBlockBlob stagedImage = stagingContainer.GetBlockBlobReference($"{request.Category}/{request.Name}");
                await stagedImage.FetchAttributesAsync();

                string dirName = $"{request.Category}/{request.Name.Substring(0, request.Name.LastIndexOf('.'))}/{request.Name.Substring(0, request.Name.LastIndexOf('.'))}_files";
                string fileExtension = request.Name.Substring(request.Name.LastIndexOf('.'));

                int level = request.Tiles[0].Level;
                Bitmap imageBitmap;
                Rectangle destRect;
                ImageFormat format = DZIBuilder.GetImageFormat(stagedImage.Properties.ContentType);

                using (Stream blobStream = await stagedImage.OpenReadAsync())
                using (Image sourceImage = Image.FromStream(blobStream))
                {
                    imageBitmap = new Bitmap(request.Tiles[0].LevelWidth, request.Tiles[0].LevelHeight);
                    using Graphics g = Graphics.FromImage(imageBitmap);
                    g.DrawImage(sourceImage, 0, 0, request.Tiles[0].LevelWidth, request.Tiles[0].LevelHeight);
                }

                foreach (TileModel tile in request.Tiles)
                {
                    if (tile.Level < level)
                    {
                        level--;
                        imageBitmap = new Bitmap(imageBitmap, tile.LevelWidth, tile.LevelHeight);
                    }

                    destRect = new Rectangle(0, 0, tile.TileRect.Width, tile.TileRect.Height);

                    using (Bitmap tileBitmap = new Bitmap(tile.TileRect.Width, tile.TileRect.Height))
                    using (Graphics graphics = Graphics.FromImage(tileBitmap))
                    using (MemoryStream stream = new MemoryStream())
                    {
                        graphics.DrawImage(imageBitmap, destRect, tile.TileRect, GraphicsUnit.Pixel);
                        tileBitmap.Save(stream, format);
                        stream.Seek(0, SeekOrigin.Begin);

                        CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{dirName}/{tile.Level}/{tile.Column}_{tile.Row}{fileExtension}");
                        blockBlob.Properties.ContentType = stagedImage.Properties.ContentType;
                        await blockBlob.UploadFromStreamAsync(stream);
                    }
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }
        }
    }
}