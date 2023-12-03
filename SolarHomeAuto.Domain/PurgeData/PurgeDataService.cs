using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.ServerApi;
using SolarHomeAuto.Domain.ServerApi.Models;

namespace SolarHomeAuto.Domain.PurgeData
{
    public class PurgeDataService : IPurgeDataService
    {
        private readonly IServerApiClient serverApiClient;
        private readonly IPurgeDataStore purgeDataStoreService;

        public PurgeDataService(IServerApiClient serverApiClient, IPurgeDataStore purgeDataStoreService)
        {
            this.serverApiClient = serverApiClient;
            this.purgeDataStoreService = purgeDataStoreService;
        }

        public async Task PurgeData()
        {
            await purgeDataStoreService.PurgeLogs(data => UploadData(new UploadDataRequest { Logs = data }));
            await purgeDataStoreService.PurgeSolarRealTime(data => UploadData(new UploadDataRequest { SolarRealTime = data }));
            await purgeDataStoreService.PurgeDeviceHistory(data => UploadData(new UploadDataRequest { DeviceHistory = data }));
            await purgeDataStoreService.PurgeRemoteCommandMessages(data => Task.FromResult(true));
        }

        private async Task<bool> UploadData(UploadDataRequest data)
        {
            await serverApiClient.UploadData(data);

            return true;
        }
    }
}
