using SolarHomeAuto.Domain.Messaging.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SolarHomeAuto.Domain.Messaging
{
    internal static class MessageLogger
    {
        public static void LogResponse<T>(this ILogger logger, MessageResponse<T> response)
        {
            var message = JsonConvert.SerializeObject(response.Message);

            logger.LogInformation($"Message received for topic {response.Topic}. Payload: {message}");
        }
    }
}
