using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Domain.Utils;

namespace SolarHomeAuto.Infrastructure.Solar.Helpers
{
    internal class DeviceHistoryBatchSplitter
    {
        private readonly SolarStatsDuration duration;

        public DeviceHistoryBatchSplitter(SolarStatsDuration duration) 
        {
            this.duration = duration;
        }

        public IEnumerable<(DateTime from, DateTime to)> GetBatches(DateTime from, DateTime to)
        {
            return BatchSplitter<DateTime>.GetBatches(from, to, BatchSize, Increment, true);
        }

        private int BatchSize
        {
            get
            {
                return duration switch
                {
                    SolarStatsDuration.Day => 29,
                    SolarStatsDuration.Month => 11,
                    SolarStatsDuration.Year => 10,
                    _ => throw new NotImplementedException($"{duration} not implemented")
                };
            }
        }

        private DateTime Increment(DateTime value, int amount)
        {
            return duration switch
            {
                SolarStatsDuration.Day => value.AddDays(amount),
                SolarStatsDuration.Month => new DateTime(value.Year, value.Month, 1).AddMonths(amount),
                SolarStatsDuration.Year => new DateTime(value.Year, 1, 1).AddYears(amount),
                _ => throw new NotImplementedException($"{duration} not implemented")
            };
        }
    }
}
