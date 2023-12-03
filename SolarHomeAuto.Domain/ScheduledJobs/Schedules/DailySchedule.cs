namespace SolarHomeAuto.Domain.ScheduledJobs.Schedules
{
    public class DailySchedule<T> : ISchedule<T>
        where T : class, ISchedulePeriod
    {
        private readonly List<T> schedule;

        public DailySchedule(IEnumerable<T> schedule)
        {
            if (schedule.Any())
            {
                this.schedule = schedule.OrderBy(x => x.Time).ToList();
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public T GetCurrentPeriod(DateTime now)
        {
            var match = schedule.LastOrDefault(x => x.Time <= now.TimeOfDay);

            return match;
        }

        public T GetNextPeriod(T period)
        {
            var (_, next) = GetNext(period);

            return next;
        }

        public DateTime GetPeriodNextStartTime(T period, DateTime now)
        {
            var time = now.Date.Add(period.Time);

            if (time < now)
            {
                time = time.AddDays(1);
            }

            return time;
        }

        private (int, T) GetNext(T period)
        {
            if (period == schedule.Last())
            {
                return (0, schedule.First());
            }
            else
            {
                var index = schedule.IndexOf(period) + 1;

                return (index, schedule[index]);
            }
        }
    }
}
