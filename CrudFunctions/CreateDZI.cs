using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using ImageToolsClassLibrary;
using System.Drawing;

namespace CrudFunctions
{
    public static class CreateDZI
    {
        [FunctionName("CreateDZI")]
        public static async Task Run(
            [BlobTrigger("staged-images/{category}/{imageName}")] CloudBlockBlob stagedImage,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            string category,
            string imageName,
            ILogger log)
        {
            try
            {
                int tileSize;
                int overlap;
                try
                {
                    tileSize = Convert.ToInt32(stagedImage.Metadata["tilesize"]);
                    overlap = Convert.ToInt32(stagedImage.Metadata["overlap"]);

                    if (tileSize == 0)
                    {
                        throw new ArgumentException("The tile size was 0.");
                    }
                }
                catch (Exception e)
                {
                    log.LogError(e.ToString());
                    tileSize = 512;
                    overlap = 1;
                }


                Stream blobStream = await stagedImage.OpenReadAsync();
                Bitmap imageBitmap = new Bitmap(blobStream);

                string dirName = $"{category}/{imageName.Substring(0, imageName.LastIndexOf('.'))}";

                DZIBuilder.OnTileBuilt onTileBuilt = async (fileName, tileImageStream) =>
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{dirName}/{fileName}");
                    blockBlob.Properties.ContentType = stagedImage.Properties.ContentType;
                    await blockBlob.UploadFromStreamAsync(tileImageStream);
                };
                DZIBuilder.OnXMLBuilt onXMLBuilt = async (fileName, xml) =>
                {
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{dirName}/{fileName}");
                    blockBlob.Properties.ContentType = "text/xml";
                    await blockBlob.UploadTextAsync(xml);
                };

                await DZIBuilder.Build(
                    imageName,
                    imageBitmap,
                    stagedImage.Properties.ContentType,
                    tileSize,
                    overlap,
                    onTileBuilt,
                    onXMLBuilt);
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }

            await stagedImage.DeleteAsync();
        }
    }
}
