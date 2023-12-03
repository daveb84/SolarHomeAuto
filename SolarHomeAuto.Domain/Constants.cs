namespace SolarHomeAuto.Domain
{
    public static class Constants
    {
        public const string FormatDateTimeLong = "d ddd MMM HH:mm:ss";

        public static string UtcToLocalLong(this DateTime? dateTime)
        {
            if (dateTime == null) return string.Empty;

            return dateTime.Value.ToLocalTime().ToString(FormatDateTimeLong);
        }

        public static string UtcToLocalLong(this DateTime dateTime)
        {
            return dateTime.ToLocalTime().ToString(FormatDateTimeLong);
        }

        public static bool IsOneOf<T>(this T value, params T[] values)
        {
            return (values ?? new T[0]).Contains(value);
        }
    }
}
