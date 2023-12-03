using Microsoft.Extensions.Configuration;
using SolarHomeAuto.AppInit.MobileApp;
using System.Reflection;

namespace SolarHomeAuto.MobileApp
{
    public static class MauiProgram
    {
        public static IServiceProvider Services { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
		    builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            // get the settings
            var a = Assembly.GetExecutingAssembly();
            
            using var appSettingsStream = a.GetManifestResourceStream("SolarHomeAuto.MobileApp.appsettings.json");
            var configBuilder = new ConfigurationBuilder()
                .AddJsonStream(appSettingsStream);

            using var appSettingsStreamProduction = a.GetManifestResourceStream("SolarHomeAuto.MobileApp.appsettings.Production.json");

            if (appSettingsStreamProduction != null)
            {
                configBuilder = configBuilder.AddJsonStream(appSettingsStreamProduction);
            }

            var config = configBuilder.Build();

            builder.Configuration.AddConfiguration(config);

            var settings = builder.Configuration.GetSection("SolarHomeAutoMobileApp").Get<MobileAppAllSettings>();

            if (settings.DataStore.InMemoryDatabase)
            {
                settings.DataStore.ConnectionString = $"Data Source=SolarHomeAutoDb;Mode=Memory;Cache=Shared";
            }
            else
            {
                settings.DataStore.ConnectionString = $"Filename={GetDatabasePath()}";
            }

            MobileAppServices.InitApp(builder.Services, builder.Logging, settings);

#if ANDROID
            builder.Services.AddSingleton<SolarHomeAuto.Domain.MobileApp.IMobileAppServicePlatform, SolarHomeAuto.MobileApp.Platforms.Android.AndroidMobileAppServicePlatform>();
#endif

            var app = builder.Build();

            Services = app.Services;

            return app;
        }

        private static string GetDatabasePath()
        {
            var databasePath = "";
            var databaseName = "SolarHomeAutoSqlite.db3";

            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                databasePath = Path.Combine(FileSystem.AppDataDirectory, databaseName);
            }
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                SQLitePCL.Batteries_V2.Init();
                databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName); ;
            }

            return databasePath;

        }
    }
}