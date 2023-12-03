using Newtonsoft.Json;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Utils;

namespace SolarHomeAuto.Infrastructure.DataStore.Entities
{
    public class DeviceEntity : Device
    {
        public int Id { get; set; }
        public string Provider { get; set; }
        public string ProviderData { get; set; }
        public string SolarJobs { get; set; }

        public static DeviceEntity FromDomain(Device data, DeviceConnectionSettings connection, List<DeviceSchedule> schedules)
        {
            var mapped = new DeviceEntity();
            Update(data, connection, schedules, mapped);

            return mapped;
        }

        public void Update(Device data, DeviceConnectionSettings connection, List<DeviceSchedule> schedules)
        {
            Update(data, connection, schedules, this);
        }

        private static void Update(Device data, DeviceConnectionSettings connection, List<DeviceSchedule> schedules, DeviceEntity target)
        {
            SimpleMapper.MapToSubClass(data, target);

            target.Provider = connection.Provider;
            target.ProviderData = connection.ProviderData?.Any() == true
                ? JsonConvert.SerializeObject(connection.ProviderData)
                : null;

            target.SolarJobs = schedules?.Any() == true
                ? JsonConvert.SerializeObject(schedules)
                : null;
        }

        public Device ToDevice()
        {
            return SimpleMapper.MapToSuperClass<DeviceEntity, Device>(this);
        }

        public DeviceConnectionSettings ToConnection()
        {
            var connection = new DeviceConnectionSettings()
            {
                DeviceId = DeviceId,
                Name = Name,
                Provider = Provider,
                ProviderData = string.IsNullOrWhiteSpace(ProviderData)
                    ? new Dictionary<string, string>()
                    : JsonConvert.DeserializeObject<Dictionary<string, string>>(ProviderData),
            };

            return connection;
        }

        public List<DeviceSchedule> ToSchedules()
        {
            if (string.IsNullOrWhiteSpace(SolarJobs))
            {
                return new List<DeviceSchedule>();
            }

            var schedules = JsonConvert.DeserializeObject<List<DeviceSchedule>>(SolarJobs);

            foreach(var schedule in schedules)
            {
                schedule.DeviceId = DeviceId;
            }

            return schedules;
        }
    }
}
