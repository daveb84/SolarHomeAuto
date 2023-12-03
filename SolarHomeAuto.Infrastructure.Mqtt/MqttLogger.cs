using Microsoft.Extensions.Logging;
using MQTTnet.Diagnostics;
using Newtonsoft.Json;

namespace SolarHomeAuto.Infrastructure.Mqtt
{
    internal class MqttLogger : IMqttNetLogger
    {
        private readonly ILogger logger;

        public MqttLogger(ILogger logger) 
        {
            this.logger = logger;
        }

        public bool IsEnabled => true;

        public void Publish(MqttNetLogLevel logLevel, string source, string message, object[] parameters, Exception exception)
        {
            switch (logLevel)
            {
                case MqttNetLogLevel.Verbose:
                    logger.LogDebug(exception, message, parameters);
                    break;

                case MqttNetLogLevel.Info:
                    logger.LogInformation(exception, message, parameters);
                    break;

                case MqttNetLogLevel.Warning:
                    logger.LogWarning(exception, message, parameters);
                    break;

                case MqttNetLogLevel.Error:
                    logger.LogError(exception, message, parameters);
                    break;
            }
        }
    }

    internal static class MqttLoggerExtensions
    {
        public static void LogMessage(this ILogger logger, string message, object payload = null, LogLevel level = LogLevel.Debug)
        {
            var output = "NULL";
            if (payload != null)
            {
                try
                {
                    output = JsonConvert.SerializeObject(payload);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Error serializing MQTT client message");
                    output = "(cannot be deserialized)";
                }
            }

            logger.Log(level, $"{message} [{payload?.GetType().Name}]: {output}");
        }
    }
}