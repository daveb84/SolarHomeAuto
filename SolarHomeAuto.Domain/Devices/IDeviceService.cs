using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Types;

namespace SolarHomeAuto.Domain.Devices
{
    public interface IDeviceService
    {
        Task<Device> GetDevice(string deviceId);
        Task<List<Device>> GetDevices();
        Task<List<DeviceHistory<T>>> GetDeviceHistory<T>(string deviceId, DeviceHistoryFilter filter);
        Task<DeviceConnectionSettings> GetDeviceConnectionSettings(string deviceId);
        Task SaveStateChange<T>(string deviceId, T state, string eventSource, string error)
            where T : IDeviceHistoryState<T>;
        Task EnableDevice(EnableDeviceRequest device);
        Task<T> GetDeviceConnection<T>(string deviceId) where T : class, IDeviceConnection;
    }
}
