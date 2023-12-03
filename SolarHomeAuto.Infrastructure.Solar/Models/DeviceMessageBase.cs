using Newtonsoft.Json;

namespace SolarHomeAuto.Infrastructure.Solar.Models
{
    public class DeviceMessageBase : IApiResponseStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("deviceSn")]
        public string DeviceSn { get; set; }

        [JsonProperty("deviceId")]
        public string DeviceId { get; set; }

        [JsonProperty("deviceType")]
        public string DeviceType { get; set; }
    }
}
