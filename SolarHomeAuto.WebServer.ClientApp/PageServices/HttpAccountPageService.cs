using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.ServerApi.Models;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.UI.Services;
using System.Reflection;

namespace SolarHomeAuto.WebServer.ClientApp.PageServices
{
    public class HttpAccountPageService : IAccountPageService
    {
        private readonly PageServiceHttpClient httpClient;

        public HttpAccountPageService(PageServiceHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<AccountDevice> GetDevice(string deviceId)
        {
            return SendRequest<AccountDevice>(HttpMethod.Get, $"/api/UI/account/devices/{deviceId}");
        }

        public Task<List<AccountDevice>> GetDevices()
        {
            return SendRequest<List<AccountDevice>>(HttpMethod.Get, $"/api/UI/account/devices");
        }

        public Task<ServerApiAccount> GetServerApiAccount()
        {
            return SendRequest<ServerApiAccount>(HttpMethod.Get, $"/api/UI/account/server");
        }

        public Task<string> GetServiceCredentials()
        {
            return SendRequest<string>(HttpMethod.Get, $"/api/UI/account/credentials");
        }

        public Task<List<SolarRealTimeSchedulePeriod>> GetSolarRealTimeImportSchedule()
        {
            return SendRequest<List<SolarRealTimeSchedulePeriod>>(HttpMethod.Get, $"/api/UI/account/solarschedule");
        }

        public Task<bool> SaveDevice(string deviceId, AccountDevice device)
        {
            return SendRequest<bool>(HttpMethod.Post, $"/api/UI/account/devices/{deviceId}", device);
        }

        public Task<bool> SaveDevices(List<AccountDevice> model)
        {
            return SendRequest<bool>(HttpMethod.Post, $"/api/UI/account/devices", model);
        }

        public Task<bool> SaveServerApiAccount(ServerApiAccount model)
        {
            return SendRequest<bool>(HttpMethod.Post, $"/api/UI/account/server", model);
        }

        public Task<bool> SaveServiceCredentials(string json)
        {
            return SendRequest<bool>(HttpMethod.Post, $"/api/UI/account/credentials", json);
        }

        public Task<bool> SaveSolarRealTimeImportSchedule(List<SolarRealTimeSchedulePeriod> model)
        {
            return SendRequest<bool>(HttpMethod.Post, $"/api/UI/account/solarschedule", model);
        }

        private Task<T> SendRequest<T>(HttpMethod method, string url, object body = null) => httpClient.SendRequest<T>(method, url, body);
    }
}
