using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;

namespace SolarHomeAuto.Domain.Devices.TestDevices
{
    public class TestSwitchProvider : IDeviceProvider
    {
        private IDeviceService deviceService;
        private readonly ILoggerFactory loggerFactory;
        private readonly IDataStoreFactory dataStoreFactory;

        public TestSwitchProvider(IDeviceService deviceService, ILoggerFactory loggerFactory, IDataStoreFactory dataStoreFactory)
        {
            this.deviceService = deviceService;
            this.loggerFactory = loggerFactory;
            this.dataStoreFactory = dataStoreFactory;
        }

        public Task<IDeviceConnection> Connect(DeviceConnectionSettings settings)
        {
            var device = new TestSwitchDevice(settings, deviceService, loggerFactory.CreateLogger<TestSwitchDevice>(), dataStoreFactory);

            return Task.FromResult((IDeviceConnection)device);
        }
    }
}
