using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.ServerApi.Models;
using SolarHomeAuto.Domain.SolarUsage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.UI.Services
{
    public interface IAccountPageService
    {
        Task<ServerApiAccount> GetServerApiAccount();
        Task<bool> SaveServerApiAccount(ServerApiAccount model);

        Task<List<AccountDevice>> GetDevices();
        Task<bool> SaveDevices(List<AccountDevice> model);

        Task<AccountDevice> GetDevice(string deviceId);
        Task<bool> SaveDevice(string deviceId, AccountDevice device);

        Task<List<SolarRealTimeSchedulePeriod>> GetSolarRealTimeImportSchedule();
        Task<bool> SaveSolarRealTimeImportSchedule(List<SolarRealTimeSchedulePeriod> model);

        Task<string> GetServiceCredentials();
        Task<bool> SaveServiceCredentials(string json);
    }
}
