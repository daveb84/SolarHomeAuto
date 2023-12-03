using SolarHomeAuto.Domain.Messaging;

namespace SolarHomeAuto.TestMqttClient
{
    public class Menu
    {
        private readonly IMessagingServer server;

        public Menu(IMessagingServer server)
        {
            this.server = server;
        }

        public async Task RunMenu()
        {
            await server.StartService();

            Console.WriteLine("Press any key to exit:");
            Console.ReadKey();

            await server.StopService();
        }
    }
}
