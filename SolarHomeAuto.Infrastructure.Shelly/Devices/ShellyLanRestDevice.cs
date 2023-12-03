using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain;
using SolarHomeAuto.Domain.Devices;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Infrastructure.Shelly.Exceptions;

namespace SolarHomeAuto.Infrastructure.Shelly.Devices
{
    public class ShellyLanRestDevice : SwitchDeviceBase
    {
        private readonly ShellyDeviceConfig config;
        private readonly HttpClient httpClient;
        private readonly ILogger<ShellyLanRestDevice> logger;

        public ShellyLanRestDevice(ShellyDeviceConfig config, HttpClient httpClient, IDeviceService deviceService, ILogger<ShellyLanRestDevice> logger)
            : base(config, deviceService, logger)
        {
            this.config = config;
            this.httpClient = httpClient;
            this.logger = logger;
        }

        protected override async Task<SwitchStatusResult> GetStatusInternal()
        {
            using (var logReporter = new LogReporter("Shelly LAN Get Status", logger, LogLevel.Debug))
            {
                var url = "/rpc/Switch.GetStatus?id=0";

                var (ok, response) = await SendRequest(HttpMethod.Get, url, logReporter, null);

                if (!ok)
                {
                    logReporter.Add("Status failed", SwitchStatus.Offline);

                    return new SwitchStatusResult(SwitchStatus.Offline);
                }

                var status = response.SelectToken("$.output")?.Value<bool>() == true
                    ? SwitchStatus.On
                    : SwitchStatus.Off;

                var power = response.SelectToken("$.apower")?.Value<decimal?>();

                return new SwitchStatusResult(status, power);
            }
        }

        protected override async Task<SwitchActionResult> SwitchInternal(SwitchAction action)
        {
            if (!action.IsOneOf(SwitchAction.TurnOff, SwitchAction.TurnOn))
            {
                return SwitchActionResult.NoAction;
            }

            using (var logReporter = new LogReporter($"Shelly LAN Switch device {action}", logger, LogLevel.Debug))
            {
                var url = "/rpc";

                var data = new SwitchStatusPayload
                {
                    Id = 1,
                    Method = "Switch.Set",
                    Parameters = new SwitchStatusPayloadParams
                    {
                        Id = 0,
                        On = action == SwitchAction.TurnOn
                    }
                };
                logReporter.Add("Data", JsonConvert.SerializeObject(data));

                var (ok, _) = await SendRequest(HttpMethod.Post, url, logReporter, data);

                if (!ok)
                {
                    logReporter.Add("Switch failed");

                    return SwitchActionResult.Failure;
                }

                logReporter.Add("Switch succeeded");
                return SwitchActionResult.Success;
            }
        }

        private async Task<(bool, JObject)> SendRequest(HttpMethod method, string url, LogReporter logReport, object data)
        {
            url = $"http://{config.ShellyLanIPAddress}{url}";

            try
            {
                var request = new HttpRequestMessage(method, url);

                if (method != HttpMethod.Get)
                {
                    request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = new StringContent(JsonConvert.SerializeObject(data));
                }

                using (var response = await httpClient.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new ShellyApiException($"Failed to {method} URL: {url}. Status: {response.StatusCode}");
                    }

                    var content = await response.Content.ReadAsStringAsync();

                    logReport.Add("Response", content);

                    var responseData = JsonConvert.DeserializeObject<JObject>(content);

                    return (true, responseData);
                }
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex, $"Failed to {method} url {url}");

                return (false, null);
            }
        }

        private class SwitchStatusPayload
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("method")]
            public string Method { get; set; }

            [JsonProperty("params")]
            public SwitchStatusPayloadParams Parameters { get; set; }
        }

        private class SwitchStatusPayloadParams
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("on")]
            public bool On { get; set; }
        }
    }
}
