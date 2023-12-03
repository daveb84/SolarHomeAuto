namespace SolarHomeAuto.Domain.Environment
{
    public enum EnvironmentType
    {
        WebServer,
        MobileApp
    }

    public class EnvironmentSettings
    {
        public string Name { get; set; }
        public EnvironmentType Type { get; set; }
        public bool IsMobileApp => Type == EnvironmentType.MobileApp;

        public bool IsEnabled(bool runOnWebServer = false, bool runOnMobileApp = false)
        {
            return (runOnWebServer && !IsMobileApp)
                || (runOnMobileApp && IsMobileApp);
        }
    }
}
