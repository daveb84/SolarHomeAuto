namespace SolarHomeAuto.Domain.Solar
{
    public class SolarDeserializationFailedException : Exception
    {
        public SolarDeserializationFailedException(string response, Exception ex)
            : base("Failed to deserialize solar response", ex)
        {
            Response = response;
        }

        public string Response
        {
            get => this.Data["Response"] as string;
            set => this.Data["Response"] = value;
        }
    }
}
