namespace SolarHomeAuto.Domain.Auth.Models
{
    public class AuthSettings
    {
        public string AuthKeyHash { get; set; }
        public string ApiKeyHash { get; set; }
        public bool AllowAnyAuth { get; set; }
        public bool AllowHttp { get; set; }
    }
}
