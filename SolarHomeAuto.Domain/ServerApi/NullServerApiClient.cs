using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.ServerApi.Models;

namespace SolarHomeAuto.Domain.ServerApi
{
    public class NullServerApiClient : IServerApiClient
    {
        public Task ConsumeRemoteCommands(ConsumeRemoteCommandsModel messages)
        {
            return Task.CompletedTask;
        }

        public Task<AllAccountSettings> GetAccountSettings()
        {
            return Task.FromResult(new AllAccountSettings());
        }

        public Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter)
        {
            return Task.FromResult(new List<RemoteCommandMessage>());
        }

        public Task PublishRemoteCommand(RemoteCommandMessage message)
        {
            return Task.CompletedTask;
        }

        public Task UploadData(UploadDataRequest data)
        {
            return Task.CompletedTask;
        }

        public Task WriteLogs(List<LogEntry> logs)
        {
            return Task.CompletedTask;
        }
    }
}
