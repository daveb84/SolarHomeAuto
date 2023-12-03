using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices.Types;
using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.History;

namespace SolarHomeAuto.Domain.Devices.TestDevices
{
    public class TestSwitchDevice : SwitchDeviceBase
    {
        private readonly IDataStoreFactory dataStoreFactory;

        public TestSwitchDevice(DeviceConnectionSettings device, IDeviceService deviceService, ILogger logger, IDataStoreFactory dataStoreFactory)
            : base(device, deviceService, logger)
        {
            this.dataStoreFactory = dataStoreFactory;
        }

        protected override async Task<SwitchStatusResult> GetStatusInternal()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var filter = new DeviceHistoryFilter
                {
                    NewestFirst = true,
                    Skip = 0,
                    Take = 1
                };

                var history = await store.GetDeviceHistory<SwitchHistoryState>(filter, DeviceId);

                var status = history.FirstOrDefault()?.State.Status ?? SwitchStatus.Off;

                return new SwitchStatusResult(status);
            }
        }

        protected override Task<SwitchActionResult> SwitchInternal(SwitchAction action)
        {
            return Task.FromResult(SwitchActionResult.Success);
        }
    }
}
