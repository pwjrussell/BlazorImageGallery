using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections;
using Microsoft.Azure.Storage.Blob;
using System.Linq;
using System.Web.Http;
using CrudFunctions.Services;
using System.Security.Claims;

namespace CrudFunctions
{
    public class ListDZICategoryURIs
    {
        private readonly AADJwtService _JwtService;

        public ListDZICategoryURIs(AADJwtService jwtService)
        {
            _JwtService = jwtService;
        }

        [FunctionName("ListDZICategoryURIs")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            [Blob("dzi-images", FileAccess.Read)] CloudBlobContainer container,
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

                IEnumerable directories = container
                    .ListBlobs()
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
