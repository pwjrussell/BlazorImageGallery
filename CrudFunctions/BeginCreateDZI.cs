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
using System.Web.Http;

namespace CrudFunctions
{
    public static class BeginCreateDZI
    {
        [FunctionName("BeginCreateDZI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "BeginCreateDZI/{name}")] HttpRequest req,
            [Blob("staged-images/{name}", FileAccess.Write)] CloudBlockBlob image,
            ILogger log)
        {
            try
            {
                int tileSize = Convert.ToInt32(req.Query["tilesize"]);
                int overlap = Convert.ToInt32(req.Query["overlap"]);

                image.Properties.ContentType = req.ContentType;
                image.Metadata["tilesize"] = tileSize.ToString();
                image.Metadata["overlap"] = overlap.ToString();

                await image.UploadFromStreamAsync(req.Body);
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
