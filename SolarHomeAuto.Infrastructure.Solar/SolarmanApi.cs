using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.Solar.Helpers;
using SolarHomeAuto.Infrastructure.Solar.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.Account.Models;

namespace SolarHomeAuto.Infrastructure.Solar
{
    public class SolarmanApi : ISolarApi
    {
        private const string AuthFailedCode = "2101019";

        private readonly HttpClient httpClient;
        private readonly IDataStoreFactory dataStoreFactory;
        private readonly IAccountCredentialsService settingsService;

        public SolarmanApi(HttpClient httpClient, IDataStoreFactory dataStoreFactory, IAccountCredentialsService solarAccount)
        {
            this.httpClient = httpClient;
            this.dataStoreFactory = dataStoreFactory;
            this.settingsService = solarAccount;
        }

        public async IAsyncEnumerable<SolarStats> GetSolarStats(SolarStatsDuration duration, DateTime from, DateTime to)
        {
            var settings = await GetSolarAccount();

            if (!settings.Enabled)
            {
                yield break;
            }

            var token = await GetToken(settings);

            var batches = new DeviceHistoryBatchSplitter(duration).GetBatches(from, to);

            foreach (var (batchFrom, batchTo) in batches)
            {
                var url = $"/device/v1.0/historical?appId={settings.AccountId}&language={settings.Language}";

                var dateFormat = DeviceHistoryHelper.GetRequestDateFormat(duration);

                var request = new DeviceHistoryRequest
                {
                    DeviceSn = settings.DeviceSn,
                    StartTime = batchFrom.ToString(dateFormat),
                    EndTime = batchTo.ToString(dateFormat),
                    TimeType = duration switch
                    {
                        SolarStatsDuration.Day => 2,
                        SolarStatsDuration.Month => 3,
                        SolarStatsDuration.Year => 4,
                        _ => throw new InvalidOperationException($"{duration} is invalid")
                    }
                };

                var data = await SendRequest<DeviceHistoryRequest, DeviceHistoryResponse>(settings, HttpMethod.Post, url, request);

                var converted = data.ParamDataList
                    .Select(x => DeviceHistoryHelper.Convert(SolarStatsDuration.Day, x));

                foreach (var result in converted)
                {
                    yield return result;
                }
            }
        }

        private async Task<SolarAccount> GetSolarAccount()
        {
            return (await settingsService.GetAccountCredentials<SolarAccount>("Solarman")) ?? new SolarAccount();
        }

        public async Task<SolarRealTime> GetSolarRealTime()
        {
            var settings = await GetSolarAccount();

            if (!settings.Enabled)
            {
                return null;
            }

            var url = $"/device/v1.0/currentData?appId={settings.AccountId}&language={settings.Language}";

            var request = new DeviceRealTimeDataRequest
            {
                DeviceSn = settings.DeviceSn,
            };

            var data = await SendRequest<DeviceRealTimeDataRequest, DeviceRealTimeDataResponse>(settings, HttpMethod.Post, url, request);

            var result = DeviceRealTimeHelper.Convert(data);

            return result;
        }

        private async Task<AccountOAuthToken> GetToken(SolarAccount settings, bool refresh = false)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var token = await store.GetAccountOAuthToken(settings.ServiceId);

                if (token == null || refresh)
                {
                    var url = $"{settings.BaseUrl}/account/v1.0/token?appId={settings.AccountId}";

                    var request = new TokenRequest
                    {
                        AppSecret = settings.AppSecret,
                        CountryCode = settings.CountryCode,
                        Email = settings.Email,
                        Password = settings.Password,
                    };

                    var response = await httpClient.PostAsJsonAsync(url, request);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    TokenResponse tokenData;

                    try
                    {
                        tokenData = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
                    }
                    catch (Exception ex)
                    {
                        throw new SolarDeserializationFailedException(responseContent, ex);
                    }

                    if (tokenData.Success)
                    {
                        token = tokenData.ToDomain(settings);

                        using (var trans = store.CreateTransaction())
                        {
                            await trans.SaveSolarAuthToken(token);
                            await trans.Commit();
                        }
                    }
                    else
                    {
                        throw new SolarAuthFailedException(tokenData.Message);
                    }
                }

                return token;
            }
        }

        private async Task<TResponse> SendRequest<TRequest, TResponse>(SolarAccount settings, HttpMethod method, string url, TRequest data) where TResponse : IApiResponseStatus
        {
            var token = await GetToken(settings);

            try
            {
                return await SendRequest<TRequest, TResponse>(settings, method, url, data, token);
            }
            catch (SolarAuthFailedException)
            {
                token = await GetToken(settings, true);

                return await SendRequest<TRequest, TResponse>(settings, method, url, data, token);
            }
        }

        private async Task<TResponse> SendRequest<TRequest, TResponse>(SolarAccount settings, HttpMethod method, string url, TRequest data, AccountOAuthToken token) where TResponse : IApiResponseStatus
        {
            url = $"{settings.BaseUrl}{url}";

            var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);

            if (method != HttpMethod.Get)
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
                throw new SolarOperationFailedException(url, ex);
            }

            TResponse responseData;

            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                responseData = JsonConvert.DeserializeObject<TResponse>(responseString);
            }
            catch (Exception ex)
            {
                throw new SolarDeserializationFailedException(responseString, ex);
            }

            if (responseData.Success)
            {
                return responseData;
            }
            else if (responseData.Code == AuthFailedCode)
            {
                throw new SolarAuthFailedException(responseData.Message);
            }

            throw new SolarOperationFailedException(url, responseData.Message, responseString);
        }

        public async Task<bool> IsEnabled()
        {
            var account = await GetSolarAccount();

            return account.Enabled;
        }
    }
}
