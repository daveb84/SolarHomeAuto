using SolarHomeAuto.Domain.Messaging.Models;

namespace SolarHomeAuto.Domain.Messaging
{
    public class NullMessageServiceProvider : IMessageServiceProvider
    {
        public bool IsConnected => false;

        public void Dispose()
        {
        }

        public Task Publish<T>(string topic, T payload)
        {
            return Task.CompletedTask;
        }

        public Task ResetConnection()
        {
            return Task.CompletedTask;
        }

        public Task ShutDown()
        {
            return Task.CompletedTask;
        }

        public Task Subscribe(MessageSubscription subscription)
        {
            return Task.CompletedTask;
        }

        public Task Unsubscribe(string subscriptionId)
        {
            return Task.CompletedTask;
        }
    }
}
