using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.PurgeData;
using SolarHomeAuto.Domain.RemoteCommands;

namespace SolarHomeAuto.UI.Services.Default
{
    public class DataPageService : IDataPageService
    {
        private readonly ILogViewer logViewer;
        private readonly IPurgeDataService uploadDataService;
        private readonly EnvironmentSettings environment;
        private readonly AccountResetService accountService;
        private readonly IDataStoreFactory dataStoreFactory;

        public DataPageService(ILogViewer logViewer, IPurgeDataService uploadDataService, EnvironmentSettings environment, AccountResetService accountService, IDataStoreFactory dataStoreFactory)
        {
            this.logViewer = logViewer;
            this.uploadDataService = uploadDataService;
            this.environment = environment;
            this.accountService = accountService;
            this.dataStoreFactory = dataStoreFactory;
        }

        public bool IsMobileApp => environment.IsMobileApp;

        public Task<List<LogEntry>> GetLogs(LogFilter filter)
        {
            return logViewer.Get(filter);
        }

        public async Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var results = await store.GetRemoteCommandMessages(filter);

                return results;
            }
        }

        public Task PurgeData()
        {
            return uploadDataService.PurgeData();
        }

        public Task ResetSettings()
        {
            return accountService.ResetSettings();
        }
    }
}
