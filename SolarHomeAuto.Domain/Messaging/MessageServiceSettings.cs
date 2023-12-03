namespace SolarHomeAuto.Domain.Messaging
{
    public class MessageServiceSettings
    {
        public bool SelfHostEnabled { get; set; }
        public bool ClientEnabled { get; set; }
        public string TestTopicName { get; set; } = "solarhomeauto/test";
    }
}
