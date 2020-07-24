using BlazorInputFile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            _client.BaseAddress = new Uri("https://imagegalleryfunctions.azurewebsites.net/api/");
        }

        public async Task<HttpResponseMessage> PostCreateDZIAsync(IFileListEntry image, int tileSize, int overlap)
        {
            StreamContent content = new StreamContent(image.Data);
            content.Headers.ContentType = new MediaTypeHeaderValue(image.Type);

            return await _client.PostAsync(
                string.Format("CreateDZI?imagename={0}&tilesize={1}&overlap={2}", 
                    HttpUtility.UrlEncode(image.Name),
                    tileSize,
                    overlap
                ), content);
        }
    }
}
