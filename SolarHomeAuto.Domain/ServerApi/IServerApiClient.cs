using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.ServerApi.Models;

namespace SolarHomeAuto.Domain.ServerApi
{
    public interface IServerApiClient
    {
        Task WriteLogs(List<LogEntry> logs);
        Task UploadData(UploadDataRequest data);
        Task<AllAccountSettings> GetAccountSettings();
        Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter);
        Task ConsumeRemoteCommands(ConsumeRemoteCommandsModel messages);
        Task PublishRemoteCommand(RemoteCommandMessage message);
    }
}
