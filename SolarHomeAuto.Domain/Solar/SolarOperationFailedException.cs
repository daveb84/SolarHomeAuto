namespace SolarHomeAuto.Domain.Solar
{
    public class SolarOperationFailedException : Exception
    {
        public SolarOperationFailedException(string url, Exception ex)
            : base($"Solar Operation {url} failed with exception", ex)
        {
        }

        public SolarOperationFailedException(string url, string messsage, string response = null)
            : base($"Solar Operation {url} failed: {(string.IsNullOrWhiteSpace(messsage) ? response : messsage)}")
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
