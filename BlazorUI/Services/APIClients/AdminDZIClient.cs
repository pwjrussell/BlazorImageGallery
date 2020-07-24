using BlazorInputFile;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
            _client.Timeout = TimeSpan.FromDays(1);
        }

        public async Task<HttpResponseMessage> PostCreateDZIAsync(IFileListEntry image, int tileSize, int overlap)
        {
            Console.WriteLine("Started creating request.");

            StreamContent content = new StreamContent(image.Data);
            content.Headers.ContentType = new MediaTypeHeaderValue(image.Type);

            Console.WriteLine("Posting request");

            return await _client.PostAsync(
                string.Format("BeginCreateDZI/{0}?tilesize={1}&overlap={2}", 
                    HttpUtility.UrlEncode(image.Name),
                    tileSize,
                    overlap), 
                content);
        }
    }
}
