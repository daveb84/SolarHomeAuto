using Microsoft.Extensions.Hosting;
using SolarHomeAuto.Domain.Messaging;

namespace SolarHomeAuto.AppInit.WebServer.BackgroundServices
{
    internal class MqttServerBackgroundService : BackgroundService
    {
        private readonly IMessagingServer server;

        public MqttServerBackgroundService(IMessagingServer server)
        {
            this.server = server;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return server.StartService();
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await server.StopService();
            server.Dispose();

            await base.StopAsync(cancellationToken);
        }
    }
}
