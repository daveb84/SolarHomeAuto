using SolarHomeAuto.Domain.Messaging.Models;

namespace SolarHomeAuto.Domain.Messaging
{
    public interface IMessageServiceProvider : IDisposable
    {
        Task Publish<T>(string topic, T payload);
        Task Subscribe(MessageSubscription subscription);
        Task Unsubscribe(string subscriptionId);
        bool IsConnected { get; }
        Task ShutDown();
        Task ResetConnection();
    }
}
