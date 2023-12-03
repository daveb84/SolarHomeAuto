using SolarHomeAuto.Domain.Messaging;
using Microsoft.Extensions.Hosting;

namespace SolarHomeAuto.AppInit.WebServer.BackgroundServices
{
    internal class MqttClientBackgroundService : BackgroundService
    {
        private readonly IMessageServiceProvider provider;

        public MqttClientBackgroundService(IMessageServiceProvider provider)
        {
            this.provider = provider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (provider.IsConnected)
            {
                await provider.ShutDown();
            }

            await base.StopAsync(cancellationToken);
        }
    }
}
