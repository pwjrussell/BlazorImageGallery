using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using System.Collections;
using System;
using System.Web.Http;
using System.Linq;
using Microsoft.AspNetCore.Http.Extensions;

namespace CrudFunctions
{
    public static class ListDZIDirectoryURIs
    {
        [FunctionName("ListDZIDirectoryURIs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Blob("dzi-images", FileAccess.Read)] CloudBlobContainer container,
            ILogger log)
        {
            try
            {
                string category = req.Query["category"];
                if ((category != null) && (category.Contains('/') || category.Contains('\\')))
                {
                    throw new ArgumentException("The category contained a slash.");
                }

                IEnumerable directories = container
                    .ListBlobs((category == null) ? "" : $"{category}/")
                    .OfType<CloudBlobDirectory>()
                    .Select(dir => dir.Uri.AbsoluteUri);

                return new OkObjectResult(directories);
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
                return new InternalServerErrorResult();
            }
        }
    }
}
