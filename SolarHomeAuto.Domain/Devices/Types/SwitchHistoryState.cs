using SolarHomeAuto.Domain.Devices.History;

namespace SolarHomeAuto.Domain.Devices.Types
{
    public class SwitchHistoryState : IDeviceHistoryState<SwitchHistoryState>
    {
        public SwitchAction Action { get; set; }
        public SwitchStatus Status { get; set; }
        public decimal? Power { get; set; }

        public bool IsStateChange(DeviceHistory<SwitchHistoryState> previous)
        {
            if (previous?.State == null)
            {
                return true;
            }

            if (Action != SwitchAction.None)
            {
                return true;
            }

            if (previous.State.Power == this.Power || previous.State.Status == this.Status)
            {
                return false;
            }

            return true;
        }
    }
}
