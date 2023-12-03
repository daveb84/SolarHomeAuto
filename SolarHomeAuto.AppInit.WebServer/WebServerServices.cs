using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Auth;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Domain.SolarUsage;
using SolarHomeAuto.Infrastructure.DataStore;
using SolarHomeAuto.Infrastructure.DataStore.Logging;
using SolarHomeAuto.Infrastructure.Mqtt;
using SolarHomeAuto.Infrastructure.Solar;
using SolarHomeAuto.AppInit.WebServer.BackgroundServices;
using SolarHomeAuto.Infrastructure.Auth.LoginAuth;
using SolarHomeAuto.Domain.Devices.TestDevices;
using SolarHomeAuto.Domain.Devices;
using SolarHomeAuto.Infrastructure.Shelly.Devices;
using SolarHomeAuto.Infrastructure.Shelly;
using SolarHomeAuto.Domain.ServerApi;
using SolarHomeAuto.Domain.ScheduledJobs;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.UI.Services.Default;
using SolarHomeAuto.Domain.MobileApp;
using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.Devices.Schedule;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.Domain.Monitoring.RemoteCommands;
using SolarHomeAuto.Domain.Devices.RemoteCommands;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.PurgeData;
using SolarHomeAuto.Infrastructure.DataStore.SqlServer;

namespace SolarHomeAuto.AppInit.WebServer
{
    public static class WebServerServices
    {
        public static void InitApp(IServiceCollection services, ILoggingBuilder logging, WebServerAllSettings settings)
        {
            // environment
            services.AddSingleton(settings.Environment);

            // logging
            logging.SetMinimumLevel(settings.Logging.DefaultLevel);
            if (settings.Logging.Filters?.Any() == true)
            {
                foreach (var key in settings.Logging.Filters.Keys)
                {
                    logging.AddFilter(key, settings.Logging.Filters[key]);
                }
            }

            // authentication
            services.AddSingleton(settings.Auth);
            services.AddScoped<IAuthService, LoginAuthenticator>();

            // settings service
            services.AddSingleton<AccountResetService>();
            services.AddSingleton<AccountDataStore>();
            services.AddSingleton<IServerApiAccountService, AccountDataStore>();
            services.AddSingleton<IAccountCredentialsService, AccountDataStore>();
            services.AddSingleton<ISolarRealTimeImportScheduleService, AccountDataStore>();

            // Data store
            services.AddSingleton(settings.DataStore);
            services.AddSingleton<IDataStoreFactory, SqlDataStoreFactory>();
            services.AddSingleton<ILogViewer, DataStoreLogViewer>();
            services.AddSingleton<StorePurgedDataService>();

            if (settings.DataStore.EnableLogging)
            {
                services.AddSingleton<ILoggerProvider, DataStoreLoggerProvider>();
            }

            // MQTT
            services.AddSingleton(settings.Mqtt);
            services.AddSingleton<MessageServiceSettings>(x => settings.Mqtt);
            services.AddSingleton<TestMessageService>();

            if (settings.Mqtt.ClientEnabled)
            {
                services.AddSingleton<IMessageServiceProvider, MqttMessageClient>();
                services.AddHostedService<MqttClientBackgroundService>();
            }
            else
            {
                services.AddSingleton<IMessageServiceProvider, NullMessageServiceProvider>();
            }

            if (settings.Mqtt.SelfHostEnabled)
            {
                services.AddSingleton<IMessagingServer, MqttMessageServer>();
                services.AddHostedService<MqttServerBackgroundService>();
            }

            // Devices
            services.AddSingleton<DeviceConnectionFactory>();
            services.AddSingleton<IDeviceService, DeviceService>();

            // Devices - register providers
            DeviceConnectionFactory.AddProvider("ShellySwitch", typeof(ShellySwitchProvider));
            DeviceConnectionFactory.AddProvider("TestDevice", typeof(TestSwitchProvider));
            services.AddSingleton<TestSwitchProvider>();
            services.AddSingleton<ShellySwitchProvider>();

            // Shelly
            services.AddSingleton<IShellyCloudClient, ShellyCloudClient>();

            // Solar API
            services.AddSingleton(settings.SolarImportScheduledJob);
            services.AddSingleton<ISolarApi, SolarmanApi>();

            // Solar excess
            services.AddSingleton(settings.DeviceScheduledJobs);

            // Solar usage
            services.AddSingleton<SolarStatsImporter>();
            services.AddSingleton<SolarRealTimeImporter>();

            // Server API
            services.AddSingleton<IServerApiClient, NullServerApiClient>();
            services.AddSingleton<IPurgeDataService, PurgeDataService>();
            services.AddSingleton<IPurgeDataStore, SqlPurgeDataStoreService>();

            // Jobs
            services.AddSingleton(settings.Monitoring);
            services.AddSingleton<MonitoringService>();
            services.AddSingleton<MonitoringWorker>();
            services.AddSingleton<MonitoringRemoteCommandConsumer>();
            services.AddSingleton<MonitoringRemoteCommandPublisher>();
            services.AddSingleton<DeviceRemoteCommandConsumer>();
            services.AddSingleton<DeviceRemoteCommandPublisher>();
            services.AddSingleton<RemoteCommandService>();
            services.AddSingleton<RemoteCommandMessageBuilder>();
            services.AddSingleton<IScheduledJob, DeviceScheduleJob>();
            services.AddSingleton<IScheduledJob, SolarRealTimeScheduledJob>();
            services.AddHostedService<MonitoringBackgroundService>();

            // page services
            services.AddScoped<IMonitoringPageService, MonitoringPageService>();
            services.AddScoped<IDevicePageService, DevicePageService>();
            services.AddScoped<ISolarPageService, SolarPageService>();
            services.AddScoped<IDataPageService, DataPageService>();
            services.AddScoped<IAccountPageService, AccountPageService>();
            services.AddSingleton<IMobileAppService, NullMobileAppService>();
        }
    }
}