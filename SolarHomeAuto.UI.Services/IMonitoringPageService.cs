using SolarHomeAuto.Domain.MobileApp;
using SolarHomeAuto.Domain.Monitoring;

namespace SolarHomeAuto.UI.Services
{
    public interface IMonitoringPageService
    {
        Task<bool> IsMobileMonitoringHost();
        Task AppInit();
        Task<MobileAppServiceStatus> GetMobileAppServiceStatus();
        Task<MobileAppServiceStatus> ToggleMobileAppServiceStatus(bool start);
        Task<MonitoringWorkerStatus> GetWorkerStatus();
        Task<MonitoringWorkerStatus> ToggleWorkerService(bool start);
        Task<MonitoringEnvironment> GetEnvironment();
        Task UpdateEnvironment(MonitoringEnvironment mode);
    }
}
