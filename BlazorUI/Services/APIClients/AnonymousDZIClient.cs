using HttpRequestModelsClassLibrary;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace BlazorUI.Services.APIClients
{
    public class AnonymousDZIClient
    {
        private readonly HttpClient _client;
        private readonly string _storageBaseAddress = "https://dzigallerystorage.blob.core.windows.net/";
        private readonly string _annotationFileName = "annotations.w3c.json";

        public AnonymousDZIClient(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets either the URIs of either the categories (if no category is provided) or images within a category.
        /// </summary>
        /// <param name="category">The category containing the images whose URIs are queried.</param>
        /// <returns>The URIs of either the categories or images.</returns>
        public async Task<string[]> GetDZIDirectoryURIsAsync(string category = "")
        {
            if (category.Contains('/') || category.Contains('\\'))
            {
                return new string[0];
            }
            return await _client.GetFromJsonAsync<string[]>(
                $"ListDZIDirectoryURIs{((category == "") ? "" : $"?category={category}")}");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="category"></param>
        /// <param name="name">The name of the file without the file extension.</param>
        /// <returns></returns>
        public async Task<MarkupString> GetDescriptionAsync(string category, string name)
        {
            try
            {
                HttpResponseMessage response = await _client.GetAsync($"{_storageBaseAddress}dzi-images/{category}/{name}/description.txt");
                return (MarkupString)await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
                return new MarkupString();
            }
        }
        /// <summary>
        /// Gets the path to the .xml tileSource metadata file given the path to the containing directory.
        /// </summary>
        /// <param name="directoryURI">The path to the containing directory</param>
        /// <returns>The path to the .xml tileSource metadata file</returns>
        public string GetTileSourceURIFromDirectoryURI(string directoryURI)
        {
            int startOfPrefix = directoryURI[0..^1].LastIndexOf('/') + 1;
            return $"{directoryURI}{directoryURI[startOfPrefix..^1]}.xml";
        }
        /// <summary>
        /// Gets the URI of the annotations .json file for a specified image directory URI.
        /// </summary>
        /// <param name="directoryURI">The URI of the image directory in question.</param>
        /// <returns>The URI of the annotations .json file for the image in question.</returns>
        public string GetAnnotationsURIFromDirectoryURI(string directoryURI)
        {
           return $"{directoryURI}{_annotationFileName}";
        }
    }
}
