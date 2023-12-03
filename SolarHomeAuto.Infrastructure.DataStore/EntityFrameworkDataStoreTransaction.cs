using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.DataStore.Entities;
using Microsoft.EntityFrameworkCore;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.RemoteCommands;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SolarHomeAuto.Infrastructure.DataStore
{
    public abstract class EntityFrameworkDataStoreTransaction : IDataStoreTransaction
    {
        private readonly Lazy<EntityFrameworkDataStore> dataStore;
        public IDataStore Store => dataStore.Value;
        private IDataStoreDbContext Db => dataStore.Value.DbContext;

        public EntityFrameworkDataStoreTransaction(DataStoreSettings settings)
        {
            this.dataStore = new Lazy<EntityFrameworkDataStore>(() => CreateStore(settings));
        }

        protected abstract EntityFrameworkDataStore CreateStore(DataStoreSettings settings);

        public Task Commit()
        {
            return Db.SaveChangesAsync();
        }

        public void Dispose()
        {
            Store.Dispose();
        }

        public async Task UpdateApplicationState(Action<ApplicationState> update)
        {
            var entity = await Db.ApplicationState.FirstOrDefaultAsync();

            if (entity == null)
            {
                entity = ApplicationStateEntity.FromDomain(ApplicationState.Default);
                Db.ApplicationState.Add(entity);
            }

            var updatedState = entity.ToDomain();
            update(updatedState);

            entity.CopyFrom(updatedState);
        }

        public async Task SaveSolarRealTime(SolarRealTime data)
        {
            var entity = SolarRealTimeEntity.FromDomain(data);

            await Db.SolarRealTime.AddAsync(entity);
        }

        public async Task SaveSolarRealTime(IEnumerable<SolarRealTime> data)
        {
            if (data?.Any() != true) return;

            var entities = data.Select(x => SolarRealTimeEntity.FromDomain(x)).ToArray();

            await Db.SolarRealTime.AddRangeAsync(entities);
        }

        public async Task SaveSolarAuthToken(AccountOAuthToken data)
        {
            var matching = await Db.AuthTokens.FirstOrDefaultAsync(x => x.ServiceId == x.ServiceId);

            if (matching != null) 
            {
                matching.Update(data);
            }
            else
            {
                var token = AuthTokenEntity.FromDomain(data);

                await Db.AuthTokens.AddAsync(token);
            }
        }

        public Task SaveSolarStats(IEnumerable<SolarStats> data)
        {
            var entities = data.Select(SolarStatsEntity.FromDomain);

            return Db.SolarStats.AddRangeAsync(entities);
        }

        public async Task SaveDeviceHistory(DeviceHistory data)
        {
            var entity = DeviceHistoryEntity.FromDomain(data);

            await Db.DeviceHistory.AddAsync(entity);
        }

        public async Task SaveDeviceHistory(IEnumerable<DeviceHistory> data)
        {
            if (data?.Any() != true) return;

            var entities = data.Select(x => DeviceHistoryEntity.FromDomain(x)).ToArray();

            await Db.DeviceHistory.AddRangeAsync(entities);
        }

        public async Task SaveDevice(Device device, DeviceConnectionSettings connection, List<DeviceSchedule> schedules)
        {
            var entity = await Db.Devices.FirstOrDefaultAsync(x => x.DeviceId == device.DeviceId);

            if (entity == null)
            {
                entity = DeviceEntity.FromDomain(device, connection, schedules);
                await Db.Devices.AddAsync(entity);
            }
            else
            {
                entity.Update(device, connection, schedules);
            }
        }

        public async Task EnableDevice(EnableDeviceRequest data)
        {
            var entity = await Db.Devices.FirstOrDefaultAsync(x => x.DeviceId == data.DeviceId);

            if (entity != null)
            {
                entity.Enabled = data.Enable;
            }
        }

        public async Task DeleteAllDevices()
        {
            var all = await Db.Devices.ToListAsync();

            Db.Devices.RemoveRange(all);
        }

        public async Task SaveLogs(IEnumerable<LogEntry> logs)
        {
            if (logs?.Any() != true) return;

            await Db.Logs.AddRangeAsync(logs.Select(x => LogEntity.FromDomain(x)).ToArray());
        }

        public async Task DeleteDevice(string deviceId)
        {
            var match = await Db.Devices.FirstOrDefaultAsync(x => x.DeviceId == deviceId);

            if (match != null)
            {
                Db.Devices.Remove(match);
            }
        }

        public async Task SaveRemoteCommandMessage(RemoteCommandMessage data)
        {
            var matching = await Db.RemoteCommandMessages.FirstOrDefaultAsync(x => x.MessageId == data.MessageId);

            if (matching != null)
            {
                matching.Update(data);
            }
            else
            {
                var newData = RemoteCommandMessageEntity.FromDomain(data);

                await Db.RemoteCommandMessages.AddAsync(newData);
            }
        }

        public async Task ConsumeRemoteCommandMessages(List<Guid> messageIds, string result)
        {
            var matches = await Db.RemoteCommandMessages
                .Where(x => messageIds.Contains(x.MessageId) && !x.Consumed)
                .ToListAsync();

            foreach (var match in matches)
            {
                match.Consumed = true;
                match.ConsumedResult = result;
            }
        }

        public async Task SaveSystemData(SystemData data)
        {
            var matching = await Db.SystemData
                .FirstOrDefaultAsync(x => x.Key == data.Key);

            if (matching != null)
            {
                matching.Update(data);
            }
            else
            {
                var newData = SystemDataEntity.FromDomain(data);

                await Db.SystemData.AddAsync(newData);
            }
        }

        public async Task ClearSystemData(string key)
        {
            var matching = await Db.SystemData
                .FirstOrDefaultAsync(x => x.Key == key);

            if (matching != null)
            {
                Db.SystemData.Remove(matching);
            }
        }
    }
}
