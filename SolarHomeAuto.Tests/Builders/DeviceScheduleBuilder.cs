using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.SolarExcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Tests.Builders
{
    public class DeviceScheduleBuilder
    {

        public static DeviceSchedule FirstTurnOff()
        {
            return new DeviceSchedule
            {
                Time = TimeSpan.Zero,
                Action = DeviceScheduleAction.TurnOff
            };
        }

        public static DeviceSchedule EveningTurnOff(TimeSpan startOfEvening)
        {
            return new DeviceSchedule
            {
                Time = startOfEvening,
                Action = DeviceScheduleAction.TurnOff
            };
        }

        public static DeviceSchedule BasicAction(TimeSpan time, DeviceScheduleAction action) 
        {
            return new DeviceSchedule
            {
                Time = time,
                Action = action
            };
        }

        public static DeviceSchedule Conditional(TimeSpan time, string turnOn, string turnOff)
        {
            return new DeviceSchedule
            {
                Time = time,
                Action = DeviceScheduleAction.Conditional,
                TurnOnCondition = turnOn,
                TurnOffCondition = turnOff,
            };
        }

        public static string TurnOnWhenGridFeedIn()
        {
            return $"Duration(300, x => x.GridFeedIn > 0 && x.Production > 500 && x.DeviceOff)";
        }

        public static string TurnOffWhenGridPurchase()
        {
            return $"GridPurchase > 0";
        }

        public static DeviceSchedule DayTimeSolarExcess(TimeSpan start)
        {
            return new DeviceSchedule
            {
                Action = DeviceScheduleAction.Conditional,
                Time = start,
                TurnOnCondition = TurnOnWhenGridFeedIn(),
                TurnOffCondition = TurnOffWhenGridPurchase(),
            };
        }
    }
}
