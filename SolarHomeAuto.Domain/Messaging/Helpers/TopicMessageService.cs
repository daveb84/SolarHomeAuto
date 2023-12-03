using SolarHomeAuto.Domain.Messaging.Models;
using Microsoft.Extensions.Logging;

namespace SolarHomeAuto.Domain.Messaging.Helpers
{
    internal class TopicMessageService<TRequest, TResponse>
    {
        private readonly string topic;
        private readonly IMessageServiceProvider provider;
        private readonly ILogger logger;

        public TopicMessageService(string topic, IMessageServiceProvider provider, ILogger logger)
        {
            this.topic = topic;
            this.provider = provider;
            this.logger = logger;
        }

        public Task Send(TRequest request)
        {
            return provider.Publish(topic, request);
        }

        public Task OnReceive(string subscriptionId, Func<TResponse, Task> action)
        {
            Func<MessageResponse, Task> callback = response =>
            {
                logger.LogResponse(response);

                if (MessageResponse<TResponse>.TryConvert(response, out var converted))
                {
                    return action(converted.Message);
                }

                return Task.CompletedTask;
            };

            var subscription = new MessageSubscription
            {
                Id = subscriptionId,
                Action = callback,
                Topic = topic
            };

            return provider.Subscribe(subscription);
        }
    }
}
