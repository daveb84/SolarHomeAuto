using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain.Devices.Types;

namespace SolarHomeAuto.Infrastructure.Shelly.Models
{
    public class ShellySwitchStatus
    {
        public bool ApiError { get; set; }
        public bool ApiErrorMaxRequests { get; set; }
        public bool Online { get; set; }
        public bool On { get; set; }

        public decimal? Power { get; set; }

        public SwitchStatus Status
        {
            get
            {
                if (!Online || ApiError || ApiErrorMaxRequests) return SwitchStatus.Offline;
                if (On) return SwitchStatus.On;
                return SwitchStatus.Off;
            }
        }

        public string ErrorMessage
        {
            get
            {
                if (ApiErrorMaxRequests) return "ApiErrorMaxRequests";
                if (ApiError) return "ApiError";
                return null;
            }
        }

        public static ShellySwitchStatus FromJson(JObject json)
        {
            var result = new ShellySwitchStatus();

            var ok = json.SelectToken("$.isok")?.Value<bool>();

            if (ok != true)
            {
                var maxRequests = json.SelectToken("$.errors.max_req")?.Value<string>();

                if (!string.IsNullOrEmpty(maxRequests))
                {
                    result.ApiErrorMaxRequests = true;
                }
                else
                {
                    result.ApiError = true;
                }
                return result;
            }

            result.Online = json.SelectToken("$.data.online")?.Value<bool>() ?? false;
            result.On = json.SelectToken("$.data.device_status.switch:0.output")?.Value<bool>() ?? false;
            result.Power = json.SelectToken("$.data.device_status.switch:0.apower")?.Value<decimal?>();

            return result;
        }
    }
}
