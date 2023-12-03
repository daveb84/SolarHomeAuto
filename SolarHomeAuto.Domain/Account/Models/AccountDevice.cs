using SolarHomeAuto.Domain.Devices.Models;

namespace SolarHomeAuto.Domain.Account.Models
{
    public class AccountDevice : DeviceConnectionSettings
    {
        public List<DeviceSchedule> Schedules { get; set; }
    }
}
