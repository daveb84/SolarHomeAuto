namespace SolarHomeAuto.Domain.ScheduledJobs.Schedules
{
    public interface ISchedule<T>
        where T : class, ISchedulePeriod
    {
        T GetCurrentPeriod(DateTime now);
        T GetNextPeriod(T period);
        DateTime GetPeriodNextStartTime(T period, DateTime now);
    }
}
