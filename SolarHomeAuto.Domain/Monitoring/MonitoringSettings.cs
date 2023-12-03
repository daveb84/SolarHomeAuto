namespace SolarHomeAuto.Domain.Monitoring
{
    public class MonitoringSettings
    {
        public int MinIntervalSeconds { get; set; }
        public int MaxIntervalSeconds { get; set; }
        public int InitialWaitSeconds { get; set; }
        public int FailedRetrySeconds { get; set; }
        public int RemoteCommandsWaitSeconds { get; set; }
    }
}
