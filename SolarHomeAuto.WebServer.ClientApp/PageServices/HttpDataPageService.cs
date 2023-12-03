using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.UI.Services;

namespace SolarHomeAuto.WebServer.ClientApp.PageServices
{
    public class HttpDataPageService : IDataPageService
    {
        private readonly PageServiceHttpClient httpClient;

        public HttpDataPageService(PageServiceHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public bool IsMobileApp => false;

        public Task<List<LogEntry>> GetLogs(LogFilter filter)
        {
            return SendRequest<List<LogEntry>>(HttpMethod.Post, $"/api/UI/logs/query", filter);
        }

        public Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter)
        {
            return SendRequest<List<RemoteCommandMessage>>(HttpMethod.Post, $"/api/UI/commands/query", filter);
        }

        public Task PurgeData()
        {
            return SendRequest<string>(HttpMethod.Post, $"/api/UI/data/purge");
        }

        public Task ResetSettings()
        {
            return SendRequest<string>(HttpMethod.Post, $"/api/UI/settings/reset");
        }

        private Task<T> SendRequest<T>(HttpMethod method, string url, object body = null) => httpClient.SendRequest<T>(method, url, body);
    }
}
