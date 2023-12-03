using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.ServerApi;

namespace SolarHomeAuto.Domain.Account
{
    public class AccountResetService
    {
        private readonly IServerApiClient serverApi;
        private readonly EnvironmentSettings environment;
        private readonly AccountDataStore accountDataStore;

        public AccountResetService(IServerApiClient serverApi, EnvironmentSettings environment, AccountDataStore accountDataStore)
        {
            this.serverApi = serverApi;
            this.environment = environment;
            this.accountDataStore = accountDataStore;
        }

        public async Task<AllAccountSettings> GetAllAccountSettings()
        {
            var creds = await accountDataStore.GetAllAccountCredentials();
            var devices = await accountDataStore.GetDevices();
            var solar = await accountDataStore.GetSolarRealTimeImportSchedule();

            return new AllAccountSettings
            {
                Credentials = creds,
                Devices = devices,
                SolarRealTimeImportSchedule = solar
            };
        }

        public async Task ResetSettings()
        {
            if (environment.IsMobileApp)
            {
                var settingsFromServer = await serverApi.GetAccountSettings();

                await accountDataStore.SaveAllAccountCredentials(settingsFromServer.Credentials);
                await accountDataStore.SaveDevices(settingsFromServer.Devices);
                await accountDataStore.SaveSolarRealTimeImportSchedule(settingsFromServer.SolarRealTimeImportSchedule);
            }
        }
    }
}
