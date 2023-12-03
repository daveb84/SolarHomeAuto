using Newtonsoft.Json;

namespace SolarHomeAuto.Domain.Messaging.Models
{
    public class MessageResponse<T>
    {
        public DateTime Received { get; set; }
        public string Topic { get; set; }
        public T Message { get; set; }

        public static bool TryConvert(MessageResponse message, out MessageResponse<T> converted, Func<T, bool> isValid = null)
        {
            if (typeof(T) == typeof(string))
            {
                converted = message as MessageResponse<T>;
                return true;
            }

            try
            {
                var obj = JsonConvert.DeserializeObject<T>(message.Message);

                if (isValid?.Invoke(obj) == false)
                {
                    converted = default;
                    return false;
                }

                converted = new MessageResponse<T>
                {
                    Topic = message.Topic,
                    Received = message.Received,
                    Message = obj
                };

                return true;
            }
            catch
            {
                converted = default;
                return false;
            }
        }
    }

    public class MessageResponse : MessageResponse<string>
    {
    }
}
