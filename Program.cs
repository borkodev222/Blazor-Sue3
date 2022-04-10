using Blazored.Toast;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sue3.Web.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            //addscoped is only instantiated per client. We use only when needed to reduce port hogging
            //and general resources on the server
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://sue3testapi.azurewebsites.net/api/"), Timeout=TimeSpan.FromMinutes(5) });
            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:1823/api/"), Timeout=TimeSpan.FromMinutes(5) });
            //Dependancy injection which allows us to use these things for the Blazor project
            //singleton because it persists across time etc
            builder.Services.AddSingleton<Data.SueApiService>();
            builder.Services.AddSingleton<Data.ModelHandlerService>();
            builder.Services.AddBlazoredToast();
            await builder.Build().RunAsync();
        }
    }
}
