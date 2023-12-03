using Newtonsoft.Json;
using SolarHomeAuto.Domain.Messaging;
using SolarHomeAuto.Domain.Messaging.Models;
using SolarHomeAuto.Infrastructure.Mqtt;

namespace SolarHomeAuto.TestMqttClient
{
    public class Menu
    {
        private readonly IMessageServiceProvider provider;
        private readonly TestMqttClientSettings settings;
        private readonly MqttSettings mqttSettings;

        private string TestTopic => mqttSettings.TestTopicName;
        private string ShellyId => settings.ShellyMqttId;

        private enum Option
        {
            Unknown,
            SendMessage,
            ShellyGetStatus,
            ShellySwitchOn,
            ShellySwitchOff,
            Quit
        }

        public Menu(IMessageServiceProvider provider, TestMqttClientSettings settings, MqttSettings mqttSettings)
        {
            this.provider = provider;
            this.settings = settings;
            this.mqttSettings = mqttSettings;
        }

        public async Task RunMenu()
        {
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();

            var option = ReadOption();

            await provider.Subscribe(new MessageSubscription { Topic = TestTopic, Action = OnReceive });
            await provider.Subscribe(new MessageSubscription { Topic = "shellyResponse/rpc", Action = OnReceive });

            while (option != Option.Quit)
            {
                switch (option)
                {
                    case Option.SendMessage:
                        await SendMessage();
                        break;

                    case Option.ShellyGetStatus:
                        await ShellyGetStatus();
                        break;

                    case Option.ShellySwitchOn:
                        await ShellySwitchOn(true);
                        break;

                    case Option.ShellySwitchOff:
                        await ShellySwitchOn(false);
                        break;

                    case Option.Quit:
                    default:
                        return;
                }

                option = ReadOption();
            }
        }

        private Option ReadOption() 
        {
            var opt = Option.Unknown;

            while (opt == Option.Unknown)
            {
                Console.WriteLine("Options:");
                Console.WriteLine("1    Send Message");
                Console.WriteLine("2    Shelly - Get Status");
                Console.WriteLine("3    Shelly - Switch on");
                Console.WriteLine("4    Shelly - Switch off");
                Console.WriteLine("q    Quit");

                var option = Console.ReadLine()?.ToLower();

                opt = option switch
                {
                    "1" => Option.SendMessage,
                    "2" => Option.ShellyGetStatus,
                    "3" => Option.ShellySwitchOn,
                    "4" => Option.ShellySwitchOff,
                    "q" => Option.Quit,
                    _ => Option.Unknown,
                };

                if (opt == Option.Unknown)
                {
                    Console.WriteLine("Invalid option. Please enter options from below");
                }
            }

            return opt;
        }

        private async Task SendMessage()
        {
            await provider.Publish(TestTopic, "SolarHomeAuto.TestMqttClient test payload");
        }

        private async Task ShellyGetStatus()
        {
            var data = new
            {
                id = 1,
                src = "shellyResponse",
                method = "Shelly.GetStatus"
            };

            var json = JsonConvert.SerializeObject(data);

            await provider.Publish($"{ShellyId}/rpc", json);
        }

        private async Task ShellySwitchOn(bool on)
        {
            var data = new
            {
                id = 0,
                src = "shellyResponse",
                method = "Switch.Set",
                @params = new
                {
                    id = 0,
                    on = on
                }
            };

            var json = JsonConvert.SerializeObject(data);

            await provider.Publish($"{ShellyId}/rpc", json);
        }

        private Task OnReceive(MessageResponse response)
        {
            MessageResponse<string>.TryConvert(response, out var converted);

            Console.WriteLine($"RESPONSE RECEIVED. Topic: {response.Topic}");
            Console.WriteLine(converted.Message);

            return Task.CompletedTask;
        }
    }
}
