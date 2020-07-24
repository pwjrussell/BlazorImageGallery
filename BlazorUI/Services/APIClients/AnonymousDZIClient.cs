using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorUI.Services.APIClients
{
    public class AnonymousDZIClient
    {
        private readonly HttpClient _client;
        private readonly string _functionsBaseAddress = "https://imagegalleryfunctions.azurewebsites.net/api/";
        private readonly string _storageBaseAddress = "https://blazorimggallerystorage.blob.core.windows.net/dzi-images/";

        public AnonymousDZIClient(HttpClient client)
        {
            _client = client;
            _client.BaseAddress = new Uri("");
        }

        public async Task<string[]> GetDZIPrefixes()
        {
            return await _client.GetFromJsonAsync<string[]>($"{_functionsBaseAddress}ListDZIDirectories");
        }

        public string GetDZIXMLAddressFromPrefix(string prefix)
        {
            return $"{_storageBaseAddress}{prefix}{prefix[0..^1]}.xml";
        }
    }
}
