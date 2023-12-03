namespace SolarHomeAuto.Domain.Account.Models
{
    public class AccountOAuthToken
    {
        public string ServiceId { get; set; }
        public string AccountId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expires { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
