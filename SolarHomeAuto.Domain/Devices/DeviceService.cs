using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Types;

namespace SolarHomeAuto.Domain.Devices
{
    public class DeviceService : IDeviceService
    {
        private readonly IDataStoreFactory dataStoreFactory;
        private readonly DeviceConnectionFactory deviceConnectionFactory;

        public DeviceService(IDataStoreFactory dataStoreFactory, DeviceConnectionFactory deviceConnectionFactory)
        {
            this.dataStoreFactory = dataStoreFactory;
            this.deviceConnectionFactory = deviceConnectionFactory;
        }

        public async Task<DeviceConnectionSettings> GetDeviceConnectionSettings(string deviceId)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                return await store.GetDeviceConnectionSettings(deviceId);
            }
        }

        public async Task<List<Device>> GetDevices()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                return await store.GetDevices();
            }
        }

        public async Task<Device> GetDevice(string deviceId)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var data = await store.GetDevice(deviceId);
                return data;
            }
        }

        public async Task EnableDevice(EnableDeviceRequest enable)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.EnableDevice(enable);
                    await trans.Commit();
                }
            }
        }

        public async Task SaveStateChange<T>(string deviceId, T state, string eventSource, string error)
            where T : IDeviceHistoryState<T>
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var history = new DeviceHistory<T>
                {
                    DeviceId = deviceId,
                    State = state,
                    Source = eventSource,
                    Error = error,
                    Date = DateTimeNow.UtcNow,
                    IsStateChange = true,
                };

                var latest = await store.GetLatestDeviceHistory<T>(deviceId, true);

                if (latest != null && !history.State.IsStateChange(latest))
                {
                    history.IsStateChange = false;
                }

                using (var trans = store.CreateTransaction())
                {
                    await trans.SaveDeviceHistory(history);
                    await trans.Commit();
                }
            }
        }

        public async Task<List<DeviceHistory<T>>> GetDeviceHistory<T>(string deviceId, DeviceHistoryFilter filter)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var data = await store.GetDeviceHistory<T>(filter, deviceId);

                return data;
            }
        }

        public async Task<T> GetDeviceConnection<T>(string deviceId) where T : class, IDeviceConnection
        {
            var config = await GetDeviceConnectionSettings(deviceId);

            if (config == null)
            {
                return null;
            }

            return await deviceConnectionFactory.Connect<T>(config);
        }
    }
}
