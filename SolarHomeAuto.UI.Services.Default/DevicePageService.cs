using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Devices;
using SolarHomeAuto.Domain.Devices.RemoteCommands;
using SolarHomeAuto.Domain.Monitoring;

namespace SolarHomeAuto.UI.Services.Default
{
    public class DevicePageService : IDevicePageService
    {
        private readonly IDeviceService deviceService;
        private readonly DeviceConnectionFactory deviceConnectionFactory;
        private readonly DeviceRemoteCommandPublisher remoteCommandPublisher;
        private readonly MonitoringService monitoringService;

        public DevicePageService(IDeviceService deviceService, DeviceConnectionFactory deviceFactory, DeviceRemoteCommandPublisher remoteCommandPublisher, MonitoringService monitoringService)
        {
            this.deviceService = deviceService;
            this.deviceConnectionFactory = deviceFactory;
            this.remoteCommandPublisher = remoteCommandPublisher;
            this.monitoringService = monitoringService;
        }

        public Task<List<Device>> GetDevices() => deviceService.GetDevices();

        public async Task<SwitchStatus> GetDeviceStatus(string deviceId)
        {
            if (await monitoringService.IsMonitoringHost())
            {
                var device = await GetDeviceConnection(deviceId);

                var status = await device.GetStatus();

                return status.Status;
            }
            else
            {
                return await remoteCommandPublisher.GetDeviceStatus(deviceId);
            }
        }

        public Task<List<DeviceHistory<SwitchHistoryState>>> GetDeviceHistory(string deviceId, DeviceHistoryFilter filter)
            => deviceService.GetDeviceHistory<SwitchHistoryState>(deviceId, filter);

        public async Task SwitchDevice(string deviceId, SwitchAction action)
        {
            if (await monitoringService.IsMonitoringHost())
            {
                var device = await GetDeviceConnection(deviceId);

                await device.Switch(action, "Manual");
            }
            else
            {
                await remoteCommandPublisher.SwitchDevice(deviceId, action);
            }
        }

        public async Task<bool> IsDeviceEnabled(string deviceId)
        {
            if (await monitoringService.IsMonitoringHost())
            {
                var deviceInfo = await deviceService.GetDevice(deviceId);

                return deviceInfo?.Enabled == true;
            }
            else
            {
                return await remoteCommandPublisher.IsDeviceEnabled(deviceId);
            }
        }

        public async Task EnableDevice(EnableDeviceRequest data)
        {
            if (await monitoringService.IsMonitoringHost())
            {
                await deviceService.EnableDevice(data);
            }
            else
            {
                await remoteCommandPublisher.EnableDevice(data);
            }
        }

        private async Task<ISwitchDevice> GetDeviceConnection(string deviceId)
        {
            var deviceConfig = await deviceService.GetDeviceConnectionSettings(deviceId);

            if (deviceConfig == null)
            {
                return null;
            }

            try
            {
                var device = await deviceConnectionFactory.Connect(deviceConfig);

                return device as ISwitchDevice;
            }
            catch
            {
                return null;
            }
        }
    }
}
