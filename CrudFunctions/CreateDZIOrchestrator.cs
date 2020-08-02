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
                Dictionary<int, TileModel[]> tiles = await context.CallActivityAsync<Dictionary<int, TileModel[]>>(
                    "BuildDZITilesAndXML", request);

                int numberOfTilesInSegment = 500;
                // int numOfCalls = (int)Math.Ceiling((double)tiles.Count / numberOfTilesInSegment);
                // Task[] tasks = new Task[numOfCalls];
                List<Task> tasks = new List<Task>();

                int i1 = 0;
                int i2 = 0;
                Dictionary<int, TileModel[]> tileSegment = new Dictionary<int, TileModel[]>();
                foreach (int level in tiles.Keys)
                {
                    while (tileSegment.Values.Count + tiles[level][i1..].Length > numberOfTilesInSegment)
                    {
                        i2 = numberOfTilesInSegment - tileSegment.Values.Count;
                        tileSegment[level] = tiles[level][i1..i2];

                        tasks.Add(context.CallActivityAsync("BuildDZISegment", new TileSegmentCreationRequest() 
                        { 
                            Name = request.Name,
                            Category = request.Category,
                            TileSegment = tileSegment
                        }));

                        tileSegment = new Dictionary<int, TileModel[]>();
                        i1 = i2;
                    }
                    tileSegment[level] = tiles[level][i1..];
                    i1 = 0;
                }
                if (tileSegment.Count > 0)
                {
                    tasks.Add(context.CallActivityAsync("BuildDZISegment", new TileSegmentCreationRequest()
                    {
                        Name = request.Name,
                        Category = request.Category,
                        TileSegment = tileSegment
                    }));
                }

                //for (int i = 0; i < numOfCalls; i++)
                //{
                //    tasks[i] = context.CallActivityAsync("BuildDZISegment", new TileSegmentCreationRequest()
                //    {
                //        Name = request.Name,
                //        Category = request.Category,
                //        FileExtension = request.Name.Substring(request.Name.LastIndexOf('.')),
                //        TileSegment = tiles.GetRange(
                //            i * numberOfTilesInSegment,
                //            (i == numOfCalls - 1) ? tiles.Count - i * numberOfTilesInSegment : numberOfTilesInSegment)
                //        .ToArray()
                //    });
                //}

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }

            // await stagedImage.DeleteIfExistsAsync();
        }
    }
}
