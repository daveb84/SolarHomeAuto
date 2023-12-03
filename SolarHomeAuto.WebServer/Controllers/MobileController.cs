using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.PurgeData;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.ServerApi;
using SolarHomeAuto.Domain.ServerApi.Models;
using SolarHomeAuto.Infrastructure.Auth.ApiKeyAuth;

namespace SolarHomeAuto.WebServer.Controllers
{
    [ApiController]
    [ApiKeyAuth]
    public class MobileController : ControllerBase, IServerApiClient
    {
        private readonly IDataStoreFactory dataStoreFactory;
        private readonly StorePurgedDataService storeDataService;
        private readonly AccountResetService accountService;

        public MobileController(IDataStoreFactory dataStoreFactory, StorePurgedDataService storeDataService, AccountResetService accountService)
        {
            this.dataStoreFactory = dataStoreFactory;
            this.storeDataService = storeDataService;
            this.accountService = accountService;
        }
        
        [HttpPost]
        [Route("api/Mobile/Log/Bulk")]
        public async Task WriteLogs(List<LogEntry> logs)
        {
            var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

            using (var store = dataStoreFactory.CreateStore())
            {
                using (var trans = store.CreateTransaction())
                {
                    foreach(var log in logs)
                    {
                        log.IpAddress = ipAddress;
                    }

                    await trans.SaveLogs(logs);
                    await trans.Commit();
                }
            }
        }

        [HttpPost]
        [Route("/api/Mobile/StoreData")]
        public async Task UploadData(UploadDataRequest data)
        {
            await storeDataService.SaveData(data);
        }

        [HttpGet]
        [Route("/api/Mobile/AccountSettings")]
        public async Task<ActionResult> GetAccountSettings()
        {
            var settings = await accountService.GetAllAccountSettings();

            var json = JObject.FromObject(settings);

            return Content(json.ToString(), "application/json");
        }

        Task<AllAccountSettings> IServerApiClient.GetAccountSettings()
        {
            throw new InvalidOperationException();
        }


        [HttpPost]
        [Route("/api/Mobile/RemoteCommands/query")]
        public async Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var results = await store.GetRemoteCommandMessages(filter);

                return results;
            }
        }

        [HttpPost]
        [Route("/api/Mobile/RemoteCommands/Consume")]
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

        [HttpPost]
        [Route("/api/Mobile/RemoteCommands/Publish")]
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
    }
}
