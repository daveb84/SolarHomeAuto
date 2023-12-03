using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.Monitoring
{
    public enum MonitoringWorkerStatus
    {
        Unknown,
        Started,
        Stopped,
        Stopping,
        Starting
    }
}
