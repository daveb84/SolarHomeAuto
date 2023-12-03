using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.ServerApi.Models
{
    public class ConsumeRemoteCommandsModel
    {
        public List<Guid> MessageIds { get; set; }
        public string Result { get; set; }
    }
}
