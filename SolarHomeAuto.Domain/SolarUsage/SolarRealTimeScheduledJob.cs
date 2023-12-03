using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.DataStore;
using SolarHomeAuto.Domain.Environment;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.PurgeData;
using SolarHomeAuto.Domain.ScheduledJobs;
using SolarHomeAuto.Domain.ScheduledJobs.Schedules;
using SolarHomeAuto.Domain.Solar;
using SolarHomeAuto.Domain.SolarUsage.Models;
using System.Globalization;

namespace SolarHomeAuto.Domain.SolarUsage
{
    public class SolarRealTimeScheduledJob : IScheduledJob
    {
        private const string SystemDataKey = "SolarRealTimeScheduleJobState";

        private readonly SolarScheduledJobSettings settings;
        private readonly IDataStoreFactory dataStoreFactory;
        private readonly ISolarApi solarApi;
        private readonly SolarRealTimeImporter importer;
        private readonly IPurgeDataService purgeDataService;
        private readonly ISolarRealTimeImportScheduleService scheduleService;

        public SolarRealTimeScheduledJob(SolarScheduledJobSettings settings, 
            IDataStoreFactory dataStoreFactory,
            ISolarApi solarApi, 
            SolarRealTimeImporter importer, 
            IPurgeDataService purgeDataService, 
            ISolarRealTimeImportScheduleService scheduleService)
        {
            this.settings = settings;
            this.dataStoreFactory = dataStoreFactory;
            this.solarApi = solarApi;
            this.importer = importer;
            this.purgeDataService = purgeDataService;
            this.scheduleService = scheduleService;
        }

        public async Task<bool> IsEnabled()
        {
            if (settings.RealTimeInterval <= 0)
            {
                return false;
            }

            var enabled = await solarApi.IsEnabled();

            return enabled;
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

        public async Task<DateTime> RunNext(DateTime now, LogReporter logReporter, CancellationToken cancellationToken)
        {
            var schedule = await GetSchedule();

            if (schedule == null)
            {
                logReporter.Add("NO SCHEDULES");

                return now.AddSeconds(settings.RealTimeInterval);
            }

            var next = await MoveToNext(schedule, logReporter, now);

            await SaveState(next);

            return next;
        }

        private async Task<DateTime> MoveToNext(ISchedule<SolarRealTimeSchedulePeriod> schedule, LogReporter logReporter, DateTime now)
        {
            var nextRun = await GetState();

            if (nextRun == null)
            {
                nextRun = now.AddSeconds(settings.RealTimeInitialWait);
            }

            if (nextRun <= now)
            {
                var period = schedule.GetCurrentPeriod(now);

                logReporter.Add("ACTION", period.Action);

                if (period.Action == SolarRealTimeScheduleAction.FetchData)
                {
                    try
                    {
                        await importer.RunImport();

                        logReporter.Add("SUCESS");
                    }
                    catch (Exception ex)
                    {
                        logReporter.Add("FAILED", $"{ex.Message}");

                        logReporter.Logger.LogError(ex, "Failed to import solar real time data");
                    }

                    nextRun = now.AddSeconds(settings.RealTimeInterval);
                }
                else if (period.Action == SolarRealTimeScheduleAction.StopAndPurgeData)
                {
                    try
                    {
                        await purgeDataService.PurgeData();

                        logReporter.Add("SUCESS");
                    }
                    catch (Exception ex)
                    {
                        logReporter.Add("FAILED", $"{ex.Message}");

                        logReporter.Logger.LogError(ex, "Failed to purge data");
                    }

                    nextRun = schedule.GetPeriodNextStartTime(schedule.GetNextPeriod(period), now);
                }
                else if (period.Action == SolarRealTimeScheduleAction.Stop)
                {
                    nextRun = schedule.GetPeriodNextStartTime(schedule.GetNextPeriod(period), now);
                }

                if (nextRun <= now)
                {
                    nextRun = now.AddSeconds(settings.RealTimeInterval);
                }
            }

            return nextRun.Value;
        }

        private async Task<ISchedule<SolarRealTimeSchedulePeriod>> GetSchedule()
        {
            var schedulePeriods = await scheduleService.GetSolarRealTimeImportSchedule();

            if (schedulePeriods?.Any() != true || !schedulePeriods.Any(x => x.Action != SolarRealTimeScheduleAction.Stop))
            {
                return null;
            }

            return new DailySchedule<SolarRealTimeSchedulePeriod>(schedulePeriods);
        }

        private async Task<DateTime?> GetState()
        {
            using (var store = dataStoreFactory.CreateStore())
            {
                var data = await store.GetSystemData(SystemDataKey);

                if (DateTime.TryParseExact(data?.Value, "s", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                {
                    return date;
                }

                return null;
            }
        }

        private async Task SaveState(DateTime nextRun)
        {
            using (var store = dataStoreFactory.CreateStore())
            using (var trans = store.CreateTransaction())
            {
                var state = new SystemData
                {
                    Key = SystemDataKey,
                    Value = nextRun.ToString("s")
                };

                await trans.SaveSystemData(state);
                await trans.Commit();
            }
        }
    }
}
