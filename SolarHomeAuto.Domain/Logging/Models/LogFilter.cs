using SolarHomeAuto.Domain.DataStore.Filtering;

namespace SolarHomeAuto.Domain.Logging.Models
{
    public class LogFilter : PagingFilter
    {
        public List<string> Sources { get; set; } = new List<string>();

        public static new LogFilter Default
        {
            get
            {
                return new LogFilter
                {
                    NewestFirst = true,
                    Skip = 0,
                    Take = 20
                };
            }
        }
    }
}
