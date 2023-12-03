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
    internal class DeviceJobRun
    {
        public DeviceJobRun()
        {
        }

        public DeviceJobRun(DeviceJobRunState state)
        {
            this.DeviceId = state.DeviceId;
            this.NextRun = state.NextRun;
            this.GlobalWaitUntil = state.GlobalWaitUntil;
        }

        public string DeviceId { get; set; }
        public DateTime NextRun { get; set; }
        public DateTime GlobalWaitUntil { get; set; }

        public DeviceSchedule NextRunSchedule { get; set; }
        public ISchedule<DeviceSchedule> ScheduleConfig { get; set; }

        public void PrepareForNextRun(DateTime now, List<DeviceSchedule> allSchedules)
        {
            ScheduleConfig = new DailySchedule<DeviceSchedule>(allSchedules.Where(x => x.DeviceId == DeviceId).ToList());

            NextRunSchedule = ScheduleConfig.GetCurrentPeriod(now);
        }

        public void UpdateRunResult(DateTime now, SwitchActionResult result, DeviceScheduleSettings solarConfig)
        {
            if (NextRunSchedule.Action.IsOneOf(DeviceScheduleAction.TurnOn, DeviceScheduleAction.TurnOff))
            {
                if (result == SwitchActionResult.Success)
                {
                    var nextPeriod = ScheduleConfig.GetNextPeriod(NextRunSchedule);

                    NextRun = ScheduleConfig.GetPeriodNextStartTime(nextPeriod, now);
                    GlobalWaitUntil = NextRunSchedule.DelaySeconds > 0
                        ? now.AddSeconds(NextRunSchedule.DelaySeconds)
                        : DateTime.MinValue;
                }
                else
                {
                    NextRun = now.AddSeconds(solarConfig.IntervalSeconds);
                    GlobalWaitUntil = DateTime.MinValue;
                }
            }
            else
            {
                NextRun = now.AddSeconds(solarConfig.IntervalSeconds);

                if (result == SwitchActionResult.Success && NextRunSchedule.Action == DeviceScheduleAction.Conditional)
                {
                    GlobalWaitUntil = NextRunSchedule.DelaySeconds > 0 
                        ? now.AddSeconds(NextRunSchedule.DelaySeconds)
                        : DateTime.MinValue;
                }
                else
                {
                    GlobalWaitUntil = DateTime.MinValue;
                }
            }

            if (NextRun <= now)
            {
                NextRun = NextRun.AddSeconds(solarConfig.IntervalSeconds);
            }

            NextRunSchedule = null;
            ScheduleConfig = null;
        }

        public DeviceJobRunState ToSaveState()
        {
            return new DeviceJobRunState
            {
                DeviceId = DeviceId,
                NextRun = NextRun,
                GlobalWaitUntil = GlobalWaitUntil,
            };
        }
    }
}
