namespace SolarHomeAuto.Domain.ScheduledJobs.Schedules
{
    public interface ISchedulePeriod
    {
        TimeSpan Time { get; }
    }
}