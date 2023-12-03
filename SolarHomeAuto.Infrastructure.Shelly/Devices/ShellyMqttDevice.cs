using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SolarHomeAuto.Domain;
using SolarHomeAuto.Domain.Devices;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Domain.Messaging.Helpers;

namespace SolarHomeAuto.Infrastructure.Shelly.Devices
{
    public class ShellyMqttDevice : SwitchDeviceBase
    {
        private readonly ShellyDeviceConfig config;
        private readonly IMessageServiceProvider messageServiceProvider;
        private readonly ILogger<ShellyMqttDevice> logger;

        public ShellyMqttDevice(ShellyDeviceConfig config, IMessageServiceProvider messageServiceProvider, IDeviceService deviceService, ILogger<ShellyMqttDevice> logger) 
            : base(config, deviceService, logger)
        {
            this.config = config;
            this.messageServiceProvider = messageServiceProvider;
            this.logger = logger;
        }

        protected override async Task<SwitchStatusResult> GetStatusInternal()
        {
            using (var logReporter = new LogReporter("Shelly MQTT Get Status", logger, LogLevel.Debug))
            {
                var replySrc = $"ShellyStatus{DeviceId}";
                logReporter.Add("Reply src", replySrc);

                var data = new GetStatusPayload
                {
                    Id = 1,
                    Source = replySrc,
                    Method = "Shelly.GetStatus"
                };

                logReporter.Add("Data", JsonConvert.SerializeObject(data));

                var publishTopic = $"{config.ShellyMqttDeviceId}/rpc";
                logReporter.Add("Publish topic", publishTopic);

                var replyTopic = $"{replySrc}/rpc";
                logReporter.Add("Reploy topic", replyTopic);

                var replyTask = new ReplyTopicTask<GetStatusPayload, JObject>(publishTopic, replyTopic, messageServiceProvider, logger, config.MqttTimeout);

                try
                {
                    var response = await replyTask.SendMessage(data);

                    var status = (response.Message.SelectToken("$.result.switch:0.output")?.Value<bool>() == true)
                        ? SwitchStatus.On
                        : SwitchStatus.Off;

                    var power = response.Message.SelectToken("$.result.switch:0.apower")?.Value<decimal?>();

                    logReporter.Add("Status success", status);

                    return new SwitchStatusResult(status, power);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"Device is offline {config.DeviceId}");

                    logReporter.Add("Status failed", ex.Message);

                    return new SwitchStatusResult(SwitchStatus.Offline);
                }
            }
        }

        protected override async Task<SwitchActionResult> SwitchInternal(SwitchAction action)
        {
            if (!action.IsOneOf(SwitchAction.TurnOff, SwitchAction.TurnOn))
            {
                return SwitchActionResult.NoAction;
            }

            using (var logReporter = new LogReporter($"Shelly MQTT Switch device {action}", logger, LogLevel.Debug))
            {
                var replySrc = $"ShellySwitchOn{DeviceId}";
                logReporter.Add("Reply src", replySrc);

                var data = new SwitchStatusPayload
                {
                    Id = 0,
                    Method = "Switch.Set",
                    Source = replySrc,
                    Parameters = new SwitchStatusPayloadParams
                    {
                        Id = 0,
                        On = action == SwitchAction.TurnOn
                    }
                };
                logReporter.Add("Data", JsonConvert.SerializeObject(data));

                var publishTopic = $"{config.ShellyMqttDeviceId}/rpc";
                logReporter.Add("Publish topic", publishTopic);

                var replyTopic = $"{replySrc}/rpc";
                logReporter.Add("Reploy topic", replyTopic);

                var replyTask = new ReplyTopicTask<SwitchStatusPayload, JObject>(publishTopic, replyTopic, messageServiceProvider, logger, config.MqttTimeout);

                try
                {
                    var response = await replyTask.SendMessage(data);

                    logReporter.Add("Response success", response.Message);

                    return SwitchActionResult.Success;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, $"Device switch failed. {config.DeviceId}");

                    logReporter.Add("Response failed", ex.Message);

                    return SwitchActionResult.Failure;
                }
            }
        }

        private class GetStatusPayload
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("src")]
            public string Source { get; set; }

            [JsonProperty("method")]
            public string Method { get; set; }
        }

        private class SwitchStatusPayload
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("src")]
            public string Source { get; set; }

            [JsonProperty("method")]
            public string Method { get; set; }

            [JsonProperty("params")]
            public SwitchStatusPayloadParams Parameters { get; set; }
        }

        private class SwitchStatusPayloadParams
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("on")]
            public bool On { get; set; }
        }
    }
}
