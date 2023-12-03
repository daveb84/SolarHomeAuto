using Microsoft.Extensions.DependencyInjection;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;
using System.Collections.Concurrent;

namespace SolarHomeAuto.Domain.Devices
{
    public class DeviceConnectionFactory
    {
        private static readonly ConcurrentDictionary<string, Type> providers = new ConcurrentDictionary<string, Type>();

        private readonly IServiceProvider serviceProvider;

        public DeviceConnectionFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public static void AddProvider(string name, Type provider)
        {
            providers[name] = provider;
        }

        public async Task<IDeviceConnection> Connect(DeviceConnectionSettings config)
        {
            if (!providers.TryGetValue(config.Provider, out var providerType))
            {
                throw new InvalidOperationException($"No provider registered for {config.Provider}");
            }

            var provider = (IDeviceProvider)serviceProvider.GetRequiredService(providerType);

            var device = await provider.Connect(config);

            return device;
        }

        public async Task<T> Connect<T>(DeviceConnectionSettings settings)
            where T : class, IDeviceConnection
        {
            var device = await Connect(settings);

            var typed = device as T;

            if (typed == null)
            {
                throw new DeviceRegistrationException(settings.DeviceId, device.GetType(), typeof(T));
            }

            return typed;
        }
    }
}
