namespace SolarHomeAuto.Infrastructure.Solar.Models
{
    public class DeviceHistoryResponse : DeviceMessageBase
    {
        public int TimeType { get; set; }

        public List<DeviceHistoryData> ParamDataList { get; set; }
    }

    public class DeviceHistoryData
    {
        public string CollectTime { get; set; }
        public List<DeviceHistoryValue> DataList { get; set; }
    }

    public class DeviceHistoryValue
    {
        public string Unit { get; set; }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Key { get; set; }
    }
}
