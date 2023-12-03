using SolarHomeAuto.Domain.Logging;

namespace SolarHomeAuto.Domain.ScheduledJobs
{
    public interface IScheduledJob
    {
        Task<bool> IsEnabled();
        Task Reset();
        Task<DateTime> RunNext(DateTime now, LogReporter reporter, CancellationToken cancellationToken);        
    }
}
