using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.ServerApi.Models;
using System;

namespace SolarHomeAuto.Domain.Monitoring.RemoteCommands
{
    public class MonitoringRemoteCommandConsumer
    {
        private readonly RemoteCommandService remoteCommandService;
        private readonly MonitoringService monitoringService;
        private readonly RemoteCommandMessageBuilder messageBuilder;

        public MonitoringRemoteCommandConsumer(RemoteCommandService remoteCommandService, MonitoringService monitoringService, RemoteCommandMessageBuilder messageBuilder)
        {
            this.remoteCommandService = remoteCommandService;
            this.monitoringService = monitoringService;
            this.messageBuilder = messageBuilder;
        }

        public async Task<bool> CheckIfJobsEnabled()
        {
            var commands = await remoteCommandService.GetUnconsumedMessages(RemoteCommandTypes.WorkerToggleMessageType);

            if (commands.Any())
            {
                var data = commands
                    .OrderByDescending(x => x.Date)
                    .First();

                var start = bool.Parse(data.Data);

                await monitoringService.ToggleLocalMonitoringWorker(start);

                var consumeModel = new ConsumeRemoteCommandsModel
                {
                    MessageIds = commands.Select(x => x.MessageId).ToList(),
                    Result = start.ToString()
                };

                await remoteCommandService.ConsumeMessages(consumeModel);
            }

            var isRunning = await monitoringService.IsLocalMonitoringWorkerRunning();

            var statusMessage = messageBuilder.WorkerStatus(isRunning);

            await remoteCommandService.Publish(statusMessage);

            return isRunning;
        }
    }
}
