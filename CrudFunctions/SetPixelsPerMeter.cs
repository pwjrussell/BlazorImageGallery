using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CrudFunctions.Services;
using System.Security.Claims;
using Microsoft.Azure.Storage.Blob;
using System.Web.Http;

namespace CrudFunctions
{
    public class SetPixelsPerMeter
    {
        private readonly AADJwtService _JwtService;

        public SetPixelsPerMeter(AADJwtService jwtService)
        {
            _JwtService = jwtService;
        }

        [FunctionName("SetPixelsPerMeter")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SetPixelsPerMeter/{category}/{name}")] HttpRequest req,
            [Blob("dzi-images/{category}/{name}/PixelsPerMeter.txt", FileAccess.Write)] CloudBlockBlob blob,
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

                double pixelsPerMeter = Convert.ToDouble(await req.ReadAsStringAsync());
                blob.Properties.ContentType = "text/plain";
                await blob.UploadTextAsync(pixelsPerMeter.ToString());

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
