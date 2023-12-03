using SolarHomeAuto.Domain.Messaging;

namespace SolarHomeAuto.Infrastructure.Mqtt
{
    public class MqttSettings : MessageServiceSettings
    {
        public int? SelfHostPort { get; set; }

        public string Host { get; set; }
        public int Port { get; set; }
    }
}
