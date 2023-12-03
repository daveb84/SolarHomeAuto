using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SolarHomeAuto.Domain.Devices.RemoteCommands;
using SolarHomeAuto.Domain.Devices.Schedule;
using SolarHomeAuto.Domain.Logging;
using SolarHomeAuto.Domain.Monitoring.RemoteCommands;
using SolarHomeAuto.Domain.ScheduledJobs;
using SolarHomeAuto.Domain.SolarExcess;
using SolarHomeAuto.Domain.SolarUsage;

namespace SolarHomeAuto.Domain.Monitoring
{
    public class MonitoringWorker
    {
        public static IReadOnlyCollection<string> MonitoringLoggerNames
        {
            get
            {
                return new[]
                {
                    typeof(MonitoringWorker).FullName,
                    typeof(DeviceScheduleJob).FullName,
                    typeof(SolarRealTimeScheduledJob).FullName,
                    typeof(SolarExcessEvaluator).FullName,
                };
            }
        }

        private readonly ILogger<MonitoringWorker> logger;
        private readonly MonitoringRemoteCommandConsumer remoteCommandConsumer;
        private readonly DeviceRemoteCommandConsumer deviceRemoteCommandProcesser;
        private readonly MonitoringService monitoringService;
        private readonly MonitoringSettings settings;
        private readonly List<IScheduledJob> jobs;

        public MonitoringWorker(MonitoringSettings settings, IEnumerable<IScheduledJob> jobs, ILogger<MonitoringWorker> logger, MonitoringRemoteCommandConsumer remoteCommandConsumer, DeviceRemoteCommandConsumer deviceRemoteCommandProcesser, MonitoringService monitoringService)
        {
            this.logger = logger;
            this.remoteCommandConsumer = remoteCommandConsumer;
            this.deviceRemoteCommandProcesser = deviceRemoteCommandProcesser;
            this.monitoringService = monitoringService;
            this.settings = settings;
            this.jobs = jobs.ToList();
        }

        public async Task Start(CancellationToken stoppingToken)
        {
            try
            {
                foreach (var job in jobs)
                {
                    await job.Reset();
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation($"MonitoringWorker starting but cancelled. Exiting.");

                    return;
                }

                logger.LogInformation($"MonitoringWorker starting. Settings: {JsonConvert.SerializeObject(settings)}");

                await DateTimeNow.Delay(settings.InitialWaitSeconds * 1000, stoppingToken);

                while (!stoppingToken.IsCancellationRequested)
                {
                    int waitSeconds = 0;

                    try
                    {
                        using (var logReporter = new LogReporter("Monitoring cycle starting", logger))
                        {
                            if (stoppingToken.IsCancellationRequested)
                            {
                                logReporter.Add("EXITING", $"MonitoringWorker cancelled");

                                return;
                            }

                            waitSeconds = await RunJobs(logReporter, stoppingToken);

                            if (waitSeconds < 0)
                            {
                                logReporter.Add("EXITING", $"Wait time returned: {waitSeconds}");

                                return;
                            }

                            if (waitSeconds < settings.MinIntervalSeconds)
                            {
                                waitSeconds = settings.MinIntervalSeconds;
                            }

                            if (waitSeconds > settings.MaxIntervalSeconds)
                            {
                                waitSeconds = settings.MaxIntervalSeconds;
                            }

                            logReporter.Add("NEXT CYCLE WAIT TIME", $"{waitSeconds}s");
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        logger.LogDebug("Monitoring cycle task cancelled.");
                        throw;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, $"Monitoring cycle threw exception. Waiting {settings.FailedRetrySeconds}");

                        waitSeconds = settings.FailedRetrySeconds;
                    }

                    await DateTimeNow.Delay(waitSeconds * 1000, stoppingToken);
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation($"MonitoringWorker cancelled. Exiting.");

                    return;
                }
            }
            catch (TaskCanceledException ex)
            {
                logger.LogInformation(ex, "MonitoringWorker task cancelled. Exiting.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "MonitoringWorker threw an exception. Exiting");
            }
        }

        private async Task<int> RunJobs(LogReporter logReporter, CancellationToken stoppingToken)
        {
            var enabledJobs = new List<IScheduledJob>();

            var isHost = await monitoringService.IsMonitoringHost();

            if (!isHost)
            {
                logReporter.Add("WORKER IS REMOTE HOST - jobs not applicable");
                logReporter.Level = LogLevel.Trace;

                // for server - not currently a host, but the worker runs anyway to catch if the mode is changed.
                // not applicable for mobile app, which has it's own server that stops the worker altogether.
                return settings.RemoteCommandsWaitSeconds;
            }

            if (stoppingToken.IsCancellationRequested) return -1;

            var isEnabled = await remoteCommandConsumer.CheckIfJobsEnabled();

            if (isEnabled)
            {
                foreach (var job in jobs)
                {
                    if (await job.IsEnabled())
                    {
                        enabledJobs.Add(job);
                    }
                }
            }

            if (stoppingToken.IsCancellationRequested) return -1;

            if (!enabledJobs.Any())
            {
                logReporter.Add("NO JOBS ENABLED");
                logReporter.Level = LogLevel.Debug;

                await deviceRemoteCommandProcesser.ProcessDevices();

                return settings.RemoteCommandsWaitSeconds;
            }

            var now = DateTimeNow.UtcNow;

            var nextRunTimes = new List<DateTime>();

            foreach (var job in enabledJobs)
            {
                if (stoppingToken.IsCancellationRequested) return -1;

                var jobName = job.GetType().Name;

                try
                {
                    logReporter.Add("");
                    logReporter.Add("JOB STARTING:", jobName);

                    var nextRunTime = await job.RunNext(now, logReporter, stoppingToken);

                    logReporter.Add("JOB COMPLETE: ", $"{jobName} - Next run: {nextRunTime.UtcToLocalLong()}");

                    nextRunTimes.Add(nextRunTime);
                }
                catch (Exception ex)
                {
                    var nextRunTime = now.AddSeconds(settings.FailedRetrySeconds);

                    logReporter.Add("JOB ERROR: ", $"{job.GetType().Name} - {ex.Message} - Next run: {nextRunTime.UtcToLocalLong()}");

                    logger.LogError(ex, $"Error executing job {job.GetType().Name}");

                    nextRunTimes.Add(nextRunTime);
                }
            }

            if (stoppingToken.IsCancellationRequested) return -1;

            var waitForSeconds = (int)Math.Ceiling((nextRunTimes.Min() - now).TotalSeconds);

            return waitForSeconds;
        }
    }
}
