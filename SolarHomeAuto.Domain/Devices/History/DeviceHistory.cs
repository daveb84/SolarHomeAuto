using Newtonsoft.Json;

namespace SolarHomeAuto.Domain.Devices.History
{
    public class DeviceHistory<T>
    {
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }
        public T State { get; set; }
        public string Source { get; set; }
        public string Error { get; set; }
        public bool Success => string.IsNullOrWhiteSpace(Error);
        public bool IsStateChange { get; set; }

        public DeviceHistory ToDeviceHistory()
        {
            var data = new DeviceHistory()
            {
                Date = Date,
                DeviceId = DeviceId,
                Error = Error,
                Source = Source,
                IsStateChange = IsStateChange,
                State = JsonConvert.SerializeObject(State)
            };

            return data;
        }
    }

    public class DeviceHistory
    {
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }
        public string State { get; set; }
        public string Source { get; set; }
        public string Error { get; set; }
        public bool IsStateChange { get; set; }

        public DeviceHistory<T> ToTyped<T>()
        {
            var data = new DeviceHistory<T>()
            {
                Date = Date,
                DeviceId = DeviceId,
                Error = Error,
                Source = Source,
                IsStateChange = IsStateChange,
                State = JsonConvert.DeserializeObject<T>(State)
            };

            return data;
        }
    }
}
