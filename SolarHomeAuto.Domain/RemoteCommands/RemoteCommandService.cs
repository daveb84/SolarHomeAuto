using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.ServerApi;
using SolarHomeAuto.Domain.ServerApi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.RemoteCommands
{
    public class RemoteCommandService
    {
        private readonly EnvironmentSettings environment;
        private readonly IDataStoreFactory dataStoreFactory;
        private readonly IServerApiClient serverApi;

        public RemoteCommandService(EnvironmentSettings environment, IDataStoreFactory dataStoreFactory, IServerApiClient serverApi)
        {
            this.environment = environment;
            this.dataStoreFactory = dataStoreFactory;
            this.serverApi = serverApi;
        }

        public async Task<List<RemoteCommandMessage>> GetMessages(RemoteCommandMessageFilter filter)
        {
            if (environment.IsMobileApp)
            {
                return await serverApi.GetRemoteCommandMessages(filter);
            }
            else
            {
                using (var store = dataStoreFactory.CreateStore())
                {
                    return await store.GetRemoteCommandMessages(filter);
                }
            }
        }

        public Task<List<RemoteCommandMessage>> GetUnconsumedMessages(string type)
        {
            var filter = new RemoteCommandMessageFilter
            {
                Consumed = false,
                Types = new List<string> { type }
            };

            return GetMessages(filter);
        }

        public async Task Publish(RemoteCommandMessage message)
        {
            message.Source = environment.Name;

            if (environment.IsMobileApp)
            {
                await serverApi.PublishRemoteCommand(message);
            }
            else
            {
                using (var store = dataStoreFactory.CreateStore())
                using (var trans = store.CreateTransaction())
                {
                    await trans.SaveRemoteCommandMessage(message);
                    await trans.Commit();
                }
            }
        }

        public async Task ConsumeMessages(ConsumeRemoteCommandsModel messages)
        {
            if (environment.IsMobileApp)
            {
                await serverApi.ConsumeRemoteCommands(messages);
            }
            else
            {
                using (var store = dataStoreFactory.CreateStore())
                using (var trans = store.CreateTransaction())
                {
                    await trans.ConsumeRemoteCommandMessages(messages.MessageIds, messages.Result);
                    await trans.Commit();
                }
            }
        }
    }
}
