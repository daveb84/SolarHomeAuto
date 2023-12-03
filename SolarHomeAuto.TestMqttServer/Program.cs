using SolarHomeAuto.TestMqttClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Infrastructure.Mqtt;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(x =>
    {
        x.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((x, s) =>
    {
        var settings = x.Configuration.GetSection("Mqtt").Get<MqttSettings>();

        s.AddSingleton(settings);
        s.AddSingleton<MessageServiceSettings>(x => settings);
        s.AddSingleton<IMessagingServer, MqttMessageServer>();
        
        s.AddScoped<Menu>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var server = scope.ServiceProvider.GetRequiredService<Menu>();

    await server.RunMenu();
}
