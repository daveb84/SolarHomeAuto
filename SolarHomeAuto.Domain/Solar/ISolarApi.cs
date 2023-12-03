using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.Domain.Solar
{
    public interface ISolarApi
    {
        IAsyncEnumerable<SolarStats> GetSolarStats(SolarStatsDuration duration, DateTime from, DateTime to);

        Task<SolarRealTime> GetSolarRealTime();

        Task<bool> IsEnabled();
    }
}
