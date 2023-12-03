namespace SolarHomeAuto.Domain.Devices.Types
{
    public interface IDeviceConnection
    {
        string DeviceId { get; }
        string Name { get; }
        Type StateType { get; }
    }
}
