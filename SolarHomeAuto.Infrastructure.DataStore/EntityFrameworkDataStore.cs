using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.SolarUsage.Models;
using Microsoft.EntityFrameworkCore;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Infrastructure.DataStore.Entities;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.DataStore.Filtering;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Infrastructure.DataStore.Filtering;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.RemoteCommands;

namespace SolarHomeAuto.Infrastructure.DataStore
{
    public abstract class EntityFrameworkDataStore : IDataStore
    {
        private readonly Lazy<IDataStoreDbContext> db;
        private readonly DataStoreSettings settings;

        public IDataStoreDbContext DbContext => db.Value;

        public EntityFrameworkDataStore(DataStoreSettings settings)
        {
            this.settings = settings;
            this.db = new Lazy<IDataStoreDbContext>(() => CreateContext(settings));
        }

        protected abstract IDataStoreDbContext CreateContext(DataStoreSettings settings);
        protected abstract IDataStoreTransaction CreateTransaction(DataStoreSettings settings);

        public IDataStoreTransaction CreateTransaction()
        {
            return CreateTransaction(settings);
        }

        public async Task<ApplicationState> GetApplicationState()
        {
            var state = await DbContext.ApplicationState.FirstOrDefaultAsync();

            return state == null
                ? ApplicationState.Default
                : state.ToDomain();
        }

        public async Task<List<SolarStats>> GetSolarStats(SolarStatsDuration duration, PagingFilter filter)
        {
            var query = DbContext.SolarStats.Where(x => x.Duration == duration);

            query = query.ApplyFilter(filter);

            var results = (await query.ToListAsync())
                .Select(x => x.ToDomain())
                .ToList();

            return results;
        }

        public async Task<(DateTime? From, DateTime? To)> GetSolarStatsDateRange(SolarStatsDuration duration)
        {
            if (!DbContext.SolarStats.Any())
            {
                return (null, null);
            }

            var min = await DbContext.SolarStats
                .Where(x => x.Duration == duration)
                .MinAsync(x => x.Date);

            var max = await DbContext.SolarStats
                .Where(x => x.Duration == duration)
                .MaxAsync(x => x.Date);

            return (min, max);
        }

        public async Task<AccountOAuthToken> GetAccountOAuthToken(string serviceId)
        {
            var matching = await DbContext.AuthTokens.FirstOrDefaultAsync(x => x.ServiceId == serviceId);

            return matching?.ToDomain();
        }

        public async Task<SolarRealTime> GetLatestSolarRealTime()
        {
            var match = await DbContext.SolarRealTime.OrderByDescending(x => x.Date).FirstOrDefaultAsync();

            return match?.ToDomain();
        }

        public async Task<List<SolarRealTime>> GetSolarRealTime(PagingFilter filter)
        {
            filter = filter ?? PagingFilter.Default;

            var query = DbContext.SolarRealTime.AsQueryable();

            query = query.ApplyFilter(filter);

            var results = (await query.ToListAsync())
                .Select(x => x.ToDomain())
                .ToList();

            return results;
        }

        public void Dispose()
        {
            this.DbContext.Dispose();
        }

        public async Task<List<DeviceHistory<T>>> GetDeviceHistory<T>(DeviceHistoryFilter filter, string deviceId = null)
        {
            var stateType = HistoryStateTypeMap.GetStateTypeName<T>();

            var results = await GetDeviceHistory(filter, deviceId, stateType);

            var domain = results
                .Select(x => x.ToDomain().ToTyped<T>())
                .ToList();

            return domain;
        }

        public async Task<DeviceHistory<T>> GetLatestDeviceHistory<T>(string deviceId, bool includeFailed = false)
        {
            var match = await DbContext.DeviceHistory
                .Where(x => x.DeviceId == deviceId && (includeFailed || x.Error == null))
                .OrderByDescending(x => x.Date)
                .FirstOrDefaultAsync();

            return match?.ToDomain().ToTyped<T>();
        }

