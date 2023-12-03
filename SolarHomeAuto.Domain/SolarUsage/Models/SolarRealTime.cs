namespace SolarHomeAuto.Domain.SolarUsage.Models
{
    public enum GridPowerType
    {
        Purchase,
        FeedIn,
    }

    public enum BatteryPowerType
    {
        Charging,
        Discharging,
    }

    public class SolarRealTime
    {
        public DateTime Date { get; set; }
        public decimal? Production { get; set; }
        public decimal? GridPower { get; set; }
        public bool GridPurchasing { get; set; }
        public bool BatteryCharging { get; set; }
        public decimal? BatteryPower { get; set; }
        public decimal? Consumption { get; set; }
        public decimal? BatteryCapacity { get; set; }

        public decimal GetGridPower(GridPowerType type, bool returnOppositeAsZero = false)
        {
            var power = GridPower ?? 0;

            var amount = type switch
            {
                GridPowerType.FeedIn => GridPurchasing ? -power : power,
                GridPowerType.Purchase => GridPurchasing ? power : -power,
                _ => throw new NotImplementedException()
            };

            if (returnOppositeAsZero && amount < 0) return 0;

            return amount;
        }

        public decimal GetBatteryPower(BatteryPowerType type, bool returnOppositeAsZero = false)
        {
            var power = BatteryPower ?? 0;

            var amount = type switch
            {
                BatteryPowerType.Charging => BatteryCharging ? power : -power,
                BatteryPowerType.Discharging => BatteryCharging ? -power : power,
                _ => throw new NotImplementedException()
            };

            if (returnOppositeAsZero && amount < 0) return 0;

            return amount;
        }

        public bool IsSame(SolarRealTime other)
        {
            return this.Production == other.Production
                && this.GridPower == other.GridPower
                && this.GridPurchasing == other.GridPurchasing
                && this.BatteryCharging == other.BatteryCharging
                && this.BatteryPower == other.BatteryPower
                && this.Consumption == other.Consumption
                && this.BatteryCapacity == other.BatteryCapacity;
        }
    }
}
