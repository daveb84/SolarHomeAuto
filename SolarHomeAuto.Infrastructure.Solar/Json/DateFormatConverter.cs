using Newtonsoft.Json.Converters;

namespace SolarHomeAuto.Infrastructure.Solar.Json
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
