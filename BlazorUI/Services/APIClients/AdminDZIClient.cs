using BlazorInputFile;
using HttpRequestModelsClassLibrary;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace BlazorUI.Services.APIClients
{
    public class AdminDZIClient
    {
        private readonly HttpClient _client;
        private readonly IAccessTokenProvider _tokenProvider;
        private readonly ILogger<AdminDZIClient> _logger;
        private readonly string _storageBaseAddress = "https://dzigallerystorage.blob.core.windows.net/";
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNameCaseInsensitive = true 
        };

        public AdminDZIClient(
            HttpClient client, 
            IAccessTokenProvider tokenProvider,
            ILogger<AdminDZIClient> logger)
        {
            _client = client;
            _tokenProvider = tokenProvider;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> PostCreateDZIAsync(
            IFileListEntry image, 
            string category, 
            int tileSize, 
            int overlap)
        {
            try
            {
                using var content = new StreamContent(image.Data);
                content.Headers.ContentType = new MediaTypeHeaderValue(image.Type);

                // Add retry logic for upload
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var response = await _client.PostAsync(
                            $"BeginCreateDZI/{category}/{image.Name}?tilesize={tileSize}&overlap={overlap}",
                            content);

                        if (response.IsSuccessStatusCode)
                            return response;

                        _logger.LogWarning($"Attempt {i + 1} failed. Status: {response.StatusCode}");
                        await Task.Delay(1000 * (i + 1)); // Exponential backoff
                    }
                    catch (Exception ex) when (i < 2) // Only catch if we have retries left
                    {
                        _logger.LogError($"Upload attempt {i + 1} failed: {ex.Message}");
                        await Task.Delay(1000 * (i + 1));
                    }
                }

                throw new Exception("Failed to upload after 3 attempts");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PostCreateDZIAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> DeleteDZIAsync(string category, string name)
        {
            try
            {
                var response = await _client.DeleteAsync($"DeleteDZI/{category}/{name}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to delete DZI: {response.StatusCode}");
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in DeleteDZIAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostAnnotationsAsync(
            string category, 
            string imageName, 
            W3CWebAnnotationModel[] annotations)
        {
            try
            {
                // Add retry logic
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var response = await _client.PostAsJsonAsync(
                            $"SetAnnotations/{category}/{imageName}", 
                            annotations);

                        if (response.IsSuccessStatusCode)
                        {
                            // Verify the save
                            var verifyResponse = await GetAnnotationsAsync(category, imageName);
                            if (verifyResponse.IsSuccessStatusCode)
                            {
                                var savedAnnotations = await verifyResponse.Content
                                    .ReadFromJsonAsync<W3CWebAnnotationModel[]>(_jsonOptions);
                                
                                if (AnnotationsMatch(annotations, savedAnnotations))
                                {
                                    return response;
                                }
                                
                                _logger.LogWarning("Annotation verification failed - saved data doesn't match");
                            }
                        }

                        _logger.LogWarning($"Attempt {i + 1} failed. Status: {response.StatusCode}");
                        await Task.Delay(1000 * (i + 1));
                    }
                    catch (Exception ex) when (i < 2)
                    {
                        _logger.LogError($"Save attempt {i + 1} failed: {ex.Message}");
                        await Task.Delay(1000 * (i + 1));
                    }
                }

                throw new Exception("Failed to save annotations after 3 attempts");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PostAnnotationsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> GetAnnotationsAsync(string category, string imageName)
        {
            try
            {
                return await _client.GetAsync($"GetAnnotations/{category}/{imageName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in GetAnnotationsAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> PostDescriptionAsync(
            string category, 
            string name, 
            MarkupString markup)
        {
            try
            {
                var response = await _client.PostAsJsonAsync(
                    $"SetDescription/{category}/{name}", 
                    markup);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to save description: {response.StatusCode}");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PostDescriptionAsync: {ex.Message}");
                throw;
            }
        }

        private bool AnnotationsMatch(
            W3CWebAnnotationModel[] annotations1, 
            W3CWebAnnotationModel[] annotations2)
        {
            if (annotations1 == null || annotations2 == null)
                return annotations1 == annotations2;

            if (annotations1.Length != annotations2.Length)
                return false;

            // Compare serialized versions for deep equality
            var serialized1 = JsonSerializer.Serialize(annotations1, _jsonOptions);
            var serialized2 = JsonSerializer.Serialize(annotations2, _jsonOptions);

            return serialized1 == serialized2;
        }
    }
}
