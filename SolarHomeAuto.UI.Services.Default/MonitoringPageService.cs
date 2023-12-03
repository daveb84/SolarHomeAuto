using SolarHomeAuto.Domain.MobileApp;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.Domain.Monitoring.RemoteCommands;

namespace SolarHomeAuto.UI.Services.Default
{
    public class MonitoringPageService : IMonitoringPageService
    {
        private readonly IMobileAppService backgroundService;
        private readonly MonitoringRemoteCommandPublisher remoteCommandPublisher;
        private readonly MonitoringService monitoringService;

        public MonitoringPageService(IMobileAppService backgroundService, MonitoringRemoteCommandPublisher remoteCommandPublisher, MonitoringService monitoringService)
        {
            this.backgroundService = backgroundService;
            this.remoteCommandPublisher = remoteCommandPublisher;
            this.monitoringService = monitoringService;
        }

        public Task<bool> IsMobileMonitoringHost() => monitoringService.IsMobileMonitoringHost();

        public async Task AppInit()
        {
            if (await monitoringService.IsMobileMonitoringHost())
            {
                await backgroundService.AppInit();
            }
        }

        public Task<MonitoringEnvironment> GetEnvironment() => monitoringService.GetEnvironment();
        public async Task UpdateEnvironment(MonitoringEnvironment data)
        {
            if (backgroundService.Status != MobileAppServiceStatus.Stopped && data.Mode != MonitoringServiceMode.Host)
            {
                await backgroundService.Stop();
            }

            await monitoringService.UpdateEnvironment(data);
        }

        public Task<MobileAppServiceStatus> GetMobileAppServiceStatus()
        {
            return Task.FromResult(backgroundService.Status);
        }

        public async Task<MobileAppServiceStatus> ToggleMobileAppServiceStatus(bool start)
        {
            if (start)
            {
                return await backgroundService.Start();
            }
            else
            {
                return await backgroundService.Stop();
            }
        }

        public async Task<MonitoringWorkerStatus> GetWorkerStatus()
        {
            if (await monitoringService.IsMonitoringHost())
            {
                var isRunning = await monitoringService.IsLocalMonitoringWorkerRunning();

                return isRunning
                    ? MonitoringWorkerStatus.Started
                    : MonitoringWorkerStatus.Stopped;
            }

            return await remoteCommandPublisher.GetStatus();
        }

        public async Task<MonitoringWorkerStatus> ToggleWorkerService(bool start)
        {
            if (await monitoringService.IsMonitoringHost())
            {
                await monitoringService.ToggleLocalMonitoringWorker(start);
            }
            else
            {
                await remoteCommandPublisher.ToggleMonitoringWorker(start);
            }

            return await GetWorkerStatus();
        }
    }
}
