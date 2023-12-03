using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.ServerApi.Models;

namespace SolarHomeAuto.Domain.PurgeData
{
    public class StorePurgedDataService
    {
        private readonly IDataStoreFactory dataStoreFactory;

        public StorePurgedDataService(IDataStoreFactory dataStoreFactory)
        {
            this.dataStoreFactory = dataStoreFactory;
        }

        public async Task SaveData(UploadDataRequest data)
        {
            if (data == null) return;

            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.SaveLogs(data.Logs);
                    await trans.SaveDeviceHistory(data.DeviceHistory);
                    await trans.SaveSolarRealTime(data.SolarRealTime);
                    await trans.Commit();
                }
            }
        }
    }
}
