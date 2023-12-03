using Microsoft.Extensions.Logging;

namespace SolarHomeAuto.Domain.Logging.Models
{
    public class LogEntry
    {
        public DateTime Date { get; set; }
        public LogLevel Level { get; set; }
        public string IpAddress { get; set; }
        public string Url { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string Logger { get; set; }
    }
}
