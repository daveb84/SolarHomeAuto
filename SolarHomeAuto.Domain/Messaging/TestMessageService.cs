using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Messaging.Helpers;
using System.Collections.Concurrent;

namespace SolarHomeAuto.Domain.Messaging
{
    public class TestMessageService
    {
        private readonly TopicMessageService<string, string> service;
        private readonly IMessageServiceProvider provider;
        private readonly string subscriptionId;

        private static readonly ConcurrentBag<TestMessage> sent = new ConcurrentBag<TestMessage>();
        private static readonly ConcurrentBag<TestMessage> received = new ConcurrentBag<TestMessage>();

        private bool connected = false;
        private readonly SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);

        public TestMessageService(IMessageServiceProvider provider, MessageServiceSettings settings, ILogger<TestMessageService> logger)
        {
            service = new TopicMessageService<string, string>(settings.TestTopicName, provider, logger);
            subscriptionId = $"{nameof(TestMessageService)}{Guid.NewGuid()}";

            this.provider = provider;
        }

        public async Task Init()
        {
            if (!connected)
            {
                await connectLock.WaitAsync();

                try
                {
                    if (!connected)
                    {
                        await ResetConnection(true);
                        connected = true;
                    }
                }
                finally
                {
                    connectLock.Release();
                }
            }
        }

        public async Task Send(string message)
        {
            await Init();

            var audit = new TestMessage(DateTimeNow.UtcNow, message);
            sent.Add(audit);

            await service.Send(message);
        }

        protected Task OnReceive(string message)
        {
            var audit = new TestMessage(DateTimeNow.UtcNow, message);
            received.Add(audit);
            return Task.CompletedTask;
        }

        public IEnumerable<TestMessage> GetSentMessages()
        {
            var list = sent.ToList().OrderBy(x => x.Time);

            return list;
        }

        public IEnumerable<TestMessage> GetReceivedMessages()
        {
            var list = received.ToList().OrderBy(x => x.Time);

            return list;
        }

        public Task ResetConnection()
        {
            return ResetConnection(false);
        }

        private async Task ResetConnection(bool resubscribe)
        {
            await provider.ResetConnection();

            if (resubscribe)
            {
                await service.OnReceive(subscriptionId, OnReceive);
            }
        }

        public class TestMessage
        {
            public TestMessage(DateTime time, string message)
            {
                Time = time;
                Message = message;
            }

            public DateTime Time { get; }

            public string Message { get; }
        }
    }
}
