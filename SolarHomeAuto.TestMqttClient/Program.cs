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
        x.AddJsonFile("appsettings.Development.json", true);
    })
    .ConfigureServices((x, s) =>
    {
        var testSettings = x.Configuration.GetSection("TestMqttClient").Get<TestMqttClientSettings>();
        s.AddSingleton(testSettings);

        var mqttSettings = x.Configuration.GetSection("Mqtt").Get<MqttSettings>();
        s.AddSingleton(mqttSettings);
        s.AddSingleton<MessageServiceSettings>(x => mqttSettings);
        s.AddSingleton<IMessageServiceProvider, MqttMessageClient>();

        s.AddScoped<Menu>();
    })
    .Build();

using (var scope = host.Services.CreateScope())
{
    var menu = scope.ServiceProvider.GetRequiredService<Menu>();
    await menu.RunMenu();
}
