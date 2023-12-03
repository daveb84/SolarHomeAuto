using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.ServerApi.Models;
using SolarHomeAuto.Domain.SolarUsage.Models;
using System.ComponentModel.DataAnnotations;

namespace SolarHomeAuto.UI.Services.Default
{
    public class AccountPageService : IAccountPageService
    {
        private readonly AccountDataStore accountDataStore;
        private readonly ILogger<AccountPageService> logger;

        public AccountPageService(AccountDataStore accountDataStore, ILogger<AccountPageService> logger)
        {
            this.accountDataStore = accountDataStore;
            this.logger = logger;
        }

        public async Task<AccountDevice> GetDevice(string deviceId)
        {
            var device = (await accountDataStore.GetDevices()).FirstOrDefault(x => x.DeviceId == deviceId);

            return device;
        }

        public Task<List<AccountDevice>> GetDevices() => accountDataStore.GetDevices();
        public Task<ServerApiAccount> GetServerApiAccount() => accountDataStore.GetServerApiAccount();

        public async Task<string> GetServiceCredentials()
        {
            var data = await accountDataStore.GetAllAccountCredentials();

            return data?.ToString(Formatting.Indented);
        }

        public Task<List<SolarRealTimeSchedulePeriod>> GetSolarRealTimeImportSchedule() => accountDataStore.GetSolarRealTimeImportSchedule();

        public Task<bool> SaveDevice(string deviceId, AccountDevice device) => accountDataStore.SaveDevice(deviceId, device);

        public Task<bool> SaveDevices(List<AccountDevice> model) => accountDataStore.SaveDevices(model);

        public Task<bool> SaveServerApiAccount(ServerApiAccount model) => SaveSetting(x => accountDataStore.SaveServerApiAccount(x), model);

        public async Task<bool> SaveServiceCredentials(string json)
        {
            try
            {
                var data = string.IsNullOrWhiteSpace(json)
                    ? (JObject)null
                    : JObject.Parse(json);

                await accountDataStore.SaveAllAccountCredentials(data);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save account credentials");
                return false;
            }

            return true;
        }

        public Task<bool> SaveSolarRealTimeImportSchedule(List<SolarRealTimeSchedulePeriod> model) => accountDataStore.SaveSolarRealTimeImportSchedule(model);

        private async Task<bool> SaveSetting<T>(Func<T, Task> save, T data)
        {
            if (data != null)
            {
                var results = new List<ValidationResult>();
                var context = new ValidationContext(data, null, null);

                if (!Validator.TryValidateObject(data, context, results))
                {
                    return false;
                }
            }

            await save(data);

            return true;
        }
    }
}
