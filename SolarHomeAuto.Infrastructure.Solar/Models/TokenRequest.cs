using Newtonsoft.Json;

namespace SolarHomeAuto.Infrastructure.Solar.Models
{
    public class TokenRequest
    {
        [JsonProperty("appSecret")]
        public string AppSecret { get; set; }

        [JsonProperty("coutryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("password")] 
        public string Password { get; set; }
    }
}
