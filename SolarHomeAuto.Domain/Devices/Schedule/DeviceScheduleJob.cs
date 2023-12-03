using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.RemoteCommands;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Monitoring;
using SolarHomeAuto.Domain.ScheduledJobs;
using SolarHomeAuto.Domain.SolarExcess;

namespace SolarHomeAuto.Domain.Devices.Schedule
{
    public class DeviceScheduleJob  : IScheduledJob
    {
        private const string SystemDataKey = "DeviceScheduleJobState";

        public DeviceScheduleJob(IDataStoreFactory dataStoreFactory, MonitoringSettings monitoringSettings, DeviceScheduleSettings settings, IDeviceService deviceService, DeviceRemoteCommandConsumer remoteCommandConsumer)
        {
            this.dataStoreFactory = dataStoreFactory;
            this.monitoringSettings = monitoringSettings;
            this.settings = settings;
            this.deviceService = deviceService;
            this.remoteCommandConsumer = remoteCommandConsumer;
        }

        private readonly IDataStoreFactory dataStoreFactory;
        private readonly MonitoringSettings monitoringSettings;
        private readonly DeviceScheduleSettings settings;
        private readonly IDeviceService deviceService;
        private readonly DeviceRemoteCommandConsumer remoteCommandConsumer;

        public Task<bool> IsEnabled()
        {
            return Task.FromResult(true);
        }

        public async Task Reset()
        {
            using (var store = dataStoreFactory.CreateStore())
            using (var trans = store.CreateTransaction())
            {
                await trans.ClearSystemData(SystemDataKey);
                await trans.Commit();
            }
        }

        private bool IsScheduleEnabled()
        {
            return settings.IntervalSeconds > 0;
        }

        public async Task<DateTime> RunNext(DateTime now, LogReporter logReporter, CancellationToken cancellationToken)
        {
            var devicesToSkip = await remoteCommandConsumer.ProcessDevices();

            if (devicesToSkip.Any())
            {
                logReporter.Add("DEVICE REMOTE COMMANDS", "Devices with messages");
                logReporter.AddCollection(devicesToSkip, x => x);
            }

            if (IsScheduleEnabled())
            {
                var schedules = await GetAllSchedules();

                if (schedules.Any())
                {
                    var deviceRuns = await GetDeviceRuns(now, schedules);

                    var result = await RunNext(now, logReporter, deviceRuns, schedules, devicesToSkip, cancellationToken);

                    await SaveState(deviceRuns);
                }   
            }

            return now.AddSeconds(monitoringSettings.RemoteCommandsWaitSeconds);
        }

        private async Task<DateTime> RunNext(DateTime now, LogReporter logReporter, List<DeviceJobRun> deviceRuns, List<DeviceSchedule> schedules, List<string> deviceIdsToSkip, CancellationToken cancellationToken)
        {
            var possible = GetNextRunCandidates(true, now, deviceRuns, schedules, deviceIdsToSkip).ToList();

            var toCheck = possible.FirstOrDefault();

            if (toCheck == null)
            {
                logReporter.Add("NO DEVICES DUE", "Current states");
                logReporter.AddCollection(deviceRuns, x => x.DeviceId, x => x.NextRun);
            }

            while (toCheck != null)
            {
                if (toCheck.NextRunSchedule.Action == DeviceScheduleAction.Conditional)
                {
                    var wait = deviceRuns
                        .Where(x => x.GlobalWaitUntil > now)
                        .Select(x => x.GlobalWaitUntil)
                        .ToList();

                    if (wait.Any())
                    {
                        var waitTime = wait.Max();

                        logReporter.Add("Global wait detected", waitTime);

                        return waitTime;
                    }
                }

                if (cancellationToken.IsCancellationRequested) return DateTime.MaxValue;

                await Run(now, logReporter, toCheck);

                toCheck = GetNextRunCandidates(true, now, deviceRuns, schedules, deviceIdsToSkip).FirstOrDefault();
            }

            var nextTime = deviceRuns
                .OrderBy(x => x.NextRun)
                .Select(x => x.NextRun)
                .FirstOrDefault();

            if (nextTime > now)
            {
                return nextTime;
            }

            return now.AddSeconds(monitoringSettings.RemoteCommandsWaitSeconds);
        }

