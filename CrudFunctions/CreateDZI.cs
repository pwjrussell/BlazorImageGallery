using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Blob;
using NetVips;
using System.IO.Compression;

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

                string fileExtension = imageName.Substring(imageName.LastIndexOf('.'));

                Stream blobStream = await stagedImage.OpenReadAsync();
                Image sourceImage = Image.NewFromStream(blobStream);

                byte[] zipBuffer = sourceImage.DzsaveBuffer(
                    basename: imageName.Substring(0, imageName.LastIndexOf('.')), 
                    suffix: fileExtension, 
                    tileSize: tileSize,
                    overlap: overlap,
                    container: Enums.ForeignDzContainer.Zip);

                using Stream zipStream = new MemoryStream(zipBuffer);
                using ZipArchive zipArchive = new ZipArchive(zipStream);

                foreach (ZipArchiveEntry entry in zipArchive.Entries)
                {
                    using Stream stream = entry.Open();
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(
                        $"{category}/{entry.FullName.Replace(".dzi", ".xml")}");
                    blockBlob.Properties.ContentType = 
                        entry.Name.Contains(".xml") ? "text/xml" : stagedImage.Properties.ContentType;

                    await blockBlob.UploadFromStreamAsync(stream);
                }
            }
            catch (Exception e)
            {
                log.LogError(e.ToString());
            }

            await stagedImage.DeleteAsync();
        }
    }
}