using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;

namespace SolarHomeAuto.Domain.Devices
{
    public interface IDeviceProvider
    {
        Task<IDeviceConnection> Connect(DeviceConnectionSettings settings);
    }
}
