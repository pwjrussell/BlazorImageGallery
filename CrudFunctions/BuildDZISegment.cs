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

                ImageFormat format = DZIBuilder.GetImageFormat(stagedImage.Properties.ContentType);

                Bitmap imageBitmap;
                Stream blobStream = await stagedImage.OpenReadAsync();
                imageBitmap = new Bitmap(blobStream);

                int l = (int)Math.Ceiling(Math.Log(Math.Max(imageBitmap.Width, imageBitmap.Height), 2));
                foreach (int level in request.TileSegment.Keys.OrderByDescending(l => l))
                {
                    while (l > level)
                    {
                        // width = (int)Math.Round(0.5 * width, MidpointRounding.AwayFromZero);
                        // height = (int)Math.Round(0.5 * height, MidpointRounding.AwayFromZero);

                        imageBitmap = imageBitmap.ScaleBy(0.5);

                        l--;
                    }

                    foreach (TileModel tile in request.TileSegment[level])
                    {
                        Rectangle destRect = new Rectangle(0, 0, tile.TileRect.Width, tile.TileRect.Height);

                        using Bitmap tileBitmap = new Bitmap(tile.TileRect.Width, tile.TileRect.Height);
                        using Graphics graphics = Graphics.FromImage(tileBitmap);
                        graphics.DrawImage(imageBitmap, destRect, tile.TileRect, GraphicsUnit.Pixel);

                        CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{dirName}/{level}/{tile.Column}_{tile.Row}{fileExtension}");
                        blockBlob.Properties.ContentType = stagedImage.Properties.ContentType;

                        using Stream stream = await blockBlob.OpenWriteAsync();
                        tileBitmap.Save(stream, format);
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