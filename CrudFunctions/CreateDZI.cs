using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Blob;
using ImageToolsClassLibrary;
using HttpRequestModelsClassLibrary;

namespace CrudFunctions
{
    public static class CreateDZI
    {
        [FunctionName("CreateDZI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "CreateDZI")] HttpRequest req,
            [Blob("DZImages", FileAccess.Read)] CloudBlobContainer container,
            ILogger log)
        {
            CreateDZIRequest request = JsonConvert.DeserializeObject<CreateDZIRequest>(await req.ReadAsStringAsync());

            DZIBuilder.OnTileBuilt onTileBuilt = async (fileName, contentType, tileImageStream) => 
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.Properties.ContentType = contentType;
                await blockBlob.UploadFromStreamAsync(tileImageStream);
            };
            DZIBuilder.OnXMLBuilt onXMLBuilt = async (fileName, xml) =>
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.Properties.ContentType = "text/xml";
                await blockBlob.UploadTextAsync(xml);
            };

            await DZIBuilder.Build(
                request.ImageName,
                request.ImageStream,
                request.TileSize,
                request.Overlap,
                onTileBuilt,
                onXMLBuilt);

            return new OkResult();
        }
    }
}
