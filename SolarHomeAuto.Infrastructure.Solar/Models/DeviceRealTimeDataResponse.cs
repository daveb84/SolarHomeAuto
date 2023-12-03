using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SolarHomeAuto.Infrastructure.Solar.Models
{
    public class DeviceRealTimeDataResponse : DeviceMessageBase
    {
        public int DeviceState { get; set; }

        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CollectionTime { get; set; }

        public List<DeviceRealTimeValue> DataList { get; set; }
    }

    public class DeviceRealTimeValue
    {
        public string Unit { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Key { get; set; }
    }
}
