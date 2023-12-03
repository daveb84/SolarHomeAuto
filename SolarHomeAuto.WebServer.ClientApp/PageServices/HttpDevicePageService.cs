using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Utils;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.Domain.Devices.Models;

namespace SolarHomeAuto.WebServer.ClientApp.PageServices
{
    public class HttpDevicePageService : IDevicePageService
    {
        private readonly PageServiceHttpClient httpClient;

        public HttpDevicePageService(PageServiceHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<List<Device>> GetDevices()
        {
            return SendRequest<List<Device>>(HttpMethod.Get, "/api/UI/Devices");
        }

        public Task<SwitchStatus> GetDeviceStatus(string deviceId)
        {
            return SendRequest<SwitchStatus>(HttpMethod.Get, $"/api/UI/Devices/{deviceId}/status");
        }

        public async Task<bool> IsDeviceEnabled(string deviceId)
        {
            var res = await SendRequest<bool>(HttpMethod.Get, $"/api/UI/Devices/{deviceId}/enabled");

            return res;
        }

        public async Task EnableDevice(EnableDeviceRequest enable)
        {
            await SendRequest<string>(HttpMethod.Post, $"/api/UI/Devices/{enable.DeviceId}/enabled", enable);
        }

        public Task SwitchDevice(string deviceId, SwitchAction action)
        {
            return SendRequest<string>(HttpMethod.Post, $"/api/UI/devices/{deviceId}/switch?action={action}");
        }

        public Task<List<DeviceHistory<SwitchHistoryState>>> GetDeviceHistory(string deviceId, DeviceHistoryFilter filter)
        {
            var qs = filter.ToQueryString();

            return SendRequest<List<DeviceHistory<SwitchHistoryState>>>(HttpMethod.Get, $"/api/UI/Devices/{deviceId}/history{qs}");
        }

        private Task<T> SendRequest<T>(HttpMethod method, string url, object body = null) => httpClient.SendRequest<T>(method, url, body);
    }
}
