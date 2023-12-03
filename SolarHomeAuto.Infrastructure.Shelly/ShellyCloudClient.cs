using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain;
using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Infrastructure.Shelly.Exceptions;
using SolarHomeAuto.Infrastructure.Shelly.Models;

namespace SolarHomeAuto.Infrastructure.Shelly
{
    public class ShellyCloudClient : IShellyCloudClient
    {
        private readonly HttpClient client;
        private readonly IAccountCredentialsService settingsService;
        private readonly ILogger<ShellyCloudClient> logger;

        private static DateTime? lastRequest;
        private static SemaphoreSlim lastRequestLock = new SemaphoreSlim(1, 1);

        public ShellyCloudClient(HttpClient client, IAccountCredentialsService settingsService, ILogger<ShellyCloudClient> logger)
        {
            this.client = client;
            this.settingsService = settingsService;
            this.logger = logger;
        }

        public async Task<ShellySwitchStatus> GetSwitchStatus(string shellyId)
        {
            var settings = await GetShellyAccount();

            if (!settings.Enabled)
            {
                return new ShellySwitchStatus
                {
                    ApiError = true
                };
            }

            using (var logReport = new LogReporter("Shelly Get Status", logger, LogLevel.Debug))
            {
                logReport.Add("DeviceID", shellyId);

                bool ok = false;
                JObject response = null;

                var result = new ShellySwitchStatus();

                try
                {
                    (ok, response) = await SendRequest(settings, HttpMethod.Post, "/device/status", shellyId, logReport, throwOnError: true, throwOnRateLimitError: true);
                }
                catch (ShellyApiException ex)
                {
                    result.ApiError = true;
                    logReport.Add("Result: Api error", ex.Message);
                }
                catch (ShellyApiLimitException ex)
                {
                    result.ApiErrorMaxRequests = true;
                    logReport.Add("Result: Api error max requests", ex.Message);
                }

                if (ok)
                {
                    var online = response.SelectToken("$.data.online")?.Value<bool>();
                    var on = response.SelectToken("$.data.device_status.switch:0.output")?.Value<bool>();

                    logReport.Add("Result: online", online);
                    logReport.Add("Result: on", on);

                    result.Online = online.GetValueOrDefault();
                    result.On = on.GetValueOrDefault();
                }

                return result;
            }
        }

        public async Task<bool> Switch(string shellyId, bool on)
        {
            var settings = await GetShellyAccount();

            if (!settings.Enabled)
            {
                return false;
            }

            using (var logReport = new LogReporter("Shelly Switch", logger, LogLevel.Debug))
            {
                logReport.Add("DeviceID", shellyId);
                logReport.Add("On", on);

                var formData = new Dictionary<string, string>()
                {
                    { "channel", "0" },
                    { "turn", on ? "on" : "off" }
                };

                var (ok, _) = await SendRequest(settings, HttpMethod.Post, "/device/relay/control", shellyId, logReport, formData);

                return ok;
            }
        }

        private async Task<ShellyAccount> GetShellyAccount()
        {
            return (await settingsService.GetAccountCredentials<ShellyAccount>("ShellyCloud")) ?? new ShellyAccount();
        }

        private async Task<(bool, JObject)> SendRequest(ShellyAccount settings, HttpMethod method, string url, string shellyId, LogReporter logReport, Dictionary<string, string> formParams = null, bool throwOnError = false, bool throwOnRateLimitError = false)
        {
            if (WaitRequired)
            {
                await DateTimeNow.Delay(2000);
            }

            await lastRequestLock.WaitAsync();

            JObject responseData = null;

            try
            {
                lastRequest = DateTimeNow.UtcNow;

                url = $"{settings.BaseUrl}{url}";

                var request = new HttpRequestMessage(method, url);

                var dict = formParams ?? new Dictionary<string, string>();
                dict["id"] = shellyId;
                dict["auth_key"] = settings.ApiKey;

                logReport.AddCollection(dict, x => x.Key, x => x.Value);

                request.Content = new FormUrlEncodedContent(dict);

                using (var response = await client.SendAsync(request))
                {
                    var content = await response.Content.ReadAsStringAsync();

                    logReport.Add("Response", content);

                    responseData = JsonConvert.DeserializeObject<JObject>(content);

                    var ok = responseData.SelectToken("$.isok")?.Value<bool>();

                    if (ok == true || (!throwOnError && !throwOnRateLimitError))
                    {
                        return (ok.GetValueOrDefault(), responseData);
                    }

                    var maxRequests = responseData.SelectToken("$.errors.max_req")?.Value<string>();

                    if (!string.IsNullOrWhiteSpace(maxRequests))
                    {
                        if (throwOnRateLimitError)
                        {
                            throw new ShellyApiLimitException(maxRequests);
                        }

                        return (false, responseData);
                    }

                    if (throwOnError)
                    {
                        throw new ShellyApiException($"Unsuccessful response: {content}");
                    }

                    return (false, responseData);
                }
            }
            catch (ShellyApiException ex)
            {
                logger.LogError(ex, "Shelly API response error");

                throw;
            }
            catch (ShellyApiLimitException ex)
            {
                logger.LogError(ex, "Shelly API max requests error");

                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Shelly API Error");

                if (throwOnError)
                {
                    throw new ShellyApiException("Shelly API Error", ex);
                }

                return (false, responseData);
            }
            finally
            {
                lastRequestLock.Release();
            }
        }

        private bool WaitRequired => lastRequest.HasValue && lastRequest > DateTimeNow.UtcNow.AddSeconds(-2);
    }
}
