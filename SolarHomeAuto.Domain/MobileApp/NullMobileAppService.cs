namespace SolarHomeAuto.Domain.MobileApp
{
    public class NullMobileAppService : IMobileAppService
    {
        public MobileAppServiceStatus Status => MobileAppServiceStatus.Stopped;

        public Task AppInit()
        {
            return Task.CompletedTask;
        }

        public Task<MobileAppServiceStatus> Start()
        {
            return Task.FromResult(MobileAppServiceStatus.Stopped);
        }

        public Task<MobileAppServiceStatus> Stop()
        {
            return Task.FromResult(MobileAppServiceStatus.Stopped);
        }
    }
}
