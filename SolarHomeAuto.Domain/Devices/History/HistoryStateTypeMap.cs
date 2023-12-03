namespace SolarHomeAuto.Domain.Devices.History
{
    public static class HistoryStateTypeMap
    {
        public static string GetStateTypeName<T>()
        {
            return GetStateTypeName(typeof(T));
        }

        public static string GetStateTypeName(Type type)
        {
            return type.Name;
        }
    }
}
