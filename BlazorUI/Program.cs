using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorUI.Services.APIClients;
using BlazorUI.Services;

namespace BlazorUI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddSingleton(sp => new HttpClient());

            builder.Services.AddSingleton<AnonymousDZIClient>();
            builder.Services.AddSingleton<AdminDZIClient>();

            builder.Services.AddSingleton<OpenSeadragonClient>();

            await builder.Build().RunAsync();
        }
    }
}
