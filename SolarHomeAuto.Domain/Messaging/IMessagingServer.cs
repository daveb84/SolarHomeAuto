namespace SolarHomeAuto.Domain.Messaging
{
    public interface IMessagingServer : IDisposable
    {
        Task StartService();

        Task StopService();
    }
}
