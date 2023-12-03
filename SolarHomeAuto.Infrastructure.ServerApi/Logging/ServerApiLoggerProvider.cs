using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.ServerApi;

namespace SolarHomeAuto.Infrastructure.ServerApi.Logging
{
    public class ServerApiLoggerProvider : ILoggerProvider
    {
        private readonly IServerApiClient serverApiClient;

        public ServerApiLoggerProvider(IServerApiClient serverApiClient)
        {
            this.serverApiClient = serverApiClient;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ServerApiLogger(categoryName, serverApiClient);
        }

        public void Dispose()
        {
        }
    }
}
