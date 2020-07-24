using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorUI.Services.APIClients;

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

            await builder.Build().RunAsync();
        }
    }
}
