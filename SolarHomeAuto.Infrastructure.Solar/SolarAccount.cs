using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Infrastructure.Solar
{
    public class SolarAccount
    {
        public string ServiceId => "Solarman";
        public bool Enabled => !string.IsNullOrWhiteSpace(BaseUrl);
        public string BaseUrl { get; set; }
        public string AccountId { get; set; }
        public string AppSecret { get; set; }
        public string CountryCode { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Language { get; set; } = "en";
        public string DeviceSn { get; set; }
    }
}
