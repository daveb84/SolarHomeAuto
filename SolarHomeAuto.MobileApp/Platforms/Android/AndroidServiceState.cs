using Android.OS;
using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.MobileApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.OS.PowerManager;

namespace SolarHomeAuto.MobileApp.Platforms.Android
{
    public class AndroidServiceState
    {
        public static MainActivity MainActivity { get; set; }

        private static TaskCompletionSource<MobileAppServiceStatus> runningTask;

        public static Task<MobileAppServiceStatus> StartService()
        {
            return RunServiceOperation(true);
        }

        public static Task<MobileAppServiceStatus> StopService()
        {
            return RunServiceOperation(false);
        }

        public static MobileAppServiceStatus StartWorker()
        {
            return RunWorkerOperation(true);
        }

        public static MobileAppServiceStatus StopWorker()
        {
            return RunWorkerOperation(false);
        }

        private static Task<MobileAppServiceStatus> RunServiceOperation(bool start)
        {
            var label = start ? "StartService" : "StopService";

            if (MainActivity == null)
            {
                GetLogger().LogWarning($"{label} operation requested but MainActivity is null");

                return Task.FromResult(MobileAppServiceStatus.Stopped);
            }

            if (runningTask != null)
            {
                GetLogger().LogWarning($"{label} operation requested but task is in progress");

                return runningTask.Task;
            }

            runningTask = new TaskCompletionSource<MobileAppServiceStatus>();

            if (start)
            {
                CreateWakeLock();
                MainActivity.StartService();
            }
            else
            {
                MainActivity.StopService();
            }

            return runningTask.Task;
        }

        private static MobileAppServiceStatus RunWorkerOperation(bool start)
        {
            var service = MauiProgram.Services.GetService<MobileAppServiceWorker>();

            var result = Task.Run(() => start ? service.Start() : service.Stop()).GetAwaiter().GetResult();

            var label = start ? "StartWorker" : "StopWorker";

            if (runningTask == null)
            {
                GetLogger().LogWarning($"{label} operation ran and returned {result} but there is no running task to resolve");
            }
            else if (!runningTask.TrySetResult(result))
            {
                GetLogger().LogWarning($"{label} operation ran and returned {result} but running task failed to resolve.  Current status: {runningTask.Task.Status}");
            }

            runningTask = null;

            if (!start)
            {
                ReleaseWakeLock();
            }
            
            return result;
        }

        private static WakeLock wakeLock;
        private static void CreateWakeLock()
        {
            ReleaseWakeLock();

            try
            {
                var pm = (PowerManager)global::Android.App.Application.Context.GetSystemService(global::Android.Content.Context.PowerService);
                wakeLock = pm.NewWakeLock(WakeLockFlags.Partial, "SolarHomeAutoServiceWakeLock");
                wakeLock.Acquire();
            }
            catch (Exception ex)
            {
                GetLogger().LogError(ex, "Failed to aquire wake lock");
            }
        }

        private static void ReleaseWakeLock()
        {
            try
            {
                if (wakeLock?.IsHeld == true)
                {
                    wakeLock?.Release();
                }
            }
            catch (Exception ex)
            {
                GetLogger().LogError(ex, "Error when releasing wake lock");
            }
        }

        private static ILogger GetLogger() => MauiProgram.Services.GetService<ILogger<AndroidServiceState>>();
    }
}
