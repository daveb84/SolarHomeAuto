using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.Domain.DataStore.Filtering;
using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.WebServer.Controllers
{
    [ApiController]
    [Authorize]
    public class SolarPageServiceController : ControllerBase, ISolarPageService
    {
        private readonly ISolarPageService solarPageService;

        public SolarPageServiceController(ISolarPageService solarPageService)
        {
            this.solarPageService = solarPageService;
        }

        [HttpGet]
        [Route("api/UI/solar/realtime")]
        public Task<List<SolarRealTime>> GetSolarRealTime([FromQuery]PagingFilter filter) => solarPageService.GetSolarRealTime(filter);
    }
}