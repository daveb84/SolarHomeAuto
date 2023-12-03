using DynamicExpresso;
using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.DataStore.Filtering;
using SolarHomeAuto.Domain.Devices.History;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.SolarExcess
{
    public class SolarExcessEvaluator
    {
        private readonly DeviceSchedule deviceSchedule;
        private readonly SwitchStatusData status;
        private readonly IDataStore dataStore;
        private readonly LogReporter logReport;

        public SolarExcessEvaluator(DeviceSchedule deviceSchedule, SwitchStatusData status, IDataStore dataStore, LogReporter logReport)
        {
            this.deviceSchedule = deviceSchedule;
            this.status = status;
            this.dataStore = dataStore;
            this.logReport = logReport;
        }

        public async Task<SwitchAction> Run()
        {
            var result = false;
            var action = SwitchAction.None;

            logReport.Add(string.Empty);
            logReport.Add("EVALUATE DEVICE CONDITION", $"{deviceSchedule.DeviceId}");
            logReport.Add("Device current status", $"{status.Status}");

            switch (status.Status)
            {
                case SwitchStatus.On:
                    action = SwitchAction.TurnOff;
                    result = await RunCondition(deviceSchedule.TurnOffCondition);
                    break;

                case SwitchStatus.Off:
                    action = SwitchAction.TurnOn;
                    result = await RunCondition(deviceSchedule.TurnOnCondition);
                    break;
            }

            var actionResult = result ? action : SwitchAction.None;

            logReport.Add("SWITCH ACTION", $"{actionResult}");
            logReport.Add(string.Empty);

            return actionResult;
        }

        private async Task<bool> RunCondition(string condition)
        {
            try
            {
                logReport.Add("Condition", $"{condition}");

                var historyFrom = DateTimeNow.UtcNow.AddSeconds(-deviceSchedule.ConditionRequiredDeviceHistorySeconds);

                logReport.Add("History required from", historyFrom);

                var solarHistory = await dataStore.GetSolarRealTime(new PagingFilter
                {
                    From = historyFrom,
                    IncludeEarlierThanFrom = 2,
                    NewestFirst = false
                });

                if (solarHistory.Any())
                {
                    var earliest = solarHistory.First().Date;

                    if (earliest < historyFrom)
                    {
                        logReport.Add("Solar history OK. From:", earliest);
                    }
                    else
                    {
                        logReport.Add("Solar history insufficient. From:", earliest);
                    }
                }
                else
                {
                    logReport.Add("Solar history insufficient", "(none)");
                }

                var deviceHistoryFilter = new DeviceHistoryFilter
                {
                    From = historyFrom,
                    NewestFirst = false,
                    IncludeEarlierThanFrom = 2,
                };

                var deviceHistory = await dataStore.GetDeviceHistory<SwitchHistoryState>(deviceHistoryFilter, deviceSchedule.DeviceId);

                if (deviceHistory.Any())
                {
                    var earliest = deviceHistory.First().Date;

                    if (earliest < historyFrom)
                    {
                        logReport.Add("Device history OK. From:", earliest);
                    }
                    else
                    {
                        logReport.Add("Device history insufficient. From:", earliest);
                    }
                }
                else
                {
                    logReport.Add("Device history insufficient", "(none)");
                }

                var history = DeviceAndSolarHistory.CreateHistory(solarHistory, deviceHistory);

                var latestHistory = history.LastOrDefault() ?? new DeviceAndSolarHistory();

                var durationCallback = CreateDurationFunction(history);

                var interpreter = new Interpreter(InterpreterOptions.LambdaExpressions);

                var parameters = typeof(DeviceAndSolarHistory)
                    .GetProperties()
                    .Select(x => new Parameter(x.Name, x.PropertyType, x.GetValue(latestHistory)))
                    .ToList();

                parameters.Add(new Parameter("Duration", typeof(Func<int, Func<DeviceAndSolarHistory, bool>, bool>), durationCallback));

                var result = (bool)interpreter.Eval(condition, parameters.ToArray());

                logReport.Add("CONDITION RESULT", result);

                return result;
            }
            catch (Exception ex)
            {
                logReport.Logger.LogError(ex, $"Failed to evaluate condition {condition}");
                logReport.Add("CONDITION FAILED", ex.Message);

                return false;
            }
        }

        private Func<int, Func<DeviceAndSolarHistory, bool>, bool> CreateDurationFunction(List<DeviceAndSolarHistory> history)
        {
            Func<int, Func<DeviceAndSolarHistory, bool>, bool> func = (seconds, condition) =>
            {
                var from = DateTimeNow.UtcNow.AddSeconds(-seconds);

                var relevantHistory = history.Where(x => x.Date > from).ToList();

                if (!relevantHistory.Any())
                {
                    return false;
                }

                return relevantHistory.All(condition);
            };

            return func;
        }
    }
}
