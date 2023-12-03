using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.ServerApi.Models;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.UI.Services;

namespace SolarHomeAuto.WebServer.Controllers
{
    [ApiController]
    [Authorize]
    public class AccountPageServiceController : ControllerBase, IAccountPageService
    {
        private readonly IAccountPageService pageService;

        public AccountPageServiceController(IAccountPageService pageService)
        {
            this.pageService = pageService;
        }

        [HttpGet]
        [Route("api/UI/account/devices")]
        public Task<List<AccountDevice>> GetDevices() => pageService.GetDevices();

        [HttpGet]
        [Route("api/UI/account/devices/{deviceId}")]
        public Task<AccountDevice> GetDevice(string deviceId) => pageService.GetDevice(deviceId);

        [HttpGet]
        [Route("api/UI/account/server")]
        public Task<ServerApiAccount> GetServerApiAccount() => pageService.GetServerApiAccount();

        [HttpGet]
        [Route("api/UI/account/credentials")]
        public Task<string> GetServiceCredentials() => pageService.GetServiceCredentials();

        [HttpGet]
        [Route("api/UI/account/solarschedule")]
        public Task<List<SolarRealTimeSchedulePeriod>> GetSolarRealTimeImportSchedule() => pageService.GetSolarRealTimeImportSchedule();

        [HttpPost]
        [Route("api/UI/account/devices")]
        public Task<bool> SaveDevices(List<AccountDevice> model) => pageService.SaveDevices(model);

        [HttpPost]
        [Route("api/UI/account/devices/{deviceId}")]
        public Task<bool> SaveDevice(string deviceId, AccountDevice model) => pageService.SaveDevice(deviceId, model);

        [HttpPost]
        [Route("api/UI/account/server")]
        public Task<bool> SaveServerApiAccount(ServerApiAccount model) => pageService.SaveServerApiAccount(model);

        [HttpPost]
        [Route("api/UI/account/credentials")]
        public Task<bool> SaveServiceCredentials([FromBody]string model) => pageService.SaveServiceCredentials(model);

        [HttpPost]
        [Route("api/UI/account/solarschedule")]
        public Task<bool> SaveSolarRealTimeImportSchedule(List<SolarRealTimeSchedulePeriod> model) => pageService.SaveSolarRealTimeImportSchedule(model);
    }
}