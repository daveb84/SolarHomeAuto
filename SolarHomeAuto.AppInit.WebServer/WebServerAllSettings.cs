using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.Auth.Models;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices.Schedule;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.Mqtt;

namespace SolarHomeAuto.AppInit.WebServer
{
    public class WebServerAllSettings
    {
        public EnvironmentSettings Environment { get; set; }
        public AuthSettings Auth { get; set; }
        public MqttSettings Mqtt { get; set; }
        public DataStoreSettings DataStore { get; set; }
        public DeviceScheduleSettings DeviceScheduledJobs { get; set; }
        public SolarScheduledJobSettings SolarImportScheduledJob { get; set; }
        public LoggingSettings Logging { get; set; }
        public MonitoringSettings Monitoring { get; set; }
    }
}
