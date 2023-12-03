using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.RemoteCommands;

namespace SolarHomeAuto.Domain.DataStore
{
    public interface IDataStoreTransaction : IDisposable
    {
        IDataStore Store { get; }

        Task UpdateApplicationState(Action<ApplicationState> update);
        Task SaveSolarStats(IEnumerable<SolarStats> data);
        Task SaveSolarRealTime(SolarRealTime data);
        Task SaveSolarRealTime(IEnumerable<SolarRealTime> data);
        Task SaveSolarAuthToken(AccountOAuthToken data);
        Task SaveDeviceHistory(DeviceHistory data);
        Task SaveDeviceHistory(IEnumerable<DeviceHistory> data);
        Task EnableDevice(EnableDeviceRequest data);
        Task SaveLogs(IEnumerable<LogEntry> logs);
        Task Commit();
        Task DeleteAllDevices();
        Task DeleteDevice(string deviceId);
        Task SaveDevice(Device device, DeviceConnectionSettings connection, List<DeviceSchedule> schedules);
        Task SaveRemoteCommandMessage(RemoteCommandMessage data);
        Task ConsumeRemoteCommandMessages(List<Guid> messageIds, string result);
        Task SaveSystemData(SystemData data);
        Task ClearSystemData(string key);
    }
}
