using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain.SolarUsage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.Account.Models
{
    public class AllAccountSettings
    {
        public JObject Credentials { get; set; }
        public List<AccountDevice> Devices { get; set; }
        public List<SolarRealTimeSchedulePeriod> SolarRealTimeImportSchedule { get; set; }
    }
}
