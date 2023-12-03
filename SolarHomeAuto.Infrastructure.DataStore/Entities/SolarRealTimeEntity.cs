using SolarHomeAuto.Domain.Utils;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Infrastructure.DataStore.Filtering;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class SolarRealTimeEntity : SolarRealTime, IDatePageableEntity
    {
        public int Id { get; set; }

        public static SolarRealTimeEntity FromDomain(SolarRealTime data)
        {
            var entity = SimpleMapper.MapToSubClass<SolarRealTime, SolarRealTimeEntity>(data);

            return entity;
        }

        public SolarRealTime ToDomain()
        {
            return SimpleMapper.MapToSuperClass<SolarRealTimeEntity, SolarRealTime>(this);
        }
    }
}
