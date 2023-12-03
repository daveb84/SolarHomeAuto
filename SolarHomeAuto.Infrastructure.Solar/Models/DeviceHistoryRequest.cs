namespace SolarHomeAuto.Infrastructure.Solar.Models
{
    public class DeviceHistoryRequest
    {
        public string DeviceSn { get; set; }
        public int TimeType { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
