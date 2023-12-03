namespace SolarHomeAuto.Domain.SolarUsage.Models
{
    public class SolarStats
    {
        public DateTime Date { get; set; }
        public SolarStatsDuration Duration { get; set; }
        public decimal? Generation { get; set; }
        public decimal? Consumption { get; set; }
        public decimal? GridFeedIn { get; set; }
        public decimal? GridPurchase { get; set; }
        public decimal? ChargeCapacity { get; set; }
        public decimal? DischargeCapacity { get; set; }
    }
}
