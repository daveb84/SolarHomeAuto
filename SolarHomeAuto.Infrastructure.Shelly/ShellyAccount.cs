using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Infrastructure.Shelly
{
    public class ShellyAccount
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }

        public bool Enabled => !string.IsNullOrEmpty(BaseUrl);
    }
}
