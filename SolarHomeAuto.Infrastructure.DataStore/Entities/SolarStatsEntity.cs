using SolarHomeAuto.Domain.Utils;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.DataStore.Filtering;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class SolarStatsEntity : SolarStats, IDatePageableEntity
    { 
        public int Id { get; set; }

        public static SolarStatsEntity FromDomain(SolarStats data)
        {
            return SimpleMapper.MapToSubClass<SolarStats, SolarStatsEntity>(data);
        }

        public SolarStats ToDomain()
        {
            return SimpleMapper.MapToSuperClass<SolarStatsEntity, SolarStats>(this);
        }
    }
}
