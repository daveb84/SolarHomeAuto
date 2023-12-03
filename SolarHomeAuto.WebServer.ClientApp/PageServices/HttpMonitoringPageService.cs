using SolarHomeAuto.Domain.MobileApp;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.UI.Services;
using System.Threading.Tasks;

namespace SolarHomeAuto.WebServer.ClientApp.PageServices
{
    public class HttpMonitoringPageService : IMonitoringPageService
    {
        private readonly PageServiceHttpClient httpClient;

        public HttpMonitoringPageService(PageServiceHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task AppInit()
        {
            return SendRequest<string>(HttpMethod.Post, $"/api/UI/monitoring/init");
        }

        public Task<MonitoringEnvironment> GetEnvironment()
        {
            return SendRequest<MonitoringEnvironment>(HttpMethod.Get, $"/api/UI/monitoring/environment");
        }

        public Task UpdateEnvironment(MonitoringEnvironment data)
        {
            return SendRequest<string>(HttpMethod.Post, $"/api/UI/monitoring/environment", data);
        }

        public Task<bool> IsMobileMonitoringHost()
        {
            return SendRequest<bool>(HttpMethod.Get, $"/api/UI/monitoring/mobileservice/enabled");
        }

        public Task<MobileAppServiceStatus> GetMobileAppServiceStatus()
        {
            return SendRequest<MobileAppServiceStatus>(HttpMethod.Get, $"/api/UI/monitoring/mobileservice/status");
        }

        public Task<MonitoringWorkerStatus> GetWorkerStatus()
        {
            return SendRequest<MonitoringWorkerStatus>(HttpMethod.Get, $"/api/UI/monitoring/worker/status");
        }

        public Task<MobileAppServiceStatus> ToggleMobileAppServiceStatus(bool start)
        {
            return SendRequest<MobileAppServiceStatus>(HttpMethod.Post, $"/api/UI/monitoring/mobileservice/toggle", start);
        }

        public Task<MonitoringWorkerStatus> ToggleWorkerService(bool start)
        {
            return SendRequest<MonitoringWorkerStatus>(HttpMethod.Post, $"/api/UI/monitoring/worker/toggle", start);
        }

        private Task<T> SendRequest<T>(HttpMethod method, string url, object body = null) => httpClient.SendRequest<T>(method, url, body);
    }
}
