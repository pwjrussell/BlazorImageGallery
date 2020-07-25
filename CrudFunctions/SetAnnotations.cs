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

namespace CrudFunctions
{
    public static class SetAnnotations
    {
        [FunctionName("SetAnnotations")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            ILogger log)
        {
            string name = req.Query["name"];
            string category = req.Query["category"];
            if (name.Contains('/') || name.Contains('\\') ||
                category.Contains('/') || category.Contains('\\'))
            {
                throw new ArgumentException("The image name or category contained a slash.");
            }

            W3CWebAnnotationModel[] annotations = JsonConvert.DeserializeObject<W3CWebAnnotationModel[]>(
                await req.ReadAsStringAsync());

            CloudBlockBlob annotationsBlob = container.GetBlockBlobReference($"{category}/{name}/annotations.w3c.json");
            annotationsBlob.Properties.ContentType = "text/json";

            await annotationsBlob.UploadTextAsync(JsonConvert.SerializeObject(annotations));

            return new OkResult();
        }
    }
}