using BlazorInputFile;
using HttpRequestModelsClassLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
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
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly string _storageBaseAddress = "https://dzigallerystorage.blob.core.windows.net/";

        public AdminDZIClient(HttpClient client, IAccessTokenProvider tokenProvider)
        {
            _client = client;
            _tokenProvider = tokenProvider;
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

            //var tokenResult = await _tokenProvider.RequestAccessToken(
            //    new AccessTokenRequestOptions() 
            //    { 
            //        Scopes = new[] { "https://storage.azure.com/user_impersonation" }
            //    });

            //if (tokenResult.TryGetToken(out AccessToken token))
            //{
            //    StorageCredentials credentials = new StorageCredentials(new TokenCredential(token.Value));

            //    CloudBlockBlob blob = new CloudBlockBlob(
            //        new Uri($"{_storageBaseAddress}staged-images/{category}/{image.Name}"), credentials);
            //    blob.Properties.ContentType = image.Type;

            //    await blob.UploadFromStreamAsync(image.Data);
            //}
        }

        public async Task<HttpResponseMessage> DeleteDZIAsync(string category, string name) 
        {
            return await _client.DeleteAsync($"DeleteDZI/{category}/{name}");
        }

        public async Task<HttpResponseMessage> PostAnnotationsAsync(
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
