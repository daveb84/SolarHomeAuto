using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Utils;

namespace SolarHomeAuto.Infrastructure.Shelly.Devices
{
    public class ShellyDeviceConfig : DeviceConnectionSettings
    {
        public string ShellyDeviceId { get; set; }
        public string ShellyMqttDeviceId { get; private set; }
        public int CloudRefreshStatusTime { get; set; }
        public string ShellyLanIPAddress { get; set; }
        public int MqttTimeout { get; set; }

        public static ShellyDeviceConfig Convert(DeviceConnectionSettings settings)
        {
            var mapped = SimpleMapper.MapToSubClass<DeviceConnectionSettings, ShellyDeviceConfig>(settings);

            mapped.ShellyDeviceId = GetValue(settings, "ShellyDeviceId");
            mapped.ShellyMqttDeviceId = GetValue(settings, "ShellyMqttDeviceId");
            mapped.CloudRefreshStatusTime = int.Parse(GetValue(settings, "CloudRefreshStatusTime", "0"));
            mapped.MqttTimeout = int.Parse(GetValue(settings, "MqttTimeout", "0"));
            mapped.ShellyLanIPAddress = GetValue(settings, "ShellyLanIPAddress", "");

            return mapped;
        }

        private static string GetValue(DeviceConnectionSettings settings, string key, string fallback = null) 
        { 
            if (!settings.ProviderData.TryGetValue(key, out var value))
            {
                return fallback;
            }
            return value;
        }
    }
}
