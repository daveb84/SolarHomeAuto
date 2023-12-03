using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.Tests.Fakes
{
    public class InMemorySolarApi : ISolarApi
    {
        public TestDataForTime<SolarRealTime> RealTime { get; set; } = new TestDataForTime<SolarRealTime>();

        public Task<SolarRealTime> GetSolarRealTime()
        {
            return Task.FromResult(RealTime.GetForTimeNow());
        }

        public IAsyncEnumerable<SolarStats> GetSolarStats(SolarStatsDuration duration, DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEnabled()
        {
            return Task.FromResult(true);
        }
    }
}
