using Microsoft.Extensions.DependencyInjection;
using SolarHomeAuto.Domain.Account;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Domain.SolarUsage.Models;
using SolarHomeAuto.Tests.AssertHelpers;
using SolarHomeAuto.Tests.Builders;
using SolarHomeAuto.Tests.Fakes;
using SolarHomeAuto.Tests.TestHarness;
using SolarHomeAuto.Domain.Account.Models;
using SolarHomeAuto.Domain.Devices.Schedule;
using SolarHomeAuto.Domain.SolarExcess;

namespace SolarHomeAuto.Tests.SolarExcess
{
    [TestClass]
    public partial class MonitoringTests
    {
        public class TestCase
        {
            public DateTime StartTime { get; set; }
            public DateTime StartDate => StartTime.Date;
            public DateTime StopTime { get; set; }
            public InMemorySolarApi SolarApi { get; } = new InMemorySolarApi();
            public InMemoryServerApi ServerApi { get; } = new InMemoryServerApi();
            public TestDataForTime<SolarRealTime> SolarData => SolarApi.RealTime;
            public DeviceScheduleSettings DeviceScheduleSettings { get; set; }
            public List<AccountDevice> Devices { get; set; } = new List<AccountDevice>();
            public SolarScheduledJobSettings SolarSettings { get; set; }
            public List<SolarRealTimeSchedulePeriod> SolarRealTimeImportSchedule { get; set; } = new List<SolarRealTimeSchedulePeriod>();

            public int ActualDateAllowedMinutesRange { get; set; } = 0;
            public List<DeviceHistory<SwitchHistoryState>> ExpectedDeviceHistory { get; set; } = new List<DeviceHistory<SwitchHistoryState>>();

            public static object[] Create(Action<TestCase> configure = null)
            {
                var test = new TestCase();

                var startTime = new DateTime(2023, 1, 1, 6, 50, 0);
                var stopTime = new DateTime(2023, 1, 1, 19, 30, 0);

                test.StartTime = startTime;
                test.StopTime = stopTime;

                test.SolarData.IgnoreDate = true;
                
                test.DeviceScheduleSettings = new DeviceScheduleSettings
                {
                    IntervalSeconds = 120,
                    InitialWaitSeconds = 30
                };

                test.SolarRealTimeImportSchedule = SolarRealTimeScheduleBuilder.ActiveDayTime();

                test.Devices = new List<AccountDevice>
                {
                    new AccountDevice
                    {
                        Provider = "TestDevice",
                        DeviceId = "Device1",
                        Name = "Test device",
                        Schedules = new List<DeviceSchedule>
                        {
                            new DeviceSchedule
                            {
                                Time = TimeSpan.Zero,
                                Action = DeviceScheduleAction.None
                            }
                        }
                    }
                };

                test.SolarSettings = new SolarScheduledJobSettings
                {
                    RealTimeInterval = 60
                };
                
                configure?.Invoke(test);

                return new object[] { test };
            }
        }

        private async Task RunTest(TestCase testCase)
        {
            // ARRANGE
            using (var harness = new MobileAppTestHarness()
                .WithSettings(s =>
                {
                    s.DeviceScheduledJobs = testCase.DeviceScheduleSettings;
                    s.SolarImportScheduledJob = testCase.SolarSettings;
                })
                .WithDevices(testCase.Devices)
                .WithSolarImportSchedule(testCase.SolarRealTimeImportSchedule)
                .WithSolarApi(testCase.SolarApi)
                .WithServerApi(testCase.ServerApi))
            {
                await harness.Build();

                await harness.RunBackgroundServiceForDateRange(testCase.StartTime, testCase.StopTime);

                // ASSERT

                using (var db = harness.Services.GetService<IDataStoreFactory>().CreateStore())
                {
                    var deviceData = await db.GetDeviceHistory<SwitchHistoryState>(new DeviceHistoryFilter
                    {
                        NewestFirst = false,
                        From = testCase.StartTime,
                        To = testCase.StopTime,
                    });

                    var changesOnly = deviceData.Where(x => x.State.Action != SwitchAction.None).ToList();

                    AssetCollectionHelper.AreSame(testCase.ExpectedDeviceHistory, changesOnly, true, (expected, actual) =>
                    {
                        var dateOk = actual.Date >= expected.Date.AddMinutes(-testCase.ActualDateAllowedMinutesRange)
                            && actual.Date <= expected.Date.AddMinutes(testCase.ActualDateAllowedMinutesRange);

                        return dateOk 
                            && expected.State.Action == actual.State.Action
                            && expected.DeviceId == actual.DeviceId;
                    });
                }
            }
        }

        private static DeviceHistory<SwitchHistoryState> CreateDeviceHistory(DateTime time, SwitchAction action, SwitchStatus status, string deviceId = "Device1")
        {
            return new DeviceHistory<SwitchHistoryState>
            {
                Date = time,
                DeviceId = deviceId,
                State = new SwitchHistoryState
                {
                    Action = action,
                    Status = status
                }
            };
        }
    }
}
