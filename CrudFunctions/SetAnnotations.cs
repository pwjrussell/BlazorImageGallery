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
using HttpRequestModelsClassLibrary;
using System.Web.Http;

namespace CrudFunctions
{
    public static class SetAnnotations
    {
        [FunctionName("SetAnnotations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SetAnnotations/{category}/{name}")] HttpRequest req,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            string category,
            string name,
            ILogger log)
        {
            try
            {
                if (name.Contains('/') || name.Contains('\\') ||
                    category.Contains('/') || category.Contains('\\'))
                {
                    throw new ArgumentException("The image name or category contained a slash.");
                }

                SetAnnotationRequestModel request = JsonConvert.DeserializeObject<SetAnnotationRequestModel>(
                    await req.ReadAsStringAsync());

                CloudBlockBlob pinsBlob = container.GetBlockBlobReference($"{category}/{name}/overlays.json");
                pinsBlob.Properties.ContentType = "text/json";
                await pinsBlob.UploadTextAsync(JsonConvert.SerializeObject(request.Pins));

                CloudBlockBlob annotationsBlob = container.GetBlockBlobReference($"{category}/{name}/annotations.w3c.json");
                annotationsBlob.Properties.ContentType = "text/json";
                await annotationsBlob.UploadTextAsync(JsonConvert.SerializeObject(request.Annotations));

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
