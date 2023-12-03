using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Domain.Monitoring;

namespace SolarHomeAuto.Infrastructure.Shelly.Devices
{
    public class ShellySwitchProvider : IDeviceProvider
    {
        private readonly IDeviceService deviceService;
        private readonly ILoggerFactory loggerFactory;
        private readonly IServiceProvider serviceProvider;
        private readonly MonitoringService monitoringService;

        public ShellySwitchProvider(IDeviceService deviceService, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, MonitoringService monitoringService)
        {
            this.deviceService = deviceService;
            this.loggerFactory = loggerFactory;
            this.serviceProvider = serviceProvider;
            this.monitoringService = monitoringService;
        }

        public async Task<IDeviceConnection> Connect(DeviceConnectionSettings settings)
        {
            var shellyDevice = ShellyDeviceConfig.Convert(settings);

            var environment = await monitoringService.GetEnvironment();

            if (environment.HasLanAccess)
            {
                if (!string.IsNullOrEmpty(shellyDevice.ShellyLanIPAddress))
                {
                    var httpClient = serviceProvider.GetRequiredService<HttpClient>();

                    var device = new ShellyLanRestDevice(shellyDevice, httpClient, deviceService, loggerFactory.CreateLogger<ShellyLanRestDevice>());

                    return device;
                }
                else
                {
                    var messageProvider = serviceProvider.GetRequiredService<IMessageServiceProvider>();

                    var device = new ShellyMqttDevice(shellyDevice, messageProvider, deviceService, loggerFactory.CreateLogger<ShellyMqttDevice>());

                    return device;
                }
            }
            else
            {
                var shellyClient = serviceProvider.GetRequiredService<IShellyCloudClient>();
                var dataStoreFactory = serviceProvider.GetRequiredService<IDataStoreFactory>();

                var device = new ShellyCloudSwitchDevice(shellyClient, shellyDevice, deviceService, dataStoreFactory, loggerFactory.CreateLogger<ShellyCloudSwitchDevice>());

                return device;
            }
        }
    }
}
