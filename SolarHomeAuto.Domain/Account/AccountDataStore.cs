using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.ServerApi;
using SolarHomeAuto.Domain.ServerApi.Models;
using SolarHomeAuto.Domain.SolarUsage;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Domain.Utils;

namespace SolarHomeAuto.Domain.Account
{
    public class AccountDataStore : IServerApiAccountService, IAccountCredentialsService, ISolarRealTimeImportScheduleService
    {
        private readonly IDataStoreFactory dataStoreFactory;
        private readonly ILogger<AccountDataStore> logger;
        private readonly DeviceConnectionFactory deviceConnectionFactory;

        public AccountDataStore(IDataStoreFactory dataStoreFactory, ILogger<AccountDataStore> logger, DeviceConnectionFactory deviceConnectionFactory)
        {
            this.dataStoreFactory = dataStoreFactory;
            this.logger = logger;
            this.deviceConnectionFactory = deviceConnectionFactory;
        }

        public async Task<T> GetAccountCredentials<T>(string key)
        {
            var all = await GetAllAccountCredentials();

            if (all == null)
            {
                return default(T);
            }

            try
            {
                return all[key].ToObject<T>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Cannot deserialize credentials for key {key} to type {typeof(T).Name}");

                return default(T);
            }
        }

        public async Task<JObject> GetAllAccountCredentials()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var state = await store.GetApplicationState();

                if (!string.IsNullOrWhiteSpace(state.AccountCredentials))
                {
                    var data = JObject.Parse(state.AccountCredentials);

                    return data;
                }

                return null;
            }
        }

        public async Task<ServerApiAccount> GetServerApiAccount()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var state = await store.GetApplicationState();

                var json = state.ServerApiAccount;

                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<ServerApiAccount>(json);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Cannot deserialize application state for type {nameof(ServerApiAccount)}");
                    }
                }

                return new ServerApiAccount();
            }
        }

        public async Task<List<SolarRealTimeSchedulePeriod>> GetSolarRealTimeImportSchedule()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var state = await store.GetApplicationState();

                var json = state.SolarRealTimeImportSchedule;

                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        return JsonConvert.DeserializeObject<List<SolarRealTimeSchedulePeriod>>(json);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Cannot deserialize application state for {nameof(ApplicationState.SolarRealTimeImportSchedule)}");
                    }
                }

                return new List<SolarRealTimeSchedulePeriod>();
            }
        }

        public async Task SaveAllAccountCredentials(JObject json)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.UpdateApplicationState(x =>
                    {
                        x.AccountCredentials = json.ToString(Formatting.None);
                    });

                    await trans.Commit();
                }
            }
        }

        public async Task SaveServerApiAccount(ServerApiAccount setting)
        {
            var json = setting == null ? null : JsonConvert.SerializeObject(setting);

            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.UpdateApplicationState(x =>
                    {
                        x.ServerApiAccount = json;
                    });

                    await trans.Commit();
                }
            }
        }

        public async Task<bool> SaveSolarRealTimeImportSchedule(List<SolarRealTimeSchedulePeriod> schedule)
        {
            var json = schedule?.Any() != true ? null : JsonConvert.SerializeObject(schedule);

            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.UpdateApplicationState(x =>
                    {
                        x.SolarRealTimeImportSchedule = json;
                    });

                    await trans.Commit();
                }
            }

            return true;
        }

        public async Task<List<AccountDevice>> GetDevices()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var devices = await store.GetDeviceConnectionSettings();

                var schedules = (await store.GetDeviceSchedules()).ToLookup(x => x.DeviceId);

                var data = devices
                    .Select(x =>
                    {
                        var mapped = SimpleMapper.MapToSubClass<DeviceConnectionSettings, AccountDevice>(x);

                        mapped.Schedules = schedules[x.DeviceId].ToList();

                        return mapped;
                    })
                    .ToList();

                return data;
            }
        }

        public async Task<bool> SaveDevices(List<AccountDevice> devices)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.DeleteAllDevices();
                    await trans.Commit();
                }

                using (var trans = store.CreateTransaction())
                {
                    foreach (var accountDevice in devices)
                    {
                        await SaveDeviceInternal(trans, accountDevice, true);
                    }

                    await trans.Commit();
                }
            }

            return true;
        }

        public async Task<bool> SaveDevice(string deviceId, AccountDevice accountDevice)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var device = await store.GetDevice(deviceId);

                if (device == null)
                {
                    return false;
                }

                if (deviceId != accountDevice.DeviceId)
                {
                    using (var trans = store.CreateTransaction())
                    {
                        await trans.DeleteDevice(deviceId);
                        await trans.Commit();
                    }
                }

                using (var trans = store.CreateTransaction())
                {
                    var success = await SaveDeviceInternal(trans, accountDevice, device.Enabled);

                    if (success)
                    {
                        await trans.Commit();
                    }

                    return success;
                }
            }
        }

        private async Task<bool> SaveDeviceInternal(IDataStoreTransaction trans, AccountDevice accountDevice, bool enabled)
        {
            IDeviceConnection connection;

            try
            {
                connection = await deviceConnectionFactory.Connect(accountDevice);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Device configuration error");
                return false;
            }

            var device = new Device
            {
                DeviceId = accountDevice.DeviceId,
                Name = accountDevice.Name,
                StateType = HistoryStateTypeMap.GetStateTypeName(connection.StateType),
                Enabled = enabled
            };

            await trans.SaveDevice(device, accountDevice, accountDevice.Schedules);
            return true;
        }
    }
}
