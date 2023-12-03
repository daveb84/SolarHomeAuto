using Microsoft.AspNetCore.Authentication.Cookies;
using SolarHomeAuto.AppInit.WebServer;
using SolarHomeAuto.AppInit.WebServer.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var applicationSettings = builder.Configuration.GetSection("SolarHomeAutoWebServer").Get<WebServerAllSettings>();
WebServerServices.InitApp(builder.Services, builder.Logging, applicationSettings);

// Http
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// cookie auth
builder.Services.AddSingleton<CookieAuthEvents>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        o.SlidingExpiration = true;
        o.EventsType = typeof(CookieAuthEvents);
    });

// razor pages
builder.Services.AddControllersWithViews();

var mvc = builder.Services.AddRazorPages();
if (builder.Environment.IsDevelopment())
{
    //mvc.AddRazorRuntimeCompilation();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (!applicationSettings.Auth.AllowHttp)
{
    app.UseHttpsRedirection();
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.SameAsRequest
});

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
