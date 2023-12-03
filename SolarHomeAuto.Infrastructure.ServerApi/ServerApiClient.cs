using Newtonsoft.Json;
using SolarHomeAuto.Domain.ServerApi;
using System.Net.Http.Json;
using SolarHomeAuto.Domain.ServerApi.Models;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.Auth;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.RemoteCommands;

namespace SolarHomeAuto.Infrastructure.ServerApi
{
    public class ServerApiClient : IServerApiClient
    {
        private readonly HttpClient httpClient;
        private readonly IServerApiAccountService authCredentialsService;

        public ServerApiClient(HttpClient client, IServerApiAccountService authCredentialsService) 
        {
            this.httpClient = client;
            this.authCredentialsService = authCredentialsService;
        }

        public async Task WriteLogs(List<LogEntry> logs)
        {
            var settings = await authCredentialsService.GetServerApiAccount();

            if (!settings.Enabled)
            {
                return;
            }

            var url = $"/api/Mobile/Log/Bulk";

            await SendRequest<NoResponse>(settings,HttpMethod.Post, url, logs);
        }

        public async Task UploadData(UploadDataRequest data)
        {
            var settings = await authCredentialsService.GetServerApiAccount();

            if (!settings.Enabled)
            {
                return;
            }

            var url = $"/api/Mobile/StoreData";

            await SendRequest<NoResponse>(settings, HttpMethod.Post, url, data);
        }

        public async Task<AllAccountSettings> GetAccountSettings()
        {
            var settings = await authCredentialsService.GetServerApiAccount();

            if (!settings.Enabled)
            {
                return null;
            }

            var url = $"/api/Mobile/AccountSettings";

            return await SendRequest<AllAccountSettings>(settings, HttpMethod.Get, url);
        }

        public async Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter)
        {
            var settings = await authCredentialsService.GetServerApiAccount();

            if (!settings.Enabled)
            {
                return new List<RemoteCommandMessage>();
            }

            var url = $"/api/Mobile/RemoteCommands/query";

            return await SendRequest<List<RemoteCommandMessage>>(settings, HttpMethod.Post, url, filter);
        }

        public async Task ConsumeRemoteCommands(ConsumeRemoteCommandsModel messages)
        {
            var settings = await authCredentialsService.GetServerApiAccount();

            if (!settings.Enabled)
            {
                return;
            }

            var url = $"/api/Mobile/RemoteCommands/Consume";

            await SendRequest<NoResponse>(settings, HttpMethod.Post, url, messages);
        }

        public async Task PublishRemoteCommand(RemoteCommandMessage message)
        {
            var settings = await authCredentialsService.GetServerApiAccount();

            if (!settings.Enabled)
            {
                return;
            }

            var url = $"/api/Mobile/RemoteCommands/Publish";

            await SendRequest<NoResponse>(settings, HttpMethod.Post, url, message);
        }

        private async Task<TResponse> SendRequest<TResponse>(ServerApiAccount settings, HttpMethod method, string url, object data = null)
        {
            url = $"{settings.BaseUrl}{url}";

            var request = new HttpRequestMessage(method, url);
            request.Headers.Add(AuthConstants.ApiKeyHeaderKey, settings.ApiKey);

            if (method != HttpMethod.Get && data != null)
            {
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = JsonContent.Create(data, mediaType: null, options: null);
            }

            HttpResponseMessage response;

            try
            {
                response = await httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                throw new ServerApiException($"Failed to fetch {url}", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                throw new ServerApiException($"Unsuccessful response from url {url}: {response.StatusCode}");
            }

            if (typeof(TResponse) == typeof(NoResponse))
            {
                return (TResponse)(object)new NoResponse();
            }

            TResponse responseData;

            var responseString = await response.Content.ReadAsStringAsync();

            if (typeof(TResponse) == typeof(string))
            {
                return (TResponse)(object)responseString;
            }

            try
            {
                responseData = JsonConvert.DeserializeObject<TResponse>(responseString);
            }
            catch (Exception ex)
            {
                throw new ServerApiException($"Failed to deserialize {responseString}", ex);
            }

            return responseData;
        }

        private class NoResponse
        {
        }
    }
}
