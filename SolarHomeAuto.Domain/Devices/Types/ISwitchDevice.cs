using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SolarHomeAuto.Domain.Devices.Types
{
    public enum SwitchStatus
    {
        Offline,
        On,
        Off
    }

    public enum SwitchAction
    {
        None,
        TurnOn,
        TurnOff
    }

    public enum SwitchActionResult
    {
        NoAction,
        Success,
        Failure,
    }

    public class SwitchStatusData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public SwitchStatus Status { get; set; }
        public decimal? Power { get; set; }
        public static SwitchStatusData Offline => new SwitchStatusData { Status = SwitchStatus.Offline };
    }

    public interface ISwitchDevice : IDeviceConnection
    {
        Task<SwitchStatusData> GetStatus();
        Task<SwitchActionResult> Switch(SwitchAction action, string eventSource);
    }
}