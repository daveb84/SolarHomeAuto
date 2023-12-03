using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.SolarUsage.Models
{
    public class SolarScheduledJobSettings
    {
        public int RealTimeInterval { get; set; }
        public int RealTimeInitialWait { get; set; }
    }
}
