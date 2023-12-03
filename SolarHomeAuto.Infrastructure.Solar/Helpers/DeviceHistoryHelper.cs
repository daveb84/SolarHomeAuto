using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.Solar.Models;
using System.Globalization;

namespace SolarHomeAuto.Infrastructure.Solar.Helpers
{
    internal static class DeviceHistoryHelper
    {
        public static string GetRequestDateFormat(SolarStatsDuration duration)
        {
            var format = duration switch
            {
                SolarStatsDuration.Day => "yyyy-MM-dd",
                SolarStatsDuration.Month => "yyyy-MM",
                SolarStatsDuration.Year => "yyyy",
                _ => throw new NotSupportedException($"{duration} is not supported")
            };

            return format;
        }

        public static string GetResponseDateFormat(SolarStatsDuration duration) => GetRequestDateFormat(duration);

        public static SolarStats Convert(SolarStatsDuration duration, DeviceHistoryData data)
        {
            var format = GetResponseDateFormat(duration);

            var date = DateTime.ParseExact(data.CollectTime, format, CultureInfo.InvariantCulture);

            return new SolarStats
            {
                Date = date,
                Duration = duration,
                Generation = GetDataItem(data, "generation"),
                Consumption = GetDataItem(data, "consumption"),
                GridFeedIn = GetDataItem(data, "grid"),
                GridPurchase = GetDataItem(data, "purchase"),
                ChargeCapacity = GetDataItem(data, "charge"),
                DischargeCapacity = GetDataItem(data, "discharge"),
            };
        }

        private static decimal? GetDataItem(DeviceHistoryData data, string key)
        {
            return data.DataList
                .FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase))?.Value;
        }
    }
}
