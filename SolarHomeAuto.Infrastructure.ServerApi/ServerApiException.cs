namespace SolarHomeAuto.Infrastructure.ServerApi
{
    public class ServerApiException : Exception
    {
        public ServerApiException(string message, Exception inner = null)
            :base(message, inner)
        { 
        }
    }
}
