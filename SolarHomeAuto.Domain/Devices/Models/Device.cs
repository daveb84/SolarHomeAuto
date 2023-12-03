namespace SolarHomeAuto.Domain.Devices.Models
{
    public class Device
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string StateType { get; set; }
        public bool Enabled { get; set; }
    }
}
