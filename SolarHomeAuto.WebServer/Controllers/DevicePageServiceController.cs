using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Types;

namespace SolarHomeAuto.WebServer.Controllers
{
    [ApiController]
    [Authorize]
    public class DevicePageServiceController : ControllerBase, IDevicePageService
    {
        private readonly IDevicePageService devicePageService;

        public DevicePageServiceController(IDevicePageService devicePageService)
        {
            this.devicePageService = devicePageService;
        }

        [HttpGet]
        [Route("api/UI/devices")]
        public Task<List<Device>> GetDevices() => devicePageService.GetDevices();

        [HttpGet]
        [Route("api/UI/devices/{deviceId}/status")]
        public Task<SwitchStatus> GetDeviceStatus(string deviceId) => devicePageService.GetDeviceStatus(deviceId);

        [HttpGet]
        [Route("api/UI/devices/{deviceId}/enabled")]
        public async Task<bool> IsDeviceEnabled(string deviceId)
        {
            var res = await devicePageService.IsDeviceEnabled(deviceId);

            return res;
        }

        [HttpPost]
        [Route("api/UI/devices/{deviceId}/enabled")]
        public Task EnableDevice(EnableDeviceRequest data) 
        {
            return devicePageService.EnableDevice(data);
        }

        [HttpGet]
        [Route("api/UI/devices/{deviceId}/history")]
        public Task<List<DeviceHistory<SwitchHistoryState>>> GetDeviceHistory(string deviceId, [FromQuery]DeviceHistoryFilter filter)
        {
            return devicePageService.GetDeviceHistory(deviceId, filter);
        }

        [HttpPost("api/UI/devices/{deviceId}/switch")]
        public Task SwitchDevice(string deviceId, SwitchAction action) => devicePageService.SwitchDevice(deviceId, action);
    }
}