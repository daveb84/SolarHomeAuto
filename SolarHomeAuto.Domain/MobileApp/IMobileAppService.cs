namespace SolarHomeAuto.Domain.MobileApp
{
    public interface IMobileAppService
    {
        Task AppInit();
        MobileAppServiceStatus Status { get; }
        Task<MobileAppServiceStatus> Start();
        Task<MobileAppServiceStatus> Stop();
    }
}