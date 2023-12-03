using SolarHomeAuto.Domain.Devices.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.Devices.RemoteCommands
{
    public class DeviceRemoteCommandStatus
    {
        public bool Enabled { get; set; }
        public SwitchStatusData Status { get; set; }
    }
}
