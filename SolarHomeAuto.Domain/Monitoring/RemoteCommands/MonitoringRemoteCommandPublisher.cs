using SolarHomeAuto.Domain.RemoteCommands;

namespace SolarHomeAuto.Domain.Monitoring.RemoteCommands
{
    public class MonitoringRemoteCommandPublisher
    {
        private readonly RemoteCommandService remoteCommandService;

        public MonitoringRemoteCommandPublisher(RemoteCommandService remoteCommandService)
        {
            this.remoteCommandService = remoteCommandService;
        }

        public async Task<MonitoringWorkerStatus> GetStatus()
        {
            var toggleMessages = await remoteCommandService.GetMessages(new RemoteCommandMessageFilter
            {
                LatestOnly = true,
                Types = new List<string>()
                {
                    RemoteCommandTypes.WorkerToggleMessageType,
                    RemoteCommandTypes.WorkerStatusMessageType,
                }
            });

            var lastMessage = toggleMessages.FirstOrDefault();

            if (lastMessage?.Type == RemoteCommandTypes.WorkerStatusMessageType)
            {
                var isRunning = bool.Parse(lastMessage.Data);

                return isRunning
                    ? MonitoringWorkerStatus.Started
                    : MonitoringWorkerStatus.Stopped;
            }
            else if (lastMessage?.Type == RemoteCommandTypes.WorkerToggleMessageType)
            {
                if (lastMessage.Consumed)
                {
                    var isRunning = bool.Parse(lastMessage.ConsumedResult);

                    return isRunning
                        ? MonitoringWorkerStatus.Started
                        : MonitoringWorkerStatus.Stopped;
                }
                else
                {
                    var start = bool.Parse(lastMessage.Data);

                    return start
                        ? MonitoringWorkerStatus.Starting
                        : MonitoringWorkerStatus.Stopping;
                }
            }

            return MonitoringWorkerStatus.Unknown;
        }

        public Task ToggleMonitoringWorker(bool start)
        {
            var message = new RemoteCommandMessage
            {
                Date = DateTimeNow.UtcNow,
                MessageId = Guid.NewGuid(),
                Type = RemoteCommandTypes.WorkerToggleMessageType,
                Data = start.ToString()
            };

            return remoteCommandService.Publish(message);
        }
    }
}
