using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.RemoteCommands;

namespace SolarHomeAuto.WebServer.Controllers
{
    [ApiController]
    [Authorize]
    public class DataPageServiceController : ControllerBase, IDataPageService
    {
        private readonly IDataPageService pageService;

        public DataPageServiceController(IDataPageService pageService)
        {
            this.pageService = pageService;
        }

        public bool IsMobileApp => pageService.IsMobileApp;

        [HttpPost]
        [Route("api/UI/logs/query")]
        public Task<List<LogEntry>> GetLogs(LogFilter filter) => pageService.GetLogs(filter);

        [HttpPost]
        [Route("api/UI/commands/query")]
        public Task<List<RemoteCommandMessage>> GetRemoteCommandMessages(RemoteCommandMessageFilter filter) => pageService.GetRemoteCommandMessages(filter);

        [HttpPost]
        [Route("api/UI/data/purge")]
        public Task PurgeData() => pageService.PurgeData();

        [HttpPost]
        [Route("api/UI/settings/reset")]
        public Task ResetSettings() => pageService.ResetSettings();
    }
}