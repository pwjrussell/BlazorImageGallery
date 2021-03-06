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
using CrudFunctions.Services;
using System.Security.Claims;
using HttpRequestModelsClassLibrary;

namespace CrudFunctions
{
    public class BeginCreateDZI
    {
        private readonly AADJwtService _JwtService;

        public BeginCreateDZI(AADJwtService jwtService)
        {
            _JwtService = jwtService;
        }

        [FunctionName("BeginCreateDZI")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "BeginCreateDZI/{category}/{name}")] HttpRequest req,
            [Blob("staged-images/{category}/{name}")] CloudBlockBlob image,
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

                if (name.Contains('/') || name.Contains('\\') ||
                    category.Contains('/') || category.Contains('\\'))
                {
                    throw new ArgumentException("The file name or category contained a slash.");
                }
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