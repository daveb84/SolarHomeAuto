using SolarHomeAuto.Domain.MobileApp;

namespace SolarHomeAuto.Tests.Fakes
{
    internal class FakeMobileAppServicePlatform : IMobileAppServicePlatform
    {
        public Task<MobileAppServiceStatus> Start()
        {
            return Task.FromResult(MobileAppServiceStatus.Started);
        }

        public Task<MobileAppServiceStatus> Stop()
        {
            return Task.FromResult(MobileAppServiceStatus.Stopped);
        }
    }
}
