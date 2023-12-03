namespace SolarHomeAuto.Infrastructure.Shelly.Exceptions
{
    public class ShellyApiException : Exception
    {
        public ShellyApiException(string message, Exception inner = null)
            : base(message, inner)
        {
        }
    }
}
