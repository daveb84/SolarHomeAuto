using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Monitoring;

namespace SolarHomeAuto.Domain.MobileApp
{
    public class MobileAppServiceWorker
    {
        private readonly IMessagingServer messageServer;
        private readonly IMessageServiceProvider messageClient;
        private readonly ILogger<MobileAppServiceWorker> logger;
        private readonly MonitoringWorker monitoringWorker;
        private readonly IDataStoreFactory dataStoreFactory;

        private CancellationTokenSource cancellationTokenSource;

        public MobileAppServiceWorker(
            IMessagingServer messageServer,
            IMessageServiceProvider messageClient,
            ILogger<MobileAppServiceWorker> logger,
            MonitoringWorker monitoringWorker,
            IDataStoreFactory dataStoreFactory)
        {
            this.messageServer = messageServer;
            this.messageClient = messageClient;
            this.logger = logger;
            this.monitoringWorker = monitoringWorker;
            this.dataStoreFactory = dataStoreFactory;
        }

        public MobileAppServiceStatus Status { get; private set; }

        public async Task<MobileAppServiceStatus> Start()
        {
            if (Status != MobileAppServiceStatus.Stopped)
            {
                logger.LogDebug($"Service start received but status is {Status}.  Ignoring.");

                return Status;
            }

            logger.LogInformation("Mobile background service starting");

            var failed = false;
            Status = MobileAppServiceStatus.Starting;

            try
            {
                cancellationTokenSource = new CancellationTokenSource();

                await messageServer.StartService();

                Status = MobileAppServiceStatus.Started;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                monitoringWorker.Start(cancellationTokenSource.Token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to start background service");

                failed = true;
            }
            finally
            {
                await SaveApplicationState(!failed);

                Status = MobileAppServiceStatus.Started;
            }

            return Status;
        }

        public async Task<MobileAppServiceStatus> Stop()
        {
            if (Status != MobileAppServiceStatus.Started)
            {
                logger.LogDebug($"Service stop received but status is {Status}.  Ignoring.");

                return Status;
            }

            logger.LogInformation("Mobile background service stopping");

            Status = MobileAppServiceStatus.Stopping;

            try
            {
                await messageServer.StopService();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to stop message server");
            }

            try
            {
                await messageClient.ShutDown();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to stop message client");
            }

            try
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource = null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to stop worker");
            }

            Status = MobileAppServiceStatus.Stopped;

            await SaveApplicationState(false);

            return Status;
        }

        private async Task SaveApplicationState(bool isRunning)
        {
            using (var store = dataStoreFactory.CreateStore())
            using (var trans = store.CreateTransaction())
            {
                await trans.UpdateApplicationState(x => x.IsBackgroundServiceRunning = isRunning);
                await trans.Commit();
            }
        }
    }
}
