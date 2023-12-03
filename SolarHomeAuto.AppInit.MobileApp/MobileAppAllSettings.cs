using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices.Schedule;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.Mqtt;
using SolarHomeAuto.Infrastructure.ServerApi;

namespace SolarHomeAuto.AppInit.MobileApp
{
    public class MobileAppAllSettings
    {
        public EnvironmentSettings Environment { get; set; }
        public MqttSettings Mqtt { get; set; }
        public ServerApiSettings ServerApi { get; set; }
        public DataStoreSettings DataStore { get; set; }
        public DeviceScheduleSettings DeviceScheduledJobs { get; set; }
        public SolarScheduledJobSettings SolarImportScheduledJob { get; set; }
        public LoggingSettings Logging { get; set; }
        public MonitoringSettings Monitoring { get; set; }
    }
}
