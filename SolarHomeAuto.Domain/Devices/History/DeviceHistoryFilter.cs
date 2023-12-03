using SolarHomeAuto.Domain.DataStore.Filtering;

namespace SolarHomeAuto.Domain.Devices.History
{
    public class DeviceHistoryFilter : PagingFilter
    {
        public bool IncludeFailed { get; set; }
        public bool StateChangesOnly { get; set; }

        public static new DeviceHistoryFilter Default
        {
            get
            {
                return new DeviceHistoryFilter()
                {
                    NewestFirst = true,
                    Skip = 0,
                    Take = 20,
                    IncludeFailed = false
                };
            }
        }
    }
}
