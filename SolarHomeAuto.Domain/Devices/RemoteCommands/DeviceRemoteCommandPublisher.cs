using Newtonsoft.Json;
using SolarHomeAuto.Domain.Devices.Models;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.RemoteCommands;
using System;

namespace SolarHomeAuto.Domain.Devices.RemoteCommands
{
    public class DeviceRemoteCommandPublisher
    {
        private readonly RemoteCommandService remoteCommandService;
        private readonly RemoteCommandMessageBuilder messageBuilder;

        public DeviceRemoteCommandPublisher(RemoteCommandService remoteCommandService, RemoteCommandMessageBuilder messageBuilder)
        {
            this.remoteCommandService = remoteCommandService;
            this.messageBuilder = messageBuilder;
        }

        public Task EnableDevice(EnableDeviceRequest enableDeviceRequest)
        {
            var message = messageBuilder.DeviceEnable(enableDeviceRequest.DeviceId, enableDeviceRequest.Enable);

            return remoteCommandService.Publish(message);
        }

        public async Task<bool> IsDeviceEnabled(string deviceId)
        {
            var (_, enabled) = await GetDeviceStatusData(deviceId);

            return enabled;
        }

        public async Task<SwitchStatus> GetDeviceStatus(string deviceId)
        {
            var (status, _) = await GetDeviceStatusData(deviceId);

            return status;
        }

        public Task SwitchDevice(string deviceId, SwitchAction action)
        {
            var message = messageBuilder.DeviceSwitch(deviceId, action);

            return remoteCommandService.Publish(message);
        }

        private async Task<(SwitchStatus, bool)> GetDeviceStatusData(string deviceId)
        {
            var statusMessages = await remoteCommandService.GetMessages(new RemoteCommandMessageFilter
            {
                LatestOnly = true,
                Types = new List<string> { RemoteCommandTypes.DeviceStatus },
                RelatedIds = new List<string> { deviceId }
            });

            var switchMessages = await remoteCommandService.GetMessages(new RemoteCommandMessageFilter
            {
                LatestOnly = true,
                Types = new List<string> { RemoteCommandTypes.DeviceSwitch },
                RelatedIds = new List<string> { deviceId }
            });

            var enableMessages = await remoteCommandService.GetMessages(new RemoteCommandMessageFilter
            {
                LatestOnly = true,
                Types = new List<string> { RemoteCommandTypes.DeviceEnable },
                RelatedIds = new List<string> { deviceId }
            });

            var allMessages = statusMessages
                .Concat(switchMessages)
                .Concat(enableMessages)
                .OrderByDescending(x => x.Date)
                .ToList();

            return GetDeviceStatusData(allMessages);
        }

        private (SwitchStatus, bool) GetDeviceStatusData(List<RemoteCommandMessage> messages)
        {
            SwitchStatus? status = null;
            bool? enabled = null;

            foreach (var message in messages)
            {
                if (message.Type == RemoteCommandTypes.DeviceStatus)
                {
                    var data = JsonConvert.DeserializeObject<DeviceRemoteCommandStatus>(message.Data);

                    status = status ?? data?.Status?.Status ?? SwitchStatus.Offline;
                    enabled = enabled ?? data?.Enabled ?? false;

                    return (status.Value, enabled.Value);
                }

                if (message.Type == RemoteCommandTypes.DeviceEnable && !enabled.HasValue)
                {
                    enabled = bool.TryParse(message.Consumed ? message.ConsumedResult : message.Data, out var converted) ? converted : false;
                }

                if (message.Type == RemoteCommandTypes.DeviceSwitch && !status.HasValue)
                {
                    if (message.Consumed)
                    {
                        var switchData = message.ConsumedResult != null 
                            ? JsonConvert.DeserializeObject<SwitchStatusData>(message.ConsumedResult) 
                            : null;

                        status = switchData?.Status ?? SwitchStatus.Offline;
                    }
                    else
                    {
                        var action = Enum.TryParse<SwitchAction>(message.Data, true, out var convertedAction)
                            ? convertedAction
                            : SwitchAction.None;

                        status = action switch
                        {
                            SwitchAction.TurnOn => SwitchStatus.On,
                            SwitchAction.TurnOff => SwitchStatus.Off,
                            _ => SwitchStatus.Offline
                        };
                    }
                }

                if (enabled.HasValue && status.HasValue)
                {
                    return (status.Value, enabled.Value);
                }
            }

            return (status ?? SwitchStatus.Offline, enabled ?? false);
        }
    }
}
