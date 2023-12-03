using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Logging.Models;

namespace SolarHomeAuto.Domain.Logging
{
    public abstract class LogWriterBase : ILogger
    {
        private readonly string loggerName;

        public LogWriterBase(string loggerName)
        {
            this.loggerName = loggerName;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var logEntry = new LogEntry 
            { 
                Date = DateTimeNow.UtcNow,
                Level = logLevel,
                Message = formatter(state, exception),
                Exception = exception?.ToString(),
                Logger = loggerName,
            };

            WriteLog(logEntry);
        }

        public void FlushLogs()
        {
        }

        protected abstract void WriteLog(LogEntry log);
    }
}
