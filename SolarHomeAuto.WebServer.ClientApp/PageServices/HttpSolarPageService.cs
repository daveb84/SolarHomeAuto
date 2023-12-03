using SolarHomeAuto.Domain.DataStore.Filtering;
using SolarHomeAuto.Domain.Utils;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.UI.Services;

namespace SolarHomeAuto.WebServer.ClientApp.PageServices
{
    public class HttpSolarPageService : ISolarPageService
    {
        private readonly PageServiceHttpClient httpClient;

        public HttpSolarPageService(PageServiceHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<List<SolarRealTime>> GetSolarRealTime(PagingFilter filter)
        {
            var qs = filter.ToQueryString();

            return SendRequest<List<SolarRealTime>>(HttpMethod.Get, $"/api/UI/solar/realtime{qs}");
        }

        private Task<T> SendRequest<T>(HttpMethod method, string url, object body = null) => httpClient.SendRequest<T>(method, url, body);
    }
}
