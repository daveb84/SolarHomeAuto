using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;

namespace SolarHomeAuto.UI.Services
{
    public interface IDevicePageService
    {
        Task<List<Device>> GetDevices();
        Task SwitchDevice(string deviceId, SwitchAction action);
        Task EnableDevice(EnableDeviceRequest device);
        Task<SwitchStatus> GetDeviceStatus(string deviceId);
        Task<bool> IsDeviceEnabled(string deviceId);
        Task<List<DeviceHistory<SwitchHistoryState>>> GetDeviceHistory(string deviceId, DeviceHistoryFilter filter);        
    }
}
