using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Logging.Models;
using SolarHomeAuto.Domain.ServerApi;

namespace SolarHomeAuto.Infrastructure.ServerApi.Logging
{
    public class ServerApiLogger : LogWriterBase
    {
        private readonly IServerApiClient serverApiClient;

        public ServerApiLogger(string loggerName, IServerApiClient serverApiClient)
            : base(loggerName)
        {
            this.serverApiClient = serverApiClient;
        }

        protected override void WriteLog(LogEntry log)
        {
            Task.Run(() => serverApiClient.WriteLogs(new List<LogEntry> { log }));
        }
    }
}
