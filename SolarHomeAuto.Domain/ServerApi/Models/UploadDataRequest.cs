using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.SolarUsage.Models;

namespace SolarHomeAuto.Domain.ServerApi.Models
{
    public class UploadDataRequest
    {
        public List<LogEntry> Logs { get; set; }
        public List<SolarRealTime> SolarRealTime { get; set; }
        public List<DeviceHistory> DeviceHistory { get; set; }
    }
}
