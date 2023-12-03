using SolarHomeAuto.Domain.Devices.History;

namespace SolarHomeAuto.Domain.DataStore
{
    public static class Extensions
    {
        public static Task SaveDeviceHistory<T>(this IDataStoreTransaction trans, DeviceHistory<T> data)
        {
            return trans.SaveDeviceHistory(data.ToDeviceHistory());
        }
    }
}
