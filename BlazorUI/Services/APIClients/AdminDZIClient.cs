using BlazorInputFile;
using HttpRequestModelsClassLibrary;
using Microsoft.AspNetCore.Components;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;

namespace BlazorUI.Services.APIClients
{
    public class AdminDZIClient
    {
        private readonly HttpClient _client;

        public AdminDZIClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<HttpResponseMessage> PostCreateDZIAsync(IFileListEntry image, string category, int tileSize, int overlap)
        {
            Console.WriteLine("Started creating request.");

            StreamContent content = new StreamContent(image.Data);
            content.Headers.ContentType = new MediaTypeHeaderValue(image.Type);

            Console.WriteLine("Posting request");

            return await _client.PostAsync(
                string.Format("BeginCreateDZI/{0}/{1}?tilesize={2}&overlap={3}",
                    category,
                    image.Name,
                    tileSize,
                    overlap), 
                content);
        }

        public async Task<HttpResponseMessage> DeleteDZIAsync(string category, string name) 
        {
            return await _client.DeleteAsync($"DeleteDZI/{category}/{name}");
        }

        public async Task<HttpResponseMessage> PostAnnotations(
            string category, string imageName, W3CWebAnnotationModel[] annotations)
        {
            return await _client.PostAsJsonAsync($"SetAnnotations/{category}/{imageName}", annotations);
        }

        public async Task<HttpResponseMessage> PostDescriptionAsync(string category, string name, MarkupString markup)
        {
            return await _client.PostAsJsonAsync($"SetDescription/{category}/{name}", markup);
        }
    }
}
