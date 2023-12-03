using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.Utils;
using SolarHomeAuto.Infrastructure.DataStore.Filtering;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class LogEntity : LogEntry, IDatePageableEntity
    {
        public int Id { get; set; }

        public static LogEntity FromDomain(LogEntry data)
        {
            return SimpleMapper.MapToSubClass<LogEntry, LogEntity>(data);
        }

        public LogEntry ToDomain()
        {
            return SimpleMapper.MapToSuperClass<LogEntity, LogEntry>(this);
        }
    }
}
