using Microsoft.Extensions.DependencyInjection;
using SolarHomeAuto.Domain.Devices;
using SolarHomeAuto.Domain.MobileApp;
using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Infrastructure.Mqtt;
using SolarHomeAuto.Infrastructure.ServerApi;
using SolarHomeAuto.Infrastructure.Shelly.Devices;
using SolarHomeAuto.Domain.ServerApi;
using SolarHomeAuto.Infrastructure.ServerApi.Logging;
using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Infrastructure.DataStore.Sqlite;
using SolarHomeAuto.Domain.Devices.TestDevices;
using SolarHomeAuto.Infrastructure.DataStore.Logging;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Infrastructure.Solar;
using SolarHomeAuto.Domain.SolarUsage;
using SolarHomeAuto.Domain.ScheduledJobs;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.UI.Services.Default;
using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.Devices.Schedule;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.Domain.Monitoring.RemoteCommands;
using SolarHomeAuto.Domain.Devices.RemoteCommands;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.PurgeData;
using SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories;
using SolarHomeAuto.Infrastructure.Shelly;

namespace SolarHomeAuto.AppInit.MobileApp
{
    public class MobileAppServices
    {
        public class Overrides
        {
            public ISolarApi SolarApi { get; set; }
            public IServerApiClient ServerApi { get; set; }
            public ISqliteConnectionFactory DatabaseConnection { get; set; }
        }

        public static void InitApp(IServiceCollection services, ILoggingBuilder logging, MobileAppAllSettings settings, Overrides overrides = null)
        {
            services.AddSingleton<HttpClient>();

            // environment
            services.AddSingleton(settings.Environment);

            // bind all settings objects
            services.AddSingleton(settings);
            services.AddSingleton(settings.ServerApi);
            services.AddSingleton(settings.DataStore);
            services.AddSingleton(settings.Mqtt);
            services.AddSingleton<MessageServiceSettings>(x => settings.Mqtt);
            services.AddSingleton(settings.DeviceScheduledJobs);
            services.AddSingleton(settings.SolarImportScheduledJob);

            // settings service
            services.AddSingleton<AccountResetService>();
            services.AddSingleton<AccountDataStore>();
            services.AddSingleton<IServerApiAccountService, AccountDataStore>();
            services.AddSingleton<IAccountCredentialsService, AccountDataStore>();
            services.AddSingleton<ISolarRealTimeImportScheduleService, AccountDataStore>();

            // mobile app background service
            services.AddSingleton<IMobileAppService, MobileAppService>();
            services.AddSingleton<MobileAppServiceWorker>();

            // server API
            if (overrides?.ServerApi != null)
            {
                services.AddSingleton<IServerApiClient>(overrides.ServerApi);
            }
            else
            {
                services.AddSingleton<IServerApiClient, ServerApiClient>();
            }

            // MQTT
            services.AddSingleton<TestMessageService>();

            if (settings.Mqtt.ClientEnabled)
            {
                services.AddSingleton<IMessageServiceProvider, MqttMessageClient>();
            }
            else
            {
                services.AddSingleton<IMessageServiceProvider, NullMessageServiceProvider>();
            }

            if (settings.Mqtt.SelfHostEnabled)
            {
                services.AddSingleton<IMessagingServer, MqttMessageServer>();
            }
            else
            {
                services.AddSingleton<IMessagingServer, NullMessagingServer>();
            }

            // data store
            services.AddSingleton<IDataStoreFactory, SqliteDataStoreFactory>();

            if (overrides?.DatabaseConnection != null)
            {
                services.AddSingleton(overrides.DatabaseConnection);
            }
            else
            {
                services.AddSingleton<ISqliteConnectionFactory, DefaultConnectionFactory>();
            }

            services.AddSingleton<IPurgeDataService, PurgeDataService>();
            services.AddSingleton<IPurgeDataStore, SqlitePurgeDataStoreService>();

            // logging
            logging.SetMinimumLevel(settings.Logging.DefaultLevel);
            if (settings.Logging.Filters?.Any() == true)
            {
                foreach (var key in settings.Logging.Filters.Keys)
                {
                    logging.AddFilter(key, settings.Logging.Filters[key]);
                }
            }

            if (settings.ServerApi.EnableLogging)
            {
                services.AddSingleton<ILoggerProvider, ServerApiLoggerProvider>();
            }

            services.AddSingleton<ILogViewer, DataStoreLogViewer>();

            if (settings.DataStore.EnableLogging)
            {
                services.AddSingleton<ILoggerProvider, DataStoreLoggerProvider>();
            }

            // page services
            services.AddScoped<IMonitoringPageService, MonitoringPageService>();
            services.AddScoped<IDevicePageService, DevicePageService>();
            services.AddScoped<ISolarPageService, SolarPageService>();
            services.AddScoped<IAuthPageService, NullAuthPageService>();
            services.AddScoped<IDataPageService, DataPageService>();
            services.AddScoped<IAccountPageService, AccountPageService>();

            // devices
            services.AddSingleton<DeviceConnectionFactory>();

            // Shelly
            services.AddSingleton<IShellyCloudClient, ShellyCloudClient>();

            DeviceConnectionFactory.AddProvider("ShellySwitch", typeof(ShellySwitchProvider));
            services.AddSingleton<ShellySwitchProvider>();
            services.AddSingleton<IDeviceService, DeviceService>();

            // test device is dependant on data store
            DeviceConnectionFactory.AddProvider("TestDevice", typeof(TestSwitchProvider));
            services.AddSingleton<TestSwitchProvider>();

            // solar data
            if (overrides?.SolarApi != null)
            {
                services.AddSingleton(x => overrides.SolarApi);
            }
            else
            {
                services.AddSingleton<ISolarApi, SolarmanApi>();
            }

            services.AddSingleton<SolarRealTimeImporter>();

            // scheduled jobs
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
        }
    }
}