using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.Tests.Builders
{
    public class SolarRealTimeScheduleBuilder
    {
        public static List<SolarRealTimeSchedulePeriod> AlwaysInactive()
        {
            return new List<SolarRealTimeSchedulePeriod>()
            {
                new SolarRealTimeSchedulePeriod()
                {
                    Action = SolarRealTimeScheduleAction.Stop,
                    Time = new TimeSpan(0, 0, 0),
                }
            };
        }

        public static List<SolarRealTimeSchedulePeriod> AlwaysActive()
        {
            return new List<SolarRealTimeSchedulePeriod>()
            {
                new SolarRealTimeSchedulePeriod()
                {
                    Action = SolarRealTimeScheduleAction.FetchData,
                    Time = new TimeSpan(0, 0, 0)
                }
            };
        }

        public static List<SolarRealTimeSchedulePeriod> ActiveDayTime(TimeSpan? activeStart = null, TimeSpan? activeEnd = null)
        {
            return new List<SolarRealTimeSchedulePeriod>()
            {
                new SolarRealTimeSchedulePeriod()
                {
                    Action = SolarRealTimeScheduleAction.Stop,
                    Time = new TimeSpan(0, 0, 0)
                },
                new SolarRealTimeSchedulePeriod()
                {
                    Action = SolarRealTimeScheduleAction.FetchData,
                    Time = activeStart ?? new TimeSpan(5, 0, 0)
                },
                new SolarRealTimeSchedulePeriod()
                {
                    Action = SolarRealTimeScheduleAction.Stop,
                    Time = activeEnd ?? new TimeSpan(21, 0, 0)
                },
            };
        }
    }
}
