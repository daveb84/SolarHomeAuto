using SolarHomeAuto.Domain.MobileApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.MobileApp.Platforms.Android
{
    public class AndroidMobileAppServicePlatform : IMobileAppServicePlatform
    {
        public Task<MobileAppServiceStatus> Start() => AndroidServiceState.StartService();
        public Task<MobileAppServiceStatus> Stop() => AndroidServiceState.StopService();
    }
}
