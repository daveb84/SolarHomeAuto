using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices;
using SolarHomeAuto.Domain.Devices.Types;

namespace SolarHomeAuto.Infrastructure.Shelly.Devices
{
    public class ShellyCloudSwitchDevice : SwitchDeviceBase
    {
        private readonly IShellyCloudClient shellyClient;
        private readonly ShellyDeviceConfig device;
        private readonly IDataStoreFactory dataStoreFactory;

        public ShellyCloudSwitchDevice(IShellyCloudClient shellyClient, ShellyDeviceConfig device, IDeviceService deviceService, IDataStoreFactory dataStoreFactory, ILogger logger)
            : base(device, deviceService, logger)
        {
            this.shellyClient = shellyClient;
            this.device = device;
            this.dataStoreFactory = dataStoreFactory;
        }

        protected override async Task<SwitchStatusResult> GetStatusInternal()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var history = await store.GetLatestDeviceHistory<SwitchHistoryState>(DeviceId);

                if (history != null && history.Date > DateTimeNow.UtcNow.AddSeconds(-device.CloudRefreshStatusTime))
                {
                    return new SwitchStatusResult(history.State.Status, history.State.Power, false);
                }

                var status = await shellyClient.GetSwitchStatus(device.ShellyDeviceId);

                return new SwitchStatusResult(status.Status, status.Power, true);
            }
        }

        protected override async Task<SwitchActionResult> SwitchInternal(SwitchAction action)
        {
            Task<bool> switchAction = action switch
            {
                SwitchAction.TurnOn => shellyClient.Switch(device.ShellyDeviceId, true),
                SwitchAction.TurnOff => shellyClient.Switch(device.ShellyDeviceId, false),
                _ => null
            };

            if (switchAction == null)
            {
                return SwitchActionResult.NoAction;
            }

            var result = await switchAction;

            return result ? SwitchActionResult.Success : SwitchActionResult.Failure;
        }
    }
}
