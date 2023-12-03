namespace SolarHomeAuto.Domain.Messaging.Models
{
    public class MessageSubscription
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Topic { get; set; }
        public Func<MessageResponse, Task> Action { get; set; }
    }
}
