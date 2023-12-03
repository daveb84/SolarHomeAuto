using Microsoft.Extensions.Logging;
using System.Text;

namespace SolarHomeAuto.Domain.Logging
{
    public class LogReporter : IDisposable
    {
        public ILogger Logger { get; }

        private readonly DateTime start;
        private readonly StringBuilder sb;

        public LogReporter(string title, ILogger logger, LogLevel level = LogLevel.Information)
        {
            start = DateTimeNow.UtcNow;
            sb = new StringBuilder(title);
            sb.AppendLine();

            Add("Started", start.UtcToLocalLong());
            this.Logger = logger;
            Level = level;
        }

        public void Add(object key, object value = null) 
        {
            var val = value?.ToString();

            if (string.IsNullOrWhiteSpace(val))
            {
                sb.AppendLine(ConvertToString(key));
            }
            else
            {
                sb.AppendLine($"{ConvertToString(key)}  :  {ConvertToString(val)}");
            }
        }

        public void AddCollection<T>(IEnumerable<T> data, params Func<T, object>[] key)
        {
            if (key?.Any() != true) return;

            foreach (var item in data)
            {
                var firstObj = key.First().Invoke(item);
                var firstStr = ConvertToString(firstObj);

                if (string.IsNullOrWhiteSpace(firstStr)) firstStr = "(no value)";

                firstStr = "  " + firstStr;

                var parts = key.Skip(1)
                    .Select(x => x.Invoke(item))
                    .Select(ConvertToString)
                    .ToArray();

                Add(firstStr, string.Join(" - ", parts));
            }
        }

        public LogLevel Level { get; set; }

        private string ConvertToString(object value)
        {
            if (value == null) return string.Empty;
            var type = value.GetType();

            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return ((DateTime)value).UtcToLocalLong();
            
            return value.ToString();
        }

        public void Dispose()
        {
            var complete = DateTimeNow.UtcNow;

            Add("Complete", complete.UtcToLocalLong());
            Add("Elapsed", (complete - start).ToString("c"));

            var message = sb.ToString();

            Logger.Log(Level, message);
        }
    }
}