        private IEnumerable<DeviceJobRun> GetNextRunCandidates(bool prepare, DateTime now, List<DeviceJobRun> deviceRuns, List<DeviceSchedule> schedules, List<string> deviceIdsToSkip)
        {
            var possible = deviceRuns
                .Where(x => x.NextRun <= now && !deviceIdsToSkip.Contains(x.DeviceId))
                .ToList();

            if (prepare)
            {
                foreach (var run in possible)
                {
                    run.PrepareForNextRun(now, schedules);
                }
            }

            return possible
                .OrderBy(x => x.NextRun)
                .ThenBy(x => x.NextRunSchedule.DeviceOrder);
        }

        private async Task Run(DateTime now, LogReporter logReporter, DeviceJobRun run)
        {
            logReporter.Add(string.Empty);
            logReporter.Add("RUNNING DEVICE SCHEDULE", run.DeviceId);
            logReporter.Add("Schedule action", run.NextRunSchedule.Action);

            if (run.NextRunSchedule.Action == DeviceScheduleAction.None)
            {
                ProcessRunResult(now, run, SwitchActionResult.NoAction, logReporter);
                return;
            }

            var device = await deviceService.GetDevice(run.DeviceId);

            if (!device.Enabled)
            {
                logReporter.Add("Device disabled");
                ProcessRunResult(now, run, SwitchActionResult.NoAction, logReporter);

                return;
            }

            var connection = await deviceService.GetDeviceConnection<ISwitchDevice>(run.DeviceId);

            var result = SwitchActionResult.NoAction;

            try
            {
                switch (run.NextRunSchedule.Action)
                {
                    case DeviceScheduleAction.TurnOn:
                        result = await connection.Switch(SwitchAction.TurnOn, "DeviceSchedule");
                        break;

                    case DeviceScheduleAction.TurnOff:
                        result = await connection.Switch(SwitchAction.TurnOff, "DeviceSchedule");
                        break;

                    case DeviceScheduleAction.Conditional:
                        var status = await connection.GetStatus();

                        using (var dataStore = dataStoreFactory.CreateStore())
                        {
                            var evaluator = new SolarExcessEvaluator(run.NextRunSchedule, status, dataStore, logReporter);

                            var evaluateResult = await evaluator.Run();

                            result = await connection.Switch(evaluateResult, "DeviceScheduleSolar");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logReporter.Logger.LogError(ex, "Failed to run device schedule");
            }

            ProcessRunResult(now, run, result, logReporter);
        }

        private void ProcessRunResult(DateTime now, DeviceJobRun run, SwitchActionResult result, LogReporter logReporter)
        {
            run.UpdateRunResult(now, result, settings);

            logReporter.Add("Result", result);
            logReporter.Add("Next run time", run.NextRun);

            if (run.GlobalWaitUntil > DateTime.MinValue)
            {
                logReporter.Add("Global wait set", run.GlobalWaitUntil);
            }
        }

        private async Task<List<DeviceJobRun>> GetDeviceRuns(DateTime now, List<DeviceSchedule> schedules)
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var data = await store.GetSystemData(SystemDataKey);

                var state = data.Value != null
                    ? JsonConvert.DeserializeObject<List<DeviceJobRunState>>(data.Value)
                    : null;

                var firstRun = state == null;

                var deviceRuns = new List<DeviceJobRun>();

                if (state != null)
                {
                    deviceRuns = state
                        .Select(x => new DeviceJobRun(x))
                        .ToList();
                }

                var toAdd = schedules
                    .GroupBy(x => x.DeviceId)
                    .Select(x => x.First())
                    .Where(x => !deviceRuns.Any(d => d.DeviceId == x.DeviceId))
                    .Select(x => new DeviceJobRun
                    {
                        DeviceId = x.DeviceId,
                        NextRun = firstRun
                            ? now.AddSeconds(settings.InitialWaitSeconds)
                            : now,
                    })
                    .ToList();

                deviceRuns.AddRange(toAdd);

                return deviceRuns;
            }
        }

        private async Task SaveState(List<DeviceJobRun> deviceRuns)
        {
            var data = deviceRuns
                .Select(x => x.ToSaveState())
                .ToList();

            using (var store = dataStoreFactory.CreateStore())
            using (var trans = store.CreateTransaction())
            {
                var state = new SystemData
                {
                    Key = SystemDataKey,
                    Value = JsonConvert.SerializeObject(data)
                };

                await trans.SaveSystemData(state);
                await trans.Commit();
            }
        }

        private async Task<List<DeviceSchedule>> GetAllSchedules()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                return await store.GetDeviceSchedules();
            }
        }
    }
}
