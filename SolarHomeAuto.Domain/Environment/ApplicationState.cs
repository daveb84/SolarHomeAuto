using SolarHomeAuto.Domain.Monitoring;

namespace SolarHomeAuto.Domain.Environment
{
    public class ApplicationState
    {
        public bool IsBackgroundServiceRunning { get; set; }
        public bool IsMonitoringWorkerRunning { get; set; }
        public MonitoringServiceMode MonitoringServiceMode { get; set; }
        public string AccountCredentials { get; set; }
        public string ServerApiAccount { get; set; }
        public string SolarRealTimeImportSchedule { get; set; }
        public bool HasLanAccess { get; set; }

        public static ApplicationState Default
        {
            get
            {
                return new ApplicationState();
            }
        }
    }
}
