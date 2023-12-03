namespace SolarHomeAuto.Domain.Utils
{
    public class DateIntervalSampler<T>
    {
        private readonly IEnumerable<T> data;
        private readonly Func<T, DateTime> key;

        public DateIntervalSampler(IEnumerable<T> data, Func<T, DateTime> key) 
        {
            this.data = data;
            this.key = key;
        }

        public IEnumerable<(T, DateTime)> Sample(TimeSpan interval)
        {
            if (interval == TimeSpan.Zero)
            {
                return data
                    .Select(x => (x, key(x)));
            }

            var results = data
                .Select((x, index) => new
                {
                    Item = x,
                    Date = key(x),
                    DateUpperBound = RoundUp(key(x), interval)
                })
                .GroupBy(x => x.DateUpperBound)
                .Select(x => new
                {
                    DateUpperBound = x.Key,
                    x.OrderBy(y => y.Date).Last().Item
                })
                .Select(x => (x.Item, x.DateUpperBound));

            return results;
        }

        private DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks);
        }
    }
}
