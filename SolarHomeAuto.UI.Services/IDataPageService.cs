using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.RemoteCommands;

namespace SolarHomeAuto.UI.Services
{
    public interface IDataPageService
    {
        public bool IsMobileApp { get; }
        Task<List<LogEntry>> GetLogs(LogFilter filter);
        Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter);
        Task PurgeData();
        Task ResetSettings();
    }
}
