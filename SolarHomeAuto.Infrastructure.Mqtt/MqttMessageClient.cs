using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Domain.Messaging.Models;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using SolarHomeAuto.Domain;

namespace SolarHomeAuto.Infrastructure.Mqtt
{
    public class MqttMessageClient : IMessageServiceProvider
    {
        private readonly SemaphoreSlim connectLock = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim disconnectLock = new SemaphoreSlim(1, 1);

        private readonly ILogger<MqttMessageClient> logger;
        private readonly MqttSettings settings;

        private MqttFactory factory;
        private IMqttClient mqttClient;

        private ConcurrentDictionary<string, MessageSubscription> subscriptions = new ConcurrentDictionary<string, MessageSubscription>();

        public bool IsConnected { get; private set; }

        public MqttMessageClient(ILogger<MqttMessageClient> logger, MqttSettings settings)
        {
            this.logger = logger;
            this.settings = settings;
        }

        public async Task ShutDown()
        {
            if (IsConnected)
            {
                await disconnectLock.WaitAsync();

                try
                {
                    if (IsConnected)
                    {
                        try
                        {
                            logger.LogDebug("MQTT Client Shut down- starting");

                            var mqttClientDisconnectOptions = factory.CreateClientDisconnectOptionsBuilder().Build();
                            await mqttClient.DisconnectAsync(mqttClientDisconnectOptions, CancellationToken.None);

                            logger.LogDebug("MQTT Client Shut down - complete");
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "MQTT Client Shut down - error occurred");
                        }

                        Dispose(false);
                    }
                }
                finally
                {
                    disconnectLock.Release();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool clearSubscriptions)
        {
            mqttClient?.Dispose();
            mqttClient = null;
            factory = null;

            if (clearSubscriptions)
            {
                subscriptions.Clear();
            }
        }

        public async Task ResetConnection()
        {
            await ShutDown();
            await Connect();

            var topics = subscriptions.Values
                .GroupBy(x => x.Topic)
                .Select(x => x.First());

            foreach (var subscription in topics)
            {
                await SubscribeInternal(subscription);
            }
        }

        private async Task Connect()
        {
            logger.LogDebug("MQTT client Connect - starting");

            factory = new MqttFactory(new MqttLogger(logger));

            mqttClient = factory.CreateMqttClient();

            // Use builder classes where possible in this project.
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(settings.Host, settings.Port).Build();

            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var message = e.ApplicationMessage.ConvertPayloadToString();
                logger.LogMessage($"Application message received for topic {e.ApplicationMessage.Topic}", message);

                return ProcessReceived(e.ApplicationMessage.Topic, message);
            };

            // This will throw an exception if the server is not available.
            // The result from this message returns additional data which was sent 
            // from the server. Please refer to the MQTT protocol specification for details.
            var response = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            logger.LogMessage("MQTT client Connect - complete with response", response);

            IsConnected = true;
        }

        public async Task Publish<T>(string topic, T payload)
        {
            await EnsureConnected();

            var messageJson = typeof(T) == typeof(string)
                ? (string)(object)payload
                : JsonConvert.SerializeObject(payload);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(messageJson)
                .Build();

            await mqttClient.PublishAsync(applicationMessage);
        }

        public async Task Subscribe(MessageSubscription action)
        {
            await EnsureConnected();

            var alreadyRegistered = subscriptions.Values.Any(x => x.Topic == action.Topic);

            subscriptions[action.Id] = action;

            if (!alreadyRegistered)
            {
                await SubscribeInternal(action);
            }
        }

        public async Task Unsubscribe(string subscriptionId)
        {
            if (!subscriptions.Remove(subscriptionId, out var action))
            {
                logger.LogWarning($"No subscription found with ID {subscriptionId}");
                return;
            }

            var hasOtherSubscriptions = subscriptions.Values.Any(x => x.Topic == action.Topic && x.Id != subscriptionId);

            if (!hasOtherSubscriptions)
            {
                try
                {
                    var options = factory.CreateUnsubscribeOptionsBuilder()
                        .WithTopicFilter(action.Topic)
                        .Build();

                    await mqttClient.UnsubscribeAsync(options, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error occurred whilst unsubscribing from topic {action.Topic}");
                }
            }
        }

        private async Task SubscribeInternal(MessageSubscription subscription)
        {
            try
            {
                var options = factory.CreateSubscribeOptionsBuilder()
                    .WithTopicFilter(
                        f =>
                        {
                            f.WithTopic(subscription.Topic);
                        })
                    .Build();

                await mqttClient.SubscribeAsync(options, CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred whilst subscribing to {subscription.Topic}");
            }
        }

        private Task ProcessReceived(string topic, string message)
        {
            var matching = subscriptions.Values.Where(x => x.Topic == topic).ToList();

            if (!matching.Any())
            {
                logger.LogWarning($"No subscriptions found for topic {topic}");

                return Task.CompletedTask;
            }

            logger.LogDebug($"Found {matching.Count} subscriptions for topic {topic}");

            var now = DateTimeNow.UtcNow;

            var messageResponse = new MessageResponse
            {
                Topic = topic,
                Message = message,
                Received = now
            };

            var tasks = matching.Select(x => x.Action(messageResponse)).ToList();

            return Task.WhenAll(tasks);
        }

        private async Task EnsureConnected()
        {
            if (IsConnected && !mqttClient.IsConnected)
            {
                logger.LogWarning("MQTT client ensure connected - resetting connection");

                await ResetConnection();
            }

            if (!IsConnected)
            {
                await connectLock.WaitAsync();

                try
                {
                    if (!IsConnected)

                    {
                        logger.LogDebug("MQTT client ensure connected - connecting");
                        await Connect();

                        logger.LogDebug("MQTT client ensure connected - complete successfully");
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "MQTT client ensure connected - error occurred");
                }
                finally
                {
                    connectLock.Release();
                }
            }
        }
    }
}
