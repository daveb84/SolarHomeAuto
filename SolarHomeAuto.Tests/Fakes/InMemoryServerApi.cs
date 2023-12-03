using Microsoft.EntityFrameworkCore;
using SolarHomeAuto.Domain;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.ServerApi;
using SolarHomeAuto.Domain.ServerApi.Models;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.DataStore.Sqlite;
using SolarHomeAuto.Infrastructure.DataStore.Sqlite.ConnectionFactories;

namespace SolarHomeAuto.Tests.Fakes
{
    public class InMemoryServerApi : IServerApiClient, IDisposable
    {
        private readonly InMemoryConnectionFactory databaseConnection;
        private readonly SqliteDataStoreFactory dataStoreFactory;
        private readonly RemoteCommandMessageBuilder messageBuilder;
        private readonly List<RemoteCommandMessage> messages;
        private readonly Dictionary<Guid, bool> messagesAdded;

        public InMemoryServerApi()
        {
            databaseConnection = new InMemoryConnectionFactory();
            dataStoreFactory = new SqliteDataStoreFactory(new DataStoreSettings(), databaseConnection);
            messageBuilder = new RemoteCommandMessageBuilder(new EnvironmentSettings { Name = "Test" });
            messages = new List<RemoteCommandMessage>();
            messagesAdded = new Dictionary<Guid, bool>();
        }

        public AllAccountSettings AccountSettings { get; set; } = new AllAccountSettings()
        {
            Devices = new List<AccountDevice>(),
            SolarRealTimeImportSchedule = new List<SolarRealTimeSchedulePeriod>()
        };

        public void SetupRemoteCommandMessage(DateTime time, Func<RemoteCommandMessageBuilder, RemoteCommandMessage> messageFactory)
        {
            var message = messageFactory.Invoke(messageBuilder);
            message.Date = time;
            messages.Add(message);
        }

        public async Task ConsumeRemoteCommands(ConsumeRemoteCommandsModel messages)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.ConsumeRemoteCommandMessages(messages.MessageIds, messages.Result);
                    await trans.Commit();
                }
            }
        }

        public void Dispose()
        {
            databaseConnection?.Dispose();
        }

        public Task<AllAccountSettings> GetAccountSettings()
        {
            return Task.FromResult(AccountSettings);
        }

        public async Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var now = DateTimeNow.UtcNow;

                var applicable = messages
                    .Where(x => x.Date <= now && !messagesAdded.ContainsKey(x.MessageId))
                    .ToList();

                if (applicable.Any())
                {
                    using (var trans = store.CreateTransaction())
                    {
                        foreach (var add in applicable)
                        {
                            messagesAdded[add.MessageId] = true;

                            await trans.SaveRemoteCommandMessage(add);
                        }

                        await trans.Commit();
                    }
                }
            }

            using (var store = dataStoreFactory.CreateStore())
            {
                var results = await store.GetRemoteCommandMessages(filter);

                return results;
            }
        }

        public async Task PublishRemoteCommand(RemoteCommandMessage message)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.SaveRemoteCommandMessage(message);
                    await trans.Commit();
                }
            }
        }

        public Task UploadData(UploadDataRequest data)
        {
            return Task.CompletedTask;
        }

        public async Task WriteLogs(List<LogEntry> logs)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    await trans.SaveLogs(logs);
                    await trans.Commit();
                }
            }
        }
    }
}
