using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.Domain.PurgeData
{
    public interface IPurgeDataStore
    {
        Task PurgeLogs(Func<List<LogEntry>, Task<bool>> process);
        Task PurgeSolarRealTime(Func<List<SolarRealTime>, Task<bool>> process);
        Task PurgeDeviceHistory(Func<List<DeviceHistory>, Task<bool>> process);
        Task PurgeRemoteCommandMessages(Func<List<RemoteCommandMessage>, Task<bool>> process);
    }
}
