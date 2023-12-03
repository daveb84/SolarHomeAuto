using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Tests.Builders;

namespace SolarHomeAuto.Tests.SolarExcess
{
    public partial class MonitoringTests
    {
        public static IEnumerable<object[]> RemoteCommandTests
        {
            get
            {
                // remote command to turn on/off
                yield return TestCase.Create((test) =>
                {
                    var startDate = test.StartDate;

                    var device = test.Devices.First();
                    device.Schedules = new List<DeviceSchedule>
                    {
                        DeviceScheduleBuilder.FirstTurnOff(),
                    };

                    test.StartTime = test.StartDate.Add(new TimeSpan(7, 0, 0));

                    test.SolarData.AddData(startDate, SolarRealTimeBuilder.NoSolar);

                    test.ServerApi.SetupRemoteCommandMessage(startDate.Add(new TimeSpan(9, 0, 0)), x => x.DeviceSwitch(device.DeviceId, SwitchAction.TurnOn));
                    test.ServerApi.SetupRemoteCommandMessage(startDate.Add(new TimeSpan(10, 0, 0)), x => x.DeviceSwitch(device.DeviceId, SwitchAction.TurnOff));

                    test.ActualDateAllowedMinutesRange = 5;
                    test.ExpectedDeviceHistory = new List<DeviceHistory<SwitchHistoryState>>
                    {
                        CreateDeviceHistory(startDate.Add(new TimeSpan(7, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(9, 0, 0)), SwitchAction.TurnOn, SwitchStatus.On),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(10, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off),
                    };
                });

                // remote command to disable solar monitoring
                yield return TestCase.Create((test) =>
                {
                    var startDate = test.StartDate;

                    var device = test.Devices.First();
                    device.Schedules = new List<DeviceSchedule>
                    {
                        DeviceScheduleBuilder.FirstTurnOff(),
                        DeviceScheduleBuilder.Conditional(
                            new TimeSpan(7, 0, 0),
                            "Duration(300, x => x.GridFeedIn > 0 && x.Production > 500 && x.DeviceOff)",
                            "GridPurchase > 0"),
                        DeviceScheduleBuilder.EveningTurnOff(new TimeSpan(19, 0, 0))
                    };

                    test.StartTime = test.StartDate.Add(new TimeSpan(6, 50, 0));

                    test.SolarData.AddData(startDate, SolarRealTimeBuilder.NoSolar);
                    test.SolarData.AddData(startDate.AddHours(8), SolarRealTimeBuilder.BatteryFullyCharged);
                    test.SolarData.AddData(startDate.AddHours(9), SolarRealTimeBuilder.Purchasing);
                    test.SolarData.AddData(startDate.AddHours(10), SolarRealTimeBuilder.BatteryFullyCharged);
                    test.SolarData.AddData(startDate.AddHours(11), SolarRealTimeBuilder.Purchasing);
                    test.SolarData.AddData(startDate.AddHours(12), SolarRealTimeBuilder.BatteryFullyCharged);

                    test.ServerApi.SetupRemoteCommandMessage(startDate.Add(new TimeSpan(9, 30, 0)), x => x.WorkerToggle(false));
                    test.ServerApi.SetupRemoteCommandMessage(startDate.Add(new TimeSpan(11, 30, 0)), x => x.WorkerToggle(true));

                    test.ActualDateAllowedMinutesRange = 10;
                    test.ExpectedDeviceHistory = new List<DeviceHistory<SwitchHistoryState>>
                    {
                        CreateDeviceHistory(startDate.Add(new TimeSpan(6, 50, 30)), SwitchAction.TurnOff, SwitchStatus.Off),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(8, 0, 0)), SwitchAction.TurnOn, SwitchStatus.On),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(9, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(12, 0, 0)), SwitchAction.TurnOn, SwitchStatus.On),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(19, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off)
                    };
                });
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(RemoteCommandTests))]
        public Task SolarExcess_WithRemoteCommands_ShouldTurnOnOffDevices(TestCase test)
        {
            return RunTest(test);
        }
    }
}
