using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.Domain.MobileApp;
using SolarHomeAuto.Domain.Monitoring;

namespace SolarHomeAuto.WebServer.Controllers
{
    [ApiController]
    [Authorize]
    public class MonitoringPageServiceController : ControllerBase, IMonitoringPageService
    {
        private readonly IMonitoringPageService pageService;

        public MonitoringPageServiceController(IMonitoringPageService pageService)
        {
            this.pageService = pageService;
        }

        [HttpPost]
        [Route("api/UI/monitoring/init")]
        public Task AppInit() 
        { 
            return pageService.AppInit();
        }

        [HttpPost]
        [Route("api/UI/monitoring/environment")]
        public Task UpdateEnvironment([FromBody] MonitoringEnvironment data) => pageService.UpdateEnvironment(data);

        [HttpGet]
        [Route("api/UI/monitoring/environment")]
        public Task<MonitoringEnvironment> GetEnvironment() => pageService.GetEnvironment();

        [HttpGet]
        [Route("api/UI/monitoring/mobileservice/enabled")]
        public Task<bool> IsMobileMonitoringHost() => pageService.IsMobileMonitoringHost();
        
        [HttpGet]
        [Route("api/UI/monitoring/mobileservice/status")]
        public Task<MobileAppServiceStatus> GetMobileAppServiceStatus() => pageService.GetMobileAppServiceStatus();

        [HttpGet]
        [Route("api/UI/monitoring/worker/status")]
        public Task<MonitoringWorkerStatus> GetWorkerStatus() => pageService.GetWorkerStatus();

        [HttpPost]
        [Route("api/UI/monitoring/mobileservice/toggle")]
        public Task<MobileAppServiceStatus> ToggleMobileAppServiceStatus([FromBody]bool start) => pageService.ToggleMobileAppServiceStatus(start);

        [HttpPost]
        [Route("api/UI/monitoring/worker/toggle")]
        public Task<MonitoringWorkerStatus> ToggleWorkerService([FromBody]bool start) => pageService.ToggleWorkerService(start);
    }
}