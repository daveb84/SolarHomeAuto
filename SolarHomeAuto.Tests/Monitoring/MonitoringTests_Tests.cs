using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Tests.Builders;

namespace SolarHomeAuto.Tests.SolarExcess
{
    public partial class MonitoringTests
    {
        public static IEnumerable<object[]> SimpleTests
        {
            get
            {
                // conditional - turn on when solar excess, turn off when purchasing
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
                        DeviceScheduleBuilder.EveningTurnOff(new TimeSpan(20, 0, 0))
                    };

                    test.StartTime = test.StartDate.Add(new TimeSpan(6, 50, 0));

                    test.SolarData.AddData(startDate, SolarRealTimeBuilder.NoSolar);
                    test.SolarData.AddData(startDate.AddHours(7), SolarRealTimeBuilder.BatteryCharging);
                    test.SolarData.AddData(startDate.AddHours(12), SolarRealTimeBuilder.BatteryFullyCharged);
                    test.SolarData.AddData(startDate.AddHours(18), SolarRealTimeBuilder.Purchasing);

                    test.ActualDateAllowedMinutesRange = 10;
                    test.ExpectedDeviceHistory = new List<DeviceHistory<SwitchHistoryState>>
                    {
                        CreateDeviceHistory(startDate.Add(new TimeSpan(6, 50, 30)), SwitchAction.TurnOff, SwitchStatus.Off),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(12, 0, 0)), SwitchAction.TurnOn, SwitchStatus.On),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(18, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off)
                    };
                });

                // conditional - when battery has enough charge
                yield return TestCase.Create((test) =>
                {
                    var startDate = test.StartDate;

                    var device = test.Devices.First();
                    device.Schedules = new List<DeviceSchedule>
                    {
                        DeviceScheduleBuilder.FirstTurnOff(),
                        DeviceScheduleBuilder.Conditional(
                            new TimeSpan(7, 0, 0),
                            "Duration(300, x => x.BatteryDischarging < 600) && BatteryCapacity > 30",
                            "BatteryDischarging >= 1000 || BatteryCapacity <= 30")
                    };

                    test.StartTime = test.StartDate.Add(new TimeSpan(6, 50, 0));

                    test.SolarData.AddData(startDate, x => new SolarRealTime
                    {
                        // battery outputting 500
                        Date = x,

                        Production = 500,
                        Consumption = 1000,

                        BatteryCapacity = 35,

                        BatteryPower = 500,
                        BatteryCharging = false,

                        GridPower = 0,
                        GridPurchasing = false,
                    });
                    test.SolarData.AddData(startDate.AddHours(12), x => new SolarRealTime
                    {
                        // battery outputting 1000
                        Date = x,

                        Production = 500,
                        Consumption = 1500,

                        BatteryCapacity = 35,

                        BatteryPower = 1000,
                        BatteryCharging = false,

                        GridPower = 0,
                        GridPurchasing = false,
                    });
                    test.SolarData.AddData(startDate.AddHours(15), x => new SolarRealTime
                    {
                        // battery outputting 500
                        Date = x,

                        Production = 500,
                        Consumption = 1000,

                        BatteryCapacity = 35,

                        BatteryPower = 500,
                        BatteryCharging = false,

                        GridPower = 0,
                        GridPurchasing = false,
                    });

                    test.SolarData.AddData(startDate.AddHours(16), x => new SolarRealTime
                    {
                        // battery outputting 500, but low capacity
                        Date = x,

                        Production = 500,
                        Consumption = 1000,

                        BatteryCapacity = 20,

                        BatteryPower = 500,
                        BatteryCharging = false,

                        GridPower = 0,
                        GridPurchasing = false,
                    });

                    test.ActualDateAllowedMinutesRange = 5;
                    test.ExpectedDeviceHistory = new List<DeviceHistory<SwitchHistoryState>>
                    {
                        CreateDeviceHistory(startDate.Add(new TimeSpan(6, 50, 30)), SwitchAction.TurnOff, SwitchStatus.Off),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(7, 0, 0)), SwitchAction.TurnOn, SwitchStatus.On),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(12, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(15, 0, 0)), SwitchAction.TurnOn, SwitchStatus.On),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(16, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off)
                    };
                });

                // always turn on
                yield return TestCase.Create((test) =>
                {
                    var startDate = test.StartDate;

                    var device = test.Devices.First();
                    device.Schedules = new List<DeviceSchedule>
                    {
                        DeviceScheduleBuilder.FirstTurnOff(),
                        DeviceScheduleBuilder.BasicAction(new TimeSpan(9, 0, 0), DeviceScheduleAction.TurnOn),
                        DeviceScheduleBuilder.BasicAction(new TimeSpan(11, 0, 0), DeviceScheduleAction.None),
                        DeviceScheduleBuilder.BasicAction(new TimeSpan(13, 0, 0), DeviceScheduleAction.TurnOff),
                        DeviceScheduleBuilder.BasicAction(new TimeSpan(15, 0, 0), DeviceScheduleAction.None),
                        DeviceScheduleBuilder.BasicAction(new TimeSpan(17, 0, 0), DeviceScheduleAction.TurnOn),
                        DeviceScheduleBuilder.EveningTurnOff(new TimeSpan(19, 0, 0))
                    };

                    test.StartTime = test.StartDate.Add(new TimeSpan(6, 50, 0));

                    test.SolarData.AddData(startDate, SolarRealTimeBuilder.NoSolar);
                    test.SolarData.AddData(startDate.AddHours(7), SolarRealTimeBuilder.BatteryCharging);
                    test.SolarData.AddData(startDate.AddHours(12), SolarRealTimeBuilder.BatteryFullyCharged);
                    test.SolarData.AddData(startDate.AddHours(19), SolarRealTimeBuilder.NoSolar);

                    test.ActualDateAllowedMinutesRange = 10;
                    test.ExpectedDeviceHistory = new List<DeviceHistory<SwitchHistoryState>>
                    {
                        CreateDeviceHistory(startDate.Add(new TimeSpan(6, 50, 30)), SwitchAction.TurnOff, SwitchStatus.Off),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(9, 0, 0)), SwitchAction.TurnOn, SwitchStatus.On),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(13, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(17, 0, 0)), SwitchAction.TurnOn, SwitchStatus.On),
                        CreateDeviceHistory(startDate.Add(new TimeSpan(19, 0, 0)), SwitchAction.TurnOff, SwitchStatus.Off)
                    };
                });
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(SimpleTests))]
        public Task SolarExcess_ShouldTurnOnOffDevices(TestCase test)
        {
            return RunTest(test);
        }
    }
}
