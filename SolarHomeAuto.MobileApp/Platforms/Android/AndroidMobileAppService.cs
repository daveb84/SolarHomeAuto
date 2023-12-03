using Android.App;
using Android.Content;
using Android.OS;
using Microsoft.Extensions.Logging;
using AndroidX.Core.App;
using SolarHomeAuto.Domain.Logging;

namespace SolarHomeAuto.MobileApp.Platforms.Android
{
    [Service]
    public class AndroidMobileAppService : Service
    {
        public AndroidMobileAppService()
        {
        }

        private const int NotificationId = 10000;

        private ILogger GetLogger() => MauiProgram.Services.GetService<ILogger<AndroidMobileAppService>>();

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            var logger = GetLogger();

            using (var logReporter = new LogReporter("AndroidMobileAppService OnStartCommand", logger))
            {
                try
                {
                    logReporter.Add("Intent", intent?.Action);

                    var notificationIntent = new Intent(this, typeof(MainActivity));

                    var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.Immutable);

                    var notification = new NotificationCompat.Builder(this, MainApplication.ChannelId)
                       .SetContentTitle("Solar Home Auto")
                       .SetContentText("Solar Home Auto is running")
                       .SetSmallIcon(Resource.Drawable.ic_stat_wb_sunny)
                       .SetContentIntent(pendingIntent)
                       .Build();

                    logReporter.Add("Starting foreground service");

                    StartForeground(NotificationId, notification);

                    logReporter.Add("Running worker");

                    var result = AndroidServiceState.StartWorker();

                    logReporter.Add("Worker result", result);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "AndroidMobileAppService OnStartCommand exception");
                }
            }

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            var logger = GetLogger();

            using (var logReporter = new LogReporter("OnDestroy", logger))
            {
                try
                {
                    logReporter.Add("Cancelling notification");

                    var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                    notificationManager.Cancel(NotificationId);

                    logReporter.Add("Stopping worker");

                    var result = AndroidServiceState.StopWorker();

                    logReporter.Add("Worker result", result);
                }
                catch (Exception ex)
                {
                    logReporter.Add("Exception", ex.Message);

                    logger.LogError(ex, "OnDestroy exception");
                }
            }

            base.OnDestroy();
        }

        public override void OnLowMemory()
        {
            var logger = GetLogger();
            logger.LogInformation("AndroidMobileAppService OnLowMemory called");

            base.OnLowMemory();
        }
    }
}