        private async Task<List<DeviceHistoryEntity>> GetDeviceHistory(DeviceHistoryFilter filter, string deviceId = null, string stateType = null)
        {
            filter = filter ?? DeviceHistoryFilter.Default;

            var query = DbContext.DeviceHistory.AsQueryable();

            query = ApplyDeviceHistoryFilter(query, filter, deviceId, stateType);

            var results = await query.ToListAsync();

            return results;
        }

        private IQueryable<DeviceHistoryEntity> ApplyDeviceHistoryFilter(IQueryable<DeviceHistoryEntity> query, DeviceHistoryFilter filter, string deviceId = null, string stateType = null)
        {
            if (filter.StateChangesOnly)
            {
                query = query.Where(x => x.IsStateChange);
            }

            if (deviceId != null)
            {
                query = query.Where(x => x.DeviceId == deviceId);
            }

            if (stateType != null)
            {
                var devices = DbContext.Devices.Where(x => x.StateType == stateType);

                query = query.Join(devices, m => m.DeviceId, d => d.DeviceId, (m, d) => m);
            }

            if (!filter.IncludeFailed)
            {
                query = query.Where(x => x.Error == null);
            }

            query = query.ApplyFilter(filter);

            return query;
        }

        public Task<List<Device>> GetDevices()
        {
            return GetDevices(null);
        }

        public async Task<Device> GetDevice(string deviceId)
        {
            var results = await GetDevices(deviceId);

            return results.FirstOrDefault();
        }

        public async Task<List<DeviceConnectionSettings>> GetDeviceConnectionSettings()
        {
            var data = await DbContext.Devices.ToListAsync();

            return data.Select(x => x.ToConnection()).ToList();
        }

        public async Task<DeviceConnectionSettings> GetDeviceConnectionSettings(string deviceId)
        {
            var data = await DbContext.Devices.FirstOrDefaultAsync(x => x.DeviceId == deviceId);

            return data?.ToConnection();
        }

        public async Task<List<DeviceSchedule>> GetDeviceSchedules()
        {
            var data = await DbContext.Devices.ToListAsync();

            return data.SelectMany(x => x.ToSchedules()).ToList();
        }

        private async Task<List<Device>> GetDevices(string deviceId)
        {
            var entities = DbContext.Devices.AsQueryable();

            if (deviceId != null)
            {
                entities = entities.Where(x => x.DeviceId == deviceId);
            }

            var results = await entities.ToListAsync();

            return results.Select(x => x.ToDevice()).ToList();
        }

        public async Task<List<LogEntry>> GetLogs(LogFilter filter)
        {
            filter = filter ?? LogFilter.Default;

            var query = DbContext.Logs.AsQueryable();

            if (filter.Sources.Any())
            {
                query = query.Where(x => filter.Sources.Contains(x.Logger));
            }

            var results = await query.ApplyFilter(filter).ToListAsync();

            return results.Select(x => x.ToDomain()).ToList();
        }

        public async Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter)
        {
            var query = DbContext.RemoteCommandMessages.AsQueryable();

            if (filter?.Consumed.HasValue == true)
            {
                query = query.Where(x => x.Consumed == filter.Consumed);
            }

            if (filter?.Types?.Any() == true)
            {
                query = query.Where(x => filter.Types.Contains(x.Type));
            }

            if (filter?.RelatedIds?.Any() == true)
            {
                query = query.Where(x => filter.RelatedIds.Contains(x.RelatedId));
            }

            if (filter?.LatestOnly == true)
            {
                query = query.OrderByDescending(x => x.Date).Take(1);
            }

            if (filter != null)
            {
                query = query.ApplyFilter(filter);
            }

            var results = await query.ToListAsync();

            return results.Select(x => x.ToDomain()).ToList();
        }

        public async Task<SystemData> GetSystemData(string key)
        {
            var entity = await DbContext.SystemData.FirstOrDefaultAsync(x => x.Key == key);

            var result = entity?.ToDomain() ?? new SystemData { Key = key };

            return result;
        }
    }
}
