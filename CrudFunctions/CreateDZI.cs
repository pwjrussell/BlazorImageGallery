using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using ImageToolsClassLibrary;

namespace CrudFunctions
{
    public static class CreateDZI
    {
        [FunctionName("CreateDZI")]
        public static async Task Run(
            [BlobTrigger("staged-images/{imageName}")] CloudBlockBlob stagedImage,
            [Blob("dzi-images", FileAccess.Write)] CloudBlobContainer container,
            string imageName,
            ILogger log)
        {
            try
            {
                int tileSize = Convert.ToInt32(stagedImage.Metadata["tilesize"]);
                int overlap = Convert.ToInt32(stagedImage.Metadata["overlap"]);

                Stream imageStream = new MemoryStream();
                await stagedImage.DownloadToStreamAsync(imageStream);

                string dirName = imageName.Substring(0, imageName.LastIndexOf('.'));

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
                    imageStream,
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
