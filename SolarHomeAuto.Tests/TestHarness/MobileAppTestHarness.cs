using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.MobileApp;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Infrastructure.DataStore.Sqlite;
using SolarHomeAuto.Infrastructure.Mqtt;
using SolarHomeAuto.Infrastructure.ServerApi;
using SolarHomeAuto.Tests.Builders;
using SolarHomeAuto.Tests.Fakes;
using SolarHomeAuto.AppInit.MobileApp;
using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.Devices.Schedule;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories;

namespace SolarHomeAuto.Tests.TestHarness
{
    internal class MobileAppTestHarness : IDisposable
    {
        private MobileAppAllSettings settings;
        private ISolarApi solarApi;
        private IServiceProvider serviceProvider;
        private InMemoryConnectionFactory databaseConnection;

        private InMemoryServerApi serverApi;

        private List<Func<IDataStoreTransaction, Task>> addDataTasks;
        private List<AccountDevice> devices;
        private List<SolarRealTimeSchedulePeriod> solarSchedule;

        public IServiceProvider Services
        {
            get
            {
                EnsureBuilt();

                return serviceProvider;
            }
        }
        
        public MobileAppTestHarness()
        {
            addDataTasks = new();
            devices = new();

            var inMemSolarApi = new InMemorySolarApi();
            inMemSolarApi.RealTime.DefaultData = SolarRealTimeBuilder.NoSolar;
            solarApi = inMemSolarApi;

            databaseConnection = new();

            settings = new MobileAppAllSettings
            {
                DataStore = new DataStoreSettings
                {
                    EnableLogging = false,
                    InMemoryDatabase = true,
                    ConnectionString = $"Data Source=TestDb{Guid.NewGuid()};Mode=Memory;Cache=Shared",
                },
                Environment = new EnvironmentSettings
                {
                    Name = "Lan",
                    Type = EnvironmentType.MobileApp
                },
                Logging = new LoggingSettings
                {
                    DefaultLevel = LogLevel.Error,
                    Filters = new Dictionary<string, LogLevel>()
                },
                Mqtt = new MqttSettings()
                {
                    ClientEnabled = false,
                    SelfHostEnabled = false,
                },
                ServerApi = new ServerApiSettings
                {
                },
                DeviceScheduledJobs = new DeviceScheduleSettings
                {
                    IntervalSeconds = 0,
                    InitialWaitSeconds = 0,
                },
                SolarImportScheduledJob = new SolarScheduledJobSettings
                {
                    RealTimeInterval = 0,
                    RealTimeInitialWait = 0
                },
                Monitoring = new MonitoringSettings
                {
                    InitialWaitSeconds = 0,
                    MaxIntervalSeconds = 300,
                    MinIntervalSeconds = 2,
                    RemoteCommandsWaitSeconds = 30
                }
            };
        }

        public MobileAppTestHarness WithSolarApi(ISolarApi api)
        {
            solarApi = api;
            return this;
        }

        public MobileAppTestHarness WithSettings(Action<MobileAppAllSettings> update)
        {
            update(settings);

            return this;
        }

        public MobileAppTestHarness WithDevices(IEnumerable<AccountDevice> devices)
        {
            this.devices = devices.ToList();

            return this;
        }

        public MobileAppTestHarness WithSolarImportSchedule(IEnumerable<SolarRealTimeSchedulePeriod> schedule)
        {
            this.solarSchedule = schedule.ToList();

            return this;
        }

        public MobileAppTestHarness WithServerApi(InMemoryServerApi serverApi)
        {
            this.serverApi = serverApi;

            return this;
        }

        public SqliteDbContext GetDbContext()
        {
            return SqliteDataStore.CreateDbContext(settings.DataStore, databaseConnection);
        }

        public async Task RunBackgroundServiceForDateRange(DateTime start, DateTime stop)
        {
            await EnsureBuilt();

            var service = serviceProvider.GetService<IMobileAppService>();

            // Fake the datetime now and make delays instantaneous
            DateTimeNow.SetTestTimeNowOverride(start);
            DateTimeNow.SetTestDelayOverride(true, () =>
            {
                if (DateTimeNow.UtcNow > stop)
                {
                    service.Stop();
                }
            });

            await service.Start();

            if (DateTimeNow.UtcNow <= stop)
            {
                await Task.Delay(1000);

                if (DateTimeNow.UtcNow <= stop)
                {
                    Assert.Fail("Failed to reach stop time");
                }
            }
        }

        public async Task<IServiceProvider> Build()
        {
            var builder = new HostApplicationBuilder();

            var overrides = new MobileAppServices.Overrides 
            { 
                SolarApi = solarApi,
                ServerApi = serverApi,
                DatabaseConnection = databaseConnection,
            };

            MobileAppServices.InitApp(builder.Services, builder.Logging, settings, overrides);

            builder.Services.AddSingleton<IMobileAppServicePlatform, FakeMobileAppServicePlatform>();

            var host = builder.Build();

            serviceProvider = host.Services;

            if (addDataTasks.Any())
            {
                var dataStoreFactory = serviceProvider.GetService<IDataStoreFactory>();

                foreach (var action in addDataTasks)
                {
                    using (var store = dataStoreFactory.CreateStore())
                    {
                        using (var trans = store.CreateTransaction())
                        {
                            await action.Invoke(trans);
                            await trans.Commit();
                        }
                    }
                }
            }

            var accountService = serviceProvider.GetService<AccountDataStore>();

            await accountService.SaveDevices(devices);
            await accountService.SaveSolarRealTimeImportSchedule(solarSchedule);

            return serviceProvider;
        }

        public void Dispose()
        {
            databaseConnection?.Dispose();
            DateTimeNow.ResetTestOverrides();
        }

        private Task EnsureBuilt()
        {
            if (serviceProvider == null)
            {
                return Build();
            }

            return Task.CompletedTask;
        }
    }
}
