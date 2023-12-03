using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Internal;
using MQTTnet.Packets;
using MQTTnet.Server;
using SolarHomeAuto.Domain.Messaging;

namespace SolarHomeAuto.Infrastructure.Mqtt
{
    public class MqttMessageServer : IMessagingServer
    {
        private readonly ILogger<MqttMessageServer> logger;
        private readonly MqttSettings settings;
        private readonly ILoggerFactory loggerFactory;
        private MqttServer mqttServer;

        public MqttMessageServer(ILogger<MqttMessageServer> logger, MqttSettings settings, ILoggerFactory loggerFactory)
        {
            this.logger = logger;
            this.settings = settings;
            this.loggerFactory = loggerFactory;
        }

        public void Dispose()
        {
            mqttServer?.Dispose();
        }

        public async Task StartService()
        {
            try
            {
                await StartMqttServer();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to start MQTT server");
            }
        }

        private async Task StartMqttServer()
        {
            logger.LogInformation("Starting Mqtt Message Server");

            var mqttFactory = new MqttFactory(new MqttLogger(loggerFactory.CreateLogger<MqttLogger>()));

            var optsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint();

            if (settings.SelfHostPort > 0)
            {
                optsBuilder = optsBuilder.WithDefaultEndpointPort(settings.SelfHostPort.Value);
            }

            var mqttServerOptions = optsBuilder.Build();

            mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);

            // Attach the event handler.
            mqttServer.ClientAcknowledgedPublishPacketAsync += e =>
            {
                var info = new
                {
                    e.PublishPacket.PacketIdentifier,
                    e.PublishPacket.Topic,
                    QoS1Reason = (e.AcknowledgePacket as MqttPubAckPacket)?.ReasonCode,
                    QoS2Reason = (e.AcknowledgePacket as MqttPubCompPacket)?.ReasonCode
                };

                logger.LogMessage($"Client {e.ClientId} acknowledged packet", info, LogLevel.Trace);

                return CompletedTask.Instance;
            };

            // Setup connection validation before starting the server so that there is 
            // no change to connect without valid credentials.
            mqttServer.ValidatingConnectionAsync += e =>
            {
                logger.LogMessage($"Validating Conection. Client {e.ClientId} Username {e.UserName}", LogLevel.Trace);

                //if (e.ClientId != "ValidClientId")
                //{
                //    e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
                //}

                //if (e.UserName != "ValidUser")
                //{
                //    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                //}

                //if (e.Password != "SecretPassword")
                //{
                //    e.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                //}

                return Task.CompletedTask;
            };

            mqttServer.ClientSubscribedTopicAsync += e =>
            {
                logger.LogMessage($"Client {e.ClientId} subscribed to topic {e.TopicFilter?.Topic}");

                return Task.CompletedTask;
            };

            mqttServer.InterceptingPublishAsync += e =>
            {
                logger.LogMessage($"Intercepting Publish. Client: {e.ClientId} Topic: {e.ApplicationMessage?.Topic}", e.ApplicationMessage, LogLevel.Trace);

                return Task.CompletedTask;
            };

            mqttServer.InterceptingSubscriptionAsync += e =>
            {
                logger.LogMessage($"Intercepting Subscription. Client: {e.ClientId} Topic: {e.TopicFilter?.Topic}");

                return Task.CompletedTask;
            };

            await mqttServer.StartAsync();
        }

        public async Task StopService()
        {
            logger.LogInformation("Stopping Mqtt Message Server");

            if (mqttServer != null)
            {
                await mqttServer.StopAsync();
                mqttServer.Dispose();
                mqttServer = null;
            }
        }
    }
}