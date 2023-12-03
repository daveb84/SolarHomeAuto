using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.WebServer.ClientApp.PageServices;
using SolarHomeAuto.WebServer.ClientApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<PageServiceHttpClient>();
builder.Services.AddScoped<IAuthPageService, HttpAuthPageService>();
builder.Services.AddScoped<IMonitoringPageService, HttpMonitoringPageService>();
builder.Services.AddScoped<IDevicePageService, HttpDevicePageService>();
builder.Services.AddScoped<ISolarPageService, HttpSolarPageService>();
builder.Services.AddScoped<IDataPageService, HttpDataPageService>();
builder.Services.AddScoped<IAccountPageService, HttpAccountPageService>();

await builder.Build().RunAsync();
