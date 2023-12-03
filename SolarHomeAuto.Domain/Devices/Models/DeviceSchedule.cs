using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SolarHomeAuto.Domain.ScheduledJobs.Schedules;
using System.ComponentModel.DataAnnotations;

namespace SolarHomeAuto.Domain.Devices.Models
{
    public enum DeviceScheduleAction
    {
        [Display(Name = "None")]
        None,

        [Display(Name = "Turn on")]
        TurnOn,

        [Display(Name = "Turn off")]
        TurnOff,

        [Display(Name = "Solar")]
        Conditional,
    }

    public class DeviceSchedule : ISchedulePeriod
    {
        [JsonIgnore]
        public string DeviceId { get; set; }
        public int DeviceOrder { get; set; }
        public int DelaySeconds { get; set; }
        public TimeSpan Time { get;  set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public DeviceScheduleAction Action { get; set; }
        public int ConditionRequiredDeviceHistorySeconds { get; set; }
        public string TurnOnCondition { get; set; }
        public string TurnOffCondition { get; set; }
    }
}
