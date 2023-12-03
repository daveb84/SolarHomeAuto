namespace SolarHomeAuto.Domain.Messaging
{
    public class NullMessagingServer : IMessagingServer
    {
        public void Dispose()
        {
        }

        public Task StartService()
        {
            return Task.CompletedTask;
        }

        public Task StopService()
        {
            return Task.CompletedTask;
        }
    }
}
