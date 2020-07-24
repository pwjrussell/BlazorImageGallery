using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorUI.Services.APIClients
{
    public class AnonymousDZIClient
    {
        private readonly HttpClient _client;
        private readonly string _functionsBaseAddress = "https://imagegalleryfunctions.azurewebsites.net/api/";

        public AnonymousDZIClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<string[]> GetDZIDirectoryURIs()
        {
            return await _client.GetFromJsonAsync<string[]>($"{_functionsBaseAddress}ListDZIDirectories");
        }

        public string GetTileSourcePathFromDirectoryURI(string directoryURI)
        {
            int startOfPrefix = directoryURI[0..^1].LastIndexOf('/') + 1;
            return $"{directoryURI}{directoryURI[startOfPrefix..^1]}.xml";
        }
    }
}
