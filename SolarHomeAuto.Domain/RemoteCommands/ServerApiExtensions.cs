using SolarHomeAuto.Domain.ServerApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.RemoteCommands
{
    internal static class ServerApiExtensions
    {
        public static Task<List<RemoteCommandMessage>> GetUnconsumedRemoteCommandMessages(this IServerApiClient serverApi, string type)
        {
            var filter = new RemoteCommandMessageFilter
            {
                Consumed = false,
                Types = new List<string> { type }
            };

            return serverApi.GetRemoteCommandMessages(filter);
        }
    }
}
