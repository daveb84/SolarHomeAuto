using Microsoft.Extensions.Hosting;
using SolarHomeAuto.Domain.Monitoring;

namespace SolarHomeAuto.AppInit.WebServer.BackgroundServices
{
    public class MonitoringBackgroundService : BackgroundService
    {
        private readonly MonitoringWorker jobRunner;

        public MonitoringBackgroundService(MonitoringWorker jobRunner)
        {
            this.jobRunner = jobRunner;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await jobRunner.Start(stoppingToken);
        }
    }
}
