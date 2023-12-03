using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.SolarUsage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.SolarExcess
{
    public class DeviceAndSolarHistory
    {
        public DateTime Date { get; set; }
        public decimal Production { get; set; }
        public decimal Consumption { get; set; }
        public decimal GridPower { get; set; }
        public decimal GridFeedIn { get; set; }
        public decimal GridPurchase { get; set; }
        public decimal BatteryCapacity { get; set; }
        public decimal BatteryPower { get; set; }
        public decimal BatteryCharging { get; set; }
        public decimal BatteryDischarging { get; set; }

        public bool DeviceOn { get; set; }
        public bool DeviceOff { get; set; }

        private void AssignData(SolarRealTime data)
        {
            if (data != null)
            {
                this.Production = data.Production ?? 0;
                this.Consumption = data.Consumption ?? 0;

                this.GridPower = data.GridPower ?? 0;
                this.GridFeedIn = data.GetGridPower(GridPowerType.FeedIn, true);
                this.GridPurchase = data.GetGridPower(GridPowerType.Purchase, true);

                this.BatteryCapacity = data.BatteryCapacity ?? 0;

                this.BatteryPower = data.BatteryPower ?? 0;
                this.BatteryCharging = data.GetBatteryPower(BatteryPowerType.Charging, true);
                this.BatteryDischarging = data.GetBatteryPower(BatteryPowerType.Discharging, true);
            }
        }

        private void AssignData(DeviceHistory<SwitchHistoryState> data)
        {
            this.DeviceOn = data?.State.Status == SwitchStatus.On;
            this.DeviceOff = !this.DeviceOn;
        }

        public static List<DeviceAndSolarHistory> CreateHistory(List<SolarRealTime> solarHistory, List<DeviceHistory<SwitchHistoryState>> deviceHistory)
        {
            var data = solarHistory
                .Select(x => new WorkingData { Solar = x })
                .Concat(deviceHistory.Select(x => new WorkingData { Device = x }))
                .OrderBy(x => x.Date)
                .ToList();

            for (int i = 0; i < data.Count; i++)
            {
                data[i].Index = i;
            }

            var results = new List<DeviceAndSolarHistory>();

            foreach (var item in data)
            {
                var previousOther = data
                    .Where(x => x.IsSolar != item.IsSolar && x.Index < item.Index)
                    .LastOrDefault();

                var history = new DeviceAndSolarHistory
                {
                    Date = item.Date,
                };

                history.AssignData(item.Solar ?? previousOther?.Solar);
                history.AssignData(item.Device ?? previousOther?.Device);

                results.Add(history);
            }

            return results;
        }

        private class WorkingData
        {
            public int Index { get; set; }

            public bool IsSolar => Solar != null;
            public DateTime Date => Solar?.Date ?? Device.Date;

            public SolarRealTime Solar { get; set; }
            public DeviceHistory<SwitchHistoryState> Device { get; set; }
        }
    }
}
