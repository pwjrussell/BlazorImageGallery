using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using HttpRequestModelsClassLibrary;
using System.Collections.Generic;
using ImageToolsClassLibrary;
using System.Drawing;

namespace CrudFunctions
{
    public static class BuildDZITilesAndXML
    {
        [FunctionName("BuildDZITilesAndXML")]
        public static async Task<List<TileModel>> Run(
            [ActivityTrigger] IDurableActivityContext context,
            [Blob("staged-images", FileAccess.Read)] CloudBlobContainer stagingContainer,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            ILogger log)
        {
            DZICreationRequest request = context.GetInput<DZICreationRequest>();
            CloudBlockBlob stagedImage = stagingContainer.GetBlockBlobReference($"{request.Category}/{request.Name}");

            try
            {
                Stream blobStream = await stagedImage.OpenReadAsync();
                Bitmap imageBitmap = new Bitmap(blobStream);
                string dirName = $"{request.Category}/{request.Name.Substring(0, request.Name.LastIndexOf('.'))}";

                DZIBuilder.OnXMLBuilt onXMLBuilt = async (fileName, xml) =>
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{dirName}/{fileName}");
                    blockBlob.Properties.ContentType = "text/xml";
                    await blockBlob.UploadTextAsync(xml);
                };

                List<TileModel> tiles = await DZIBuilder.Build(
                    imageBitmap.Width,
                    imageBitmap.Height,
                    request.Name,
                    request.TileSize,
                    request.Overlap,
                    onXMLBuilt);
                return tiles;
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new List<TileModel>();
            }
        }
    }
}