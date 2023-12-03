using SolarHomeAuto.Domain;
using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.Tests.Builders
{
    public class SolarRealTimeBuilder
    {
        private SolarRealTime data;

        public SolarRealTimeBuilder()
        {
            data = BatteryCharging(DateTimeNow.UtcNow);
        }

        public static SolarRealTime NoSolar(DateTime time)
        {
            return new SolarRealTime()
            {
                Date = time,

                Production = 0,
                Consumption = 500,

                BatteryCapacity = 50,
                BatteryCharging = false,
                BatteryPower = 500,

                GridPower = 0,
                GridPurchasing = false,
            };
        }

        public static SolarRealTime Purchasing(DateTime time)
        {
            return new SolarRealTime()
            {
                Date = time,

                Production = 500,
                Consumption = 2000,

                BatteryCapacity = 50,
                BatteryCharging = false,
                BatteryPower = 1000,

                GridPower = 500,
                GridPurchasing = true,
            };
        }

        public static SolarRealTime FeedIn(DateTime time)
        {
            return new SolarRealTime()
            {
                Date = time,

                Production = 2000,
                Consumption = 500,

                BatteryCapacity = 50,
                BatteryCharging = true,
                BatteryPower = 1000,

                GridPower = 500,
                GridPurchasing = false,
            };
        }

        public static SolarRealTime BatteryCharging(DateTime time)
        {
            return new SolarRealTime()
            {
                Date = time,

                Production = 2000,
                Consumption = 1000,

                BatteryCapacity = 50,
                BatteryCharging = true,
                BatteryPower = 1000,

                GridPower = 0,
                GridPurchasing = false,
            };
        }

        public static SolarRealTime BatteryDischarging(DateTime time)
        {
            return new SolarRealTime()
            {
                Date = time,

                Production = 1000,
                Consumption = 2000,

                BatteryCapacity = 50,
                BatteryCharging = false,
                BatteryPower = 1000,

                GridPower = 0,
                GridPurchasing = false,
            };
        }

        public static SolarRealTime BatteryFullyCharged(DateTime time)
        {
            return new SolarRealTime()
            {
                Date = time,

                Production = 2000,
                Consumption = 1000,

                BatteryCapacity = 100,
                BatteryCharging = false,
                BatteryPower = 1000,

                GridPower = 1000,
                GridPurchasing = false,
            };
        }

        public SolarRealTime Build(DateTime? time = null)
        {
            if (time.HasValue)
            {
                data.Date = time.Value;
            }

            return data;
        }
    }
}
