using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using ImageToolsClassLibrary;
using System.Drawing;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using HttpRequestModelsClassLibrary;
using System.Collections.Generic;

namespace CrudFunctions
{
    public static class CreateDZIOrchestrator
    {
        [FunctionName("CreateDZIOrchestrator")]
        public static async Task Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            [Blob("staged-images", FileAccess.Read)] CloudBlobContainer stagingContainer,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            ILogger log)
        {
            DZICreationRequest request = context.GetInput<DZICreationRequest>();
            CloudBlockBlob stagedImage = stagingContainer.GetBlockBlobReference($"{request.Category}/{request.Name}");

            try
            {
                List<TileModel> tiles = await context.CallActivityAsync<List<TileModel>>("BuildDZITilesAndXML", request);

                int numberOfTilesInSegment = 100;
                int i = 0;
                while ((i + 1) * numberOfTilesInSegment < tiles.Count)
                {
                    await context.CallActivityAsync("BuildDZISegment", new TileSegmentCreationRequest() 
                    { 
                        Name = request.Name,
                        Category = request.Category,
                        FileExtension = request.Name.Substring(request.Name.LastIndexOf('.')),
                        Tiles = tiles.GetRange(i * numberOfTilesInSegment, numberOfTilesInSegment).ToArray()
                    });
                    i++;
                }
                await context.CallActivityAsync("BuildDZISegment", new TileSegmentCreationRequest()
                {
                    Name = request.Name,
                    Category = request.Category,
                    FileExtension = request.Name.Substring(request.Name.LastIndexOf('.')),
                    Tiles = tiles.GetRange(i * numberOfTilesInSegment, tiles.Count - i * numberOfTilesInSegment).ToArray()
                });

            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }

            // await stagedImage.DeleteIfExistsAsync();
        }
    }
}
