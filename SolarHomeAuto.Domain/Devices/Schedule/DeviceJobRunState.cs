using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.ScheduledJobs.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.Devices.Schedule
{
    internal class DeviceJobRunState
    {
        public string DeviceId { get; set; }
        public DateTime NextRun { get; set; }
        public DateTime GlobalWaitUntil { get; set; }
    }
}
