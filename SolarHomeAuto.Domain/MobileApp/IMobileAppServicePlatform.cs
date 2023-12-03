namespace SolarHomeAuto.Domain.MobileApp
{
    public interface IMobileAppServicePlatform
    {
        Task<MobileAppServiceStatus> Start();
        Task<MobileAppServiceStatus> Stop();
    }
}
