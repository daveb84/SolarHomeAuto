using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.RemoteCommands
{
    public class RemoteCommandMessage
    {
        public Guid MessageId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string RelatedId { get; set; }
        public string Data { get; set; }
        public string Source { get; set; }
        public bool Consumed { get; set; }
        public string ConsumedResult { get; set; }
    }
}
