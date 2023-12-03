using Newtonsoft.Json;
using SolarHomeAuto.Domain;
using SolarHomeAuto.Domain.Account.Models;

namespace SolarHomeAuto.Infrastructure.Solar.Models
{
    public class TokenResponse : IApiResponseStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }
        
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("uid")]
        public int Uid { get; set; }

        public AccountOAuthToken ToDomain(SolarAccount settings)
        {
            return new AccountOAuthToken
            {
                ServiceId = settings.ServiceId,
                AccessToken = AccessToken,
                RefreshToken = RefreshToken,
                Created = DateTimeNow.UtcNow,
                Expires = DateTimeNow.UtcNow.AddSeconds(ExpiresIn),
                AccountId = settings.AccountId
            };
        }
    }
}
