using SolarHomeAuto.Infrastructure.Shelly.Models;

namespace SolarHomeAuto.Infrastructure.Shelly
{
    public interface IShellyCloudClient
    {
        Task<ShellySwitchStatus> GetSwitchStatus(string deviceId);
        Task<bool> Switch(string deviceId, bool on);
    }
}