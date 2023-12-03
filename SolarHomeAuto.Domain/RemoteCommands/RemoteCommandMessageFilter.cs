using SolarHomeAuto.Domain.DataStore.Filtering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.RemoteCommands
{
    public class RemoteCommandMessageFilter : PagingFilter
    {
        public bool? Consumed { get; set; }
        public List<string> Types { get; set; }
        public List<string> RelatedIds { get; set; }
        public bool LatestOnly { get; set; }
    }
}
