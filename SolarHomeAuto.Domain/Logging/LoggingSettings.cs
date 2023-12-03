using Microsoft.Extensions.Logging;

namespace SolarHomeAuto.Domain.Logging
{
    public class LoggingSettings
    {
        public LogLevel DefaultLevel { get; set; }
        public Dictionary<string, LogLevel> Filters { get; set; }
    }
}
