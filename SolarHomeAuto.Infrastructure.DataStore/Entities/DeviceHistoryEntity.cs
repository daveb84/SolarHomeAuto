using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Utils;
using SolarHomeAuto.Infrastructure.DataStore.Filtering;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class DeviceHistoryEntity : DeviceHistory, IDatePageableEntity
    {
        public int Id { get; set; }

        public static DeviceHistoryEntity FromDomain(DeviceHistory data)
        {
            return SimpleMapper.MapToSubClass<DeviceHistory, DeviceHistoryEntity>(data);
        }

        public DeviceHistory ToDomain()
        {
            return SimpleMapper.MapToSuperClass<DeviceHistoryEntity, DeviceHistory>(this);
        }
    }
}
