using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.Devices.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.UI.Models
{
    public class DeviceEditModel
    {

        public event EventHandler<EventArgs> SchedulesChanged;
        public void NotifySchedulesChanged(object sender, EventArgs e)
            => SchedulesChanged?.Invoke(sender, e);

        [Required]
        public string DeviceId { get; set; }

        [Required]
        public string Name { get; set; }

        [ValidateComplexType]
        public List<DeviceEditScheduleModel> Schedules { get; set; } = new List<DeviceEditScheduleModel>();

        public void CopyFrom(AccountDevice data)
        {
            DeviceId = data.DeviceId;
            Name = data.Name;

            Schedules = data.Schedules
                .Select((x, i) => new DeviceEditScheduleModel(this, i, x))
                .ToList();
        }

        public void CopyTo(AccountDevice data) 
        {
            data.DeviceId = DeviceId;
            data.Name = Name;

            data.Schedules.Clear();

            foreach(var schedule in Schedules.OrderBy(x => x.Time)) 
            {
                var newSched = new DeviceSchedule
                {
                    DeviceId = this.DeviceId
                };

                schedule.CopyTo(newSched);
                data.Schedules.Add(newSched);
            }
        }

        public void AddSchedule()
        {
            var newSchedule = new DeviceEditScheduleModel(this, Schedules.Count, null);
            Schedules.Add(newSchedule);

            NotifySchedulesChanged(Schedules, EventArgs.Empty);
        }

        public void RemoveSchedule(DeviceEditScheduleModel schedule)
        {
            Schedules.Remove(schedule);

            for (var i = 0; i < Schedules.Count; i++)
            {
                Schedules[i].Index = i;
            }

            NotifySchedulesChanged(Schedules, EventArgs.Empty);
        }

        public void SortSchedules()
        {
            var ordered = Schedules.OrderBy(x => x.Time).ToList();

            for (var i = 0; i < ordered.Count; i++)
            {
                ordered[i].Index = i;
            }

            Schedules = ordered;

            NotifySchedulesChanged(Schedules, EventArgs.Empty);
        }
    }

    public class DeviceEditScheduleModel
    {
        private readonly DeviceEditModel parent;

        public DeviceEditScheduleModel(DeviceEditModel parent, int index, DeviceSchedule data)
        {
            this.parent = parent;
            Index = index;

            if (data != null)
            {
                CopyFrom(data);
            }
        }

        public int Index { get; set; }

        public TimeSpan Time { get; set; }
        public int DeviceOrder { get; set; }
        public int DelaySeconds { get; set; }

        public DeviceScheduleAction Action { get; set; }
        public bool ShowConditions => Action == DeviceScheduleAction.Conditional;

        public string TurnOnCondition { get; set; }
        public string TurnOffCondition { get; set; }
        public int ConditionRequiredDeviceHistorySeconds { get; set; }

        public void CopyFrom(DeviceSchedule data)
        {
            Time = data.Time;
            DeviceOrder = data.DeviceOrder;
            DelaySeconds = data.DelaySeconds;
            Action = data.Action;
            TurnOnCondition = data.TurnOnCondition;
            TurnOffCondition = data.TurnOffCondition;
            ConditionRequiredDeviceHistorySeconds = data.ConditionRequiredDeviceHistorySeconds;
        }

        public void CopyTo(DeviceSchedule data)
        {
            data.Time = Time;
            data.DeviceOrder = DeviceOrder;
            data.DelaySeconds = DelaySeconds;
            data.Action = Action;
            data.TurnOnCondition = TurnOnCondition;
            data.TurnOffCondition = TurnOffCondition;
            data.ConditionRequiredDeviceHistorySeconds = ConditionRequiredDeviceHistorySeconds;
        }

        public void Remove()
        {
            parent.RemoveSchedule(this);
        }
    }
}
