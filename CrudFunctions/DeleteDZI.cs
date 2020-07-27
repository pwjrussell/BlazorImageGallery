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
using System.Linq;
using CrudFunctions.Services;
using System.Security.Claims;

namespace CrudFunctions
{
    public class DeleteDZI
    {
        private readonly AADJwtService _JwtService;

        public DeleteDZI(AADJwtService jwtService)
        {
            _JwtService = jwtService;
        }

        [FunctionName("DeleteDZI")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteDZI/{category}/{name}")] HttpRequest req,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            string category,
            string name,
            ILogger log)
        {
            try
            {
                try
                {
                    ClaimsPrincipal principal = await _JwtService.GetClaimsPrincipalAsync(req);

                    if (!principal.Identity.IsAuthenticated)
                    {
                        return new UnauthorizedResult();
                    }
                }
                catch (Exception e)
                {
                    log.LogError(e.ToString());
                    return new UnauthorizedResult();
                }

                if (category.Contains('/') || category.Contains('\\') ||
                    name.Contains('/') || name.Contains('\\'))
                {
                    throw new ArgumentException("The name or the category contained a slash.");
                }

                CloudBlockBlob[] blobs = container
                    .ListBlobs($"{category}/{name}/", true)
                    .OfType<CloudBlockBlob>().ToArray();

                foreach (CloudBlockBlob blob in blobs)
                {
                    try
                    {
                        await blob.DeleteIfExistsAsync();
                    }
                    catch (Exception e)
                    {
                        log.LogError(e.ToString());
                    }
                }

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
