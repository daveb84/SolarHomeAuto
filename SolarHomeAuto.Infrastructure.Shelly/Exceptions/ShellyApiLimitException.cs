namespace SolarHomeAuto.Infrastructure.Shelly.Exceptions
{
    public class ShellyApiLimitException : Exception
    {
        public ShellyApiLimitException(string message)
            : base(message)
        {
        }
    }
}
