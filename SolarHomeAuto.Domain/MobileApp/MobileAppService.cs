using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;

namespace SolarHomeAuto.Domain.MobileApp
{
    public class MobileAppService : IMobileAppService
    {
        private readonly ILogger<MobileAppService> logger;
        private readonly IMobileAppServicePlatform platform;
        private readonly MobileAppServiceWorker worker;
        private readonly IDataStoreFactory dataStoreFactory;

        public MobileAppService(
            ILogger<MobileAppService> logger,
            IMobileAppServicePlatform platform,
            MobileAppServiceWorker worker,
            IDataStoreFactory dataStoreFactory)
        {
            this.logger = logger;
            this.platform = platform;
            this.worker = worker;
            this.dataStoreFactory = dataStoreFactory;
        }

        public MobileAppServiceStatus Status => worker.Status;

        public async Task AppInit()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var state = await store.GetApplicationState();

                if (state.IsBackgroundServiceRunning)
                {
                    await Start();
                }
            }
        }

        public async Task<MobileAppServiceStatus> Start()
        {
            if (Status != MobileAppServiceStatus.Stopped)
            {
                logger.LogDebug($"Service start received but status is {Status}.  Ignoring.");

                return Status;
            }

            var result = await platform.Start();

            return result;
        }

        public async Task<MobileAppServiceStatus> Stop()
        {
            if (Status != MobileAppServiceStatus.Started)
            {
                logger.LogDebug($"Service stop received but status is {Status}.  Ignoring.");

                return Status;
            }

            var result = await platform.Stop();

            return result;
        }
    }
}
