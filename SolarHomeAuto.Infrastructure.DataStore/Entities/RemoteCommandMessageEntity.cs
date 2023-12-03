using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.Utils;
using SolarHomeAuto.Infrastructure.DataStore.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class RemoteCommandMessageEntity : RemoteCommandMessage, IDatePageableEntity
    {
        public int Id { get; set; }

        public void Update(RemoteCommandMessage data)
        {
            SimpleMapper.MapToSubClass(data, this);
        }

        public static RemoteCommandMessageEntity FromDomain(RemoteCommandMessage data)
        {
            return SimpleMapper.MapToSubClass<RemoteCommandMessage, RemoteCommandMessageEntity>(data);
        }

        public RemoteCommandMessage ToDomain()
        {
            return SimpleMapper.MapToSuperClass<RemoteCommandMessageEntity, RemoteCommandMessage>(this);
        }
    }
}
