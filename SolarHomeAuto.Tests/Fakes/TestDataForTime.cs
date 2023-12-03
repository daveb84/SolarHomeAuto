using SolarHomeAuto.Domain;

namespace SolarHomeAuto.Tests.Fakes
{
    public class TestDataForTime<T>
        where T : class
    {
        private List<TestDataItem> data = new List<TestDataItem>();
        public Func<DateTime, T> DefaultData { get; set; }

        public bool IgnoreDate { get; set; } = true;

        public TestDataForTime()
        {
        }

        public void AddData(DateTime from, Func<DateTime, T> data)
        {
            this.data.Add(new TestDataItem { Time = from, Data = data });
        }

        public T GetForTimeNow()
        {
            var now = DateTimeNow.UtcNow;

            var data = (GetTestDataItem(now)?.Data ?? DefaultData)?.Invoke(now);

            return data;
        }

        private TestDataItem GetTestDataItem(DateTime now)
        {
            if (IgnoreDate)
            {
                var dataForTime = data
                    .OrderBy(x => x.Time.TimeOfDay)
                    .LastOrDefault(x => x.Time.TimeOfDay <= now.TimeOfDay);

                return dataForTime;
            }

            var dataForDateTime = data
                .OrderBy(x => x.Time)
                .LastOrDefault(x => x.Time <= now);

            return dataForDateTime;
        }

        class TestDataItem
        {
            public DateTime Time { get; set; }
            public Func<DateTime, T> Data { get; set; }
        }
    }
}
