using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.RemoteCommands
{
    public static class RemoteCommandTypes
    {
        public const string WorkerToggleMessageType = "MonitoringWorkerToggle";
        public const string WorkerStatusMessageType = "MonitoringWorkerStatus";

        public const string DeviceSwitch = "DeviceSwitch";
        public const string DeviceEnable = "DeviceEnable";
        public const string DeviceStatus = "DeviceStatus";
    }
}
