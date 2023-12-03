namespace SolarHomeAuto.Domain.Devices.History
{
    public interface IDeviceHistoryState<T>
    {
        bool IsStateChange(DeviceHistory<T> previous);
    }
}
