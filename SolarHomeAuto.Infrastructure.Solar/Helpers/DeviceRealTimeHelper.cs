using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.Solar.Models;

namespace SolarHomeAuto.Infrastructure.Solar.Helpers
{
    internal static class DeviceRealTimeHelper
    {
        public static SolarRealTime Convert(DeviceRealTimeDataResponse data)
        {
            var result = new SolarRealTime()
            {
                Date = data.CollectionTime,
                Production = GetDecimal(data, "DPi_t1"),
            };

            var gridPower = GetDecimal(data, "PG_Pt1");

            result.GridPurchasing = gridPower < 0;
            result.GridPower = gridPower.HasValue 
                ? Math.Abs(gridPower.Value) : (decimal?)null;

            result.BatteryCharging = string.Equals(GetString(data, "B_ST1"), "Charging", StringComparison.OrdinalIgnoreCase);
            result.BatteryPower = GetDecimal(data, "B_P1", true);

            result.Consumption = GetDecimal(data, "E_Puse_t1");
            result.BatteryCapacity = GetDecimal(data, "B_left_cap1");

            return result;
        }

        private static decimal? GetDecimal(DeviceRealTimeDataResponse data, string key, bool absolute = false)
        {
            var match = data.DataList
                .FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));

            if (match != null && !string.IsNullOrEmpty(match.Value))
            {
                var value = decimal.Parse(match.Value);

                return absolute ? Math.Abs(value) : value;
            }

            return null;
        }

        private static string GetString(DeviceRealTimeDataResponse data, string key)
        {
            var match = data.DataList
                .FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));

            return match?.Value;
        }
    }
}
