using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Messaging.Models;

namespace SolarHomeAuto.Domain.Messaging.Helpers
{
    public class ReplyTopicTask<TRequest, TResponse>
    {
        private readonly string publishTopic;
        private readonly string replyTopic;

        private readonly IMessageServiceProvider messageService;
        private readonly int timeout;
        private readonly ILogger logger;

        public ReplyTopicTask(string publishTopic, string replyTopic, IMessageServiceProvider messageService, ILogger logger, int timeout)
        {
            this.publishTopic = publishTopic;
            this.replyTopic = replyTopic;
            this.messageService = messageService;
            this.timeout = timeout > 5 ? timeout : 5;
            this.logger = logger;
        }

        public async Task<MessageResponse<TResponse>> SendMessage(TRequest request)
        {
            var subscriptionId = Guid.NewGuid().ToString();

            var cancelSource = new CancellationTokenSource();

            Task<MessageResponse<TResponse>> getResponse = null;

            Exception exception = null;

            try
            {
                getResponse = Execute(subscriptionId, request, cancelSource.Token);
                var timeout = Timeout(cancelSource);

                await Task.WhenAny(getResponse, timeout);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                await messageService.Unsubscribe(subscriptionId);
            }

            if (getResponse?.IsCompletedSuccessfully != true)
            {
                throw new InvalidOperationException($"Reply topic message unsuccessful", exception);
            }

            return getResponse.Result;
        }

        private async Task Timeout(CancellationTokenSource cancelSource)
        {
            await DateTimeNow.Delay(timeout * 1000);

            cancelSource.Cancel();
        }

        private Task<MessageResponse<TResponse>> Execute(string subscriptionId, TRequest request, CancellationToken cancelToken)
        {
            var task = new TaskCompletionSource<MessageResponse<TResponse>>();

            cancelToken.Register(() =>
            {
                task.SetCanceled(cancelToken);
            });

            var subscription = new MessageSubscription
            {
                Id = subscriptionId,
                Topic = replyTopic,
                Action = response =>
                {
                    if (!MessageResponse<TResponse>.TryConvert(response, out var converted))
                    {
                        task.SetException(new InvalidDataException($"Cannot convert response to type {nameof(TResponse)}. Received {response.Message}"));
                    }
                    else
                    {
                        task.SetResult(converted);
                    }

                    return Task.CompletedTask;
                }
            };

            messageService.Subscribe(subscription);

            messageService.Publish(publishTopic, request);

            return task.Task;
        }
    }
}
