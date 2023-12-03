using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.Monitoring
{
    public class MonitoringEnvironment
    {
        public MonitoringServiceMode Mode { get; set; }
        public bool HasLanAccess { get; set; }
    }
}
