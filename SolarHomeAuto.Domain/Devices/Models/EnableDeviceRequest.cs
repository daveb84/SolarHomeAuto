namespace SolarHomeAuto.Domain.Devices.Models
{
    public class EnableDeviceRequest
    {
        public string DeviceId { get; set; }
        public bool Enable { get; set; }
    }
}
