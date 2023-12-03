using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SolarHomeAuto.Domain.ScheduledJobs.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.SolarUsage.Models
{
    public enum SolarRealTimeScheduleAction
    {
        FetchData,
        Stop,
        StopAndPurgeData
    }

    public class SolarRealTimeSchedulePeriod : ISchedulePeriod
    {
        public TimeSpan Time { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public SolarRealTimeScheduleAction Action { get; set; }
    }
}
