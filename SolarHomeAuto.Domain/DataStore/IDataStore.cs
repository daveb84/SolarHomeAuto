using SolarHomeAuto.Domain.DataStore.Filtering;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.RemoteCommands;

namespace SolarHomeAuto.Domain.DataStore
{
    public interface IDataStore : IDisposable
    {
        IDataStoreTransaction CreateTransaction();
        Task<ApplicationState> GetApplicationState();
        Task<List<SolarStats>> GetSolarStats(SolarStatsDuration duration, PagingFilter filter);
        Task<(DateTime? From, DateTime? To)> GetSolarStatsDateRange(SolarStatsDuration duration);
        Task<SolarRealTime> GetLatestSolarRealTime();
        Task<List<SolarRealTime>> GetSolarRealTime(PagingFilter filter);
        Task<AccountOAuthToken> GetAccountOAuthToken(string accountId);
        Task<List<DeviceHistory<T>>> GetDeviceHistory<T>(DeviceHistoryFilter filter, string deviceId = null);
        Task<List<Device>> GetDevices();
        Task<DeviceConnectionSettings> GetDeviceConnectionSettings(string deviceId);
        Task<Device> GetDevice(string deviceId);
        Task<DeviceHistory<T>> GetLatestDeviceHistory<T>(string deviceId, bool includeFailed = false);
        Task<List<LogEntry>> GetLogs(LogFilter filter);
        Task<List<DeviceConnectionSettings>> GetDeviceConnectionSettings();
        Task<List<DeviceSchedule>> GetDeviceSchedules();
        Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter);
        Task<SystemData> GetSystemData(string key);
    }
}
