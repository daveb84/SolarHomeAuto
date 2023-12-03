using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.MobileApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.Monitoring
{
    public class MonitoringService
    {
        private readonly IDataStoreFactory dataStoreFactory;
        private readonly EnvironmentSettings environment;

        public MonitoringService(IDataStoreFactory dataStoreFactory, EnvironmentSettings environment)
        {
            this.dataStoreFactory = dataStoreFactory;
            this.environment = environment;
        }

        public async Task<bool> IsMobileMonitoringHost()
        {
            if (!environment.IsMobileApp) return false;

            return await IsMonitoringHost();
        }

        public async Task<bool> IsMonitoringHost()
        {
            var data = await GetEnvironment();

            return data.Mode.IsOneOf(MonitoringServiceMode.Host);
        }

        public async Task UpdateEnvironment(MonitoringEnvironment data)
        {
            using (var store = dataStoreFactory.CreateStore())
            using (var trans = store.CreateTransaction())
            {
                await trans.UpdateApplicationState(x => 
                {
                    x.MonitoringServiceMode = data.Mode;
                    x.HasLanAccess = data.HasLanAccess;

                    if (data.Mode != MonitoringServiceMode.Host)
                    {
                        x.IsBackgroundServiceRunning = false;
                        x.IsMonitoringWorkerRunning = false;
                    }
                });

                await trans.Commit();
            }
        }

        public async Task<MonitoringEnvironment> GetEnvironment()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var state = await store.GetApplicationState();

                return new MonitoringEnvironment
                {
                    Mode = state.MonitoringServiceMode,
                    HasLanAccess = state.HasLanAccess,
                };
            }
        }

        public async Task ToggleLocalMonitoringWorker(bool run)
        {
            using (var store = dataStoreFactory.CreateStore())
            using (var trans = store.CreateTransaction())
            {
                await trans.UpdateApplicationState(x => x.IsMonitoringWorkerRunning = run);
                await trans.Commit();
            }
        }

        public async Task<bool> IsLocalMonitoringWorkerRunning()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var state = await store.GetApplicationState();

                return state.IsMonitoringWorkerRunning;
            }
        }
    }
}
