using Android.App;
using Android.Content;
using Android.Content.PM;
using SolarHomeAuto.MobileApp.Platforms.Android;

namespace SolarHomeAuto.MobileApp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        public MainActivity()
        {
            AndroidServiceState.MainActivity = this;
        }

        public void StartService()
        {
            var serviceIntent = new Intent(this, typeof(AndroidMobileAppService));
            StartService(serviceIntent);
        }

        public void StopService()
        {
            var serviceIntent = new Intent(this, typeof(AndroidMobileAppService));
            StopService(serviceIntent);
        }
    }
}