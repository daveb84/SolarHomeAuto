namespace SolarHomeAuto.Domain.Solar
{
    public class SolarAuthFailedException : Exception
    {
        public SolarAuthFailedException(string messsage)
            : base($"Solar Auth failed: {messsage}")
        {
        }
    }
}
