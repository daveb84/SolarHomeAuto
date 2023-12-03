using SolarHomeAuto.Domain.Logging.Models;

namespace SolarHomeAuto.Domain.Logging
{
    public interface ILogViewer
    {
        Task<List<LogEntry>> Get(LogFilter filter);
    }
}
