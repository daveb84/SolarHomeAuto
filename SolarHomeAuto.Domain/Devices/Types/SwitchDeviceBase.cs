using Microsoft.Extensions.Logging;
using SolarHomeAuto.Domain.Devices.Models;

namespace SolarHomeAuto.Domain.Devices.Types
{
    public abstract class SwitchDeviceBase : ISwitchDevice
    {
        private readonly DeviceConnectionSettings device;
        private readonly IDeviceService deviceService;
        private readonly ILogger logger;

        public SwitchDeviceBase(DeviceConnectionSettings device, IDeviceService deviceService, ILogger logger)
        {
            this.device = device;
            this.deviceService = deviceService;
            this.logger = logger;
        }
        public string DeviceId => device.DeviceId;
        public string Name => device.Name;
        public Type StateType => typeof(SwitchHistoryState);

        public async Task<SwitchStatusData> GetStatus()
        {
            try
            {
                var status = await GetStatusInternal();

                if (status.SaveHistory)
                {
                    var state = new SwitchHistoryState
                    {
                        Action = SwitchAction.None,
                        Status = status.Result.Status,
                        Power = status.Result.Power,
                    };
                    
                    await deviceService.SaveStateChange(DeviceId, state, "RefreshStatus", null);
                }

                return status.Result;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to get device status for {Name} {DeviceId}");

                return new SwitchStatusData
                {
                    Status = SwitchStatus.Offline,
                };
            }
        }

        public async Task<SwitchActionResult> Switch(SwitchAction action, string eventSource)
        {
            if (action == SwitchAction.None) return SwitchActionResult.NoAction;

            SwitchOnOffDeviceResult result = null;

            try
            {
                var actionResult = await SwitchInternal(action);

                if (actionResult == SwitchActionResult.Success)
                {
                    result = new SwitchOnOffDeviceResult
                    {
                        Success = true,
                        Attempted = action,
                        Result = new SwitchStatusData
                        {
                            Status = action == SwitchAction.TurnOn
                                ? SwitchStatus.On
                                : SwitchStatus.Off,
                        }
                    };
                }
                else if (actionResult == SwitchActionResult.Failure)
                {
                    result = await GetResultForFailed(action);
                }
                else
                {
                    return SwitchActionResult.NoAction;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to perform switch action {action} device {Name} {DeviceId}");

                result = await GetResultForFailed(action);
            }
            finally
            {
                var state = new SwitchHistoryState
                {
                    Action = action,
                    Status = result.Result.Status,
                    Power = result.Result.Power,
                };

                var error = !result.Success
                    ? $"Failed to perform switch action {action} device. Status is {result.Result}"
                    : null;

                await deviceService.SaveStateChange(DeviceId, state, eventSource, error);
            }

            return result.Success 
                ? SwitchActionResult.Success 
                : SwitchActionResult.Failure;
        }

        private async Task<SwitchOnOffDeviceResult> GetResultForFailed(SwitchAction attempted)
        {
            var status = await GetStatusInternal();

            var result = new SwitchOnOffDeviceResult
            {
                Attempted = attempted,
                Success = false,
                Result = status.Result,
            };

            return result;
        }

        protected class SwitchStatusResult
        {
            public SwitchStatusResult(SwitchStatus status, decimal? power = null, bool saveHistory = true)
            {
                Result = new SwitchStatusData
                {
                    Status = status,
                    Power = power
                };

                SaveHistory = saveHistory;
            }

            public bool SaveHistory { get; }
            public SwitchStatusData Result { get; }
        }

        protected class SwitchOnOffDeviceResult
        {
            public SwitchAction Attempted { get; set; }
            public bool Success { get; set; }
            public SwitchStatusData Result { get; set; }
        }

        protected abstract Task<SwitchActionResult> SwitchInternal(SwitchAction action);
        protected abstract Task<SwitchStatusResult> GetStatusInternal();
    }
}
