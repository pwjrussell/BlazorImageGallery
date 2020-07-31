using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorUI.Services.APIClients;
using BlazorUI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;
using BlazorUI.Services.AuthorizationMessageHandlers;
using Blazored.LocalStorage;

namespace BlazorUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<UIService>();

            builder.Services.AddTransient(options => {
                return new AuthorizedAuthorizationMessageHandler(
                    options.GetRequiredService<IAccessTokenProvider>(),
                    options.GetRequiredService<NavigationManager>())
                {
                    InnerHandler = new HttpClientHandler()
                };
            });

            builder.Services.AddSingleton(sp => 
                new HttpClient() { BaseAddress = new System.Uri("https://dzigalleryfunctions.azurewebsites.net/api/") }
            );

            builder.Services.AddSingleton<AnonymousDZIClient>();
            builder.Services.AddTransient(options => {
                return new AdminDZIClient(
                    new HttpClient(options.GetRequiredService<AuthorizedAuthorizationMessageHandler>()) 
                    { BaseAddress = new System.Uri("https://dzigalleryfunctions.azurewebsites.net/api/") },
                    options.GetRequiredService<IAccessTokenProvider>());
            });

            builder.Services.AddSingleton<OpenSeadragonClient>();
            builder.Services.AddSingleton<QuillClient>();

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add(
                    "fca594e3-3b1d-460a-b21a-425b32473479/Annotations.Edit");
                options.ProviderOptions.DefaultAccessTokenScopes.Add(
                    "fca594e3-3b1d-460a-b21a-425b32473479/Images.Edit");
            });

            await builder.Build().RunAsync();
        }
    }
}
