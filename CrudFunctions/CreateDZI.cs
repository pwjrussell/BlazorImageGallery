using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using ImageToolsClassLibrary;
using System.Web.Http;

namespace CrudFunctions
{
    public static class CreateDZI
    {
        [FunctionName("CreateDZI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            ILogger log)
        {
            try
            {
                string imageName = req.Query["imagename"];
                int tileSize = Convert.ToInt32(req.Query["tilesize"]);
                int overlap = Convert.ToInt32(req.Query["overlap"]);

                Stream imageStream = req.Body;
                string dirName = imageName.Substring(0, imageName.LastIndexOf('.'));

                DZIBuilder.OnTileBuilt onTileBuilt = async (fileName, tileImageStream) =>
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{dirName}/{fileName}");
                    blockBlob.Properties.ContentType = req.ContentType;
                    await blockBlob.UploadFromStreamAsync(tileImageStream);
                };
                DZIBuilder.OnXMLBuilt onXMLBuilt = async (fileName, xml) =>
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{dirName}/{fileName}");
                    blockBlob.Properties.ContentType = "text/xml";
                    await blockBlob.UploadTextAsync(xml);
                };

                await DZIBuilder.Build(
                    imageName,
                    imageStream,
                    req.ContentType,
                    tileSize,
                    overlap,
                    onTileBuilt,
                    onXMLBuilt);

                return new OkResult();
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new InternalServerErrorResult();
            }
        }
    }
}
