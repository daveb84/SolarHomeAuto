namespace SolarHomeAuto.Domain.Devices.Models
{
    public class DeviceConnectionSettings
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Provider { get; set; }
        public Dictionary<string, string> ProviderData { get; set; }
    }
}
