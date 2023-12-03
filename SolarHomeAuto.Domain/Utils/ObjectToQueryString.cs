using System.Web;

namespace SolarHomeAuto.Domain.Utils
{
    public static class ObjectToQueryString
    {
        public static string ToQueryString(this object obj, string prefix = "?")
        {
            var props = obj.GetType().GetProperties();

            var parts = props.Select(x => new
            {
                x.Name,
                Value = x.GetValue(obj)
            })
            .Where(x => x.Value != null)
            .Select(x => new
            {
                x.Name,
                Value = HttpUtility.UrlEncode(ConvertValue(x.Value))
            })
            .Select(x => $"{x.Name}={x.Value}")
            .ToList();

            if (parts.Any())
            {
                return $"{prefix}{(string.Join('&', parts))}";
            }

            return string.Empty;
        }

        private static string ConvertValue(object value)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToString("s");
            }

            return value.ToString();
        }
    }
}
