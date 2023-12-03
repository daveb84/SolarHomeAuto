using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class SystemDataEntity : SystemData
    {
        public int Id { get; set; }

        public void Update(SystemData data)
        {
            SimpleMapper.MapToSubClass(data, this);
        }

        public static SystemDataEntity FromDomain(SystemData data)
        {
            return SimpleMapper.MapToSubClass<SystemData, SystemDataEntity>(data);
        }

        public SystemData ToDomain()
        {
            return SimpleMapper.MapToSuperClass<SystemDataEntity, SystemData>(this);
        }
    }
}
