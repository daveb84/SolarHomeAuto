using Newtonsoft.Json;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.RemoteCommands;
using SolarHomeAuto.Domain.ServerApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarHomeAuto.Domain.Devices.RemoteCommands
{
    public class DeviceRemoteCommandConsumer
    {
        private readonly RemoteCommandService remoteCommandService;
        private readonly IDeviceService deviceService;
        private readonly RemoteCommandMessageBuilder messageBuilder;

        public DeviceRemoteCommandConsumer(RemoteCommandService remoteCommandService, IDeviceService deviceService, RemoteCommandMessageBuilder messageBuilder)
        {
            this.remoteCommandService = remoteCommandService;
            this.deviceService = deviceService;
            this.messageBuilder = messageBuilder;
        }

        public async Task<List<string>> ProcessDevices()
        {
            var commands = (await remoteCommandService.GetUnconsumedMessages(RemoteCommandTypes.DeviceEnable))
                .Concat(await remoteCommandService.GetUnconsumedMessages(RemoteCommandTypes.DeviceSwitch))
                .ToList();

            var results = new List<string>();

            if (!commands.Any())
            {
                await PublishDeviceStatuses();

                return results;
            }

            var enabledDeviceIds = await ProcessEnableCommands(commands);

            var switchedDeviceIds = await ProcessSwitchCommands(commands);

            await PublishDeviceStatuses();

            return enabledDeviceIds
                .Concat(switchedDeviceIds)
                .Distinct()
                .ToList();
        }

        private async Task<List<string>> ProcessEnableCommands(List<RemoteCommandMessage> commands)
        {
            var deviceIds = new List<string>();

            var enableCommands = commands
                .Where(x => x.Type == RemoteCommandTypes.DeviceEnable)
                .GroupBy(x => x.RelatedId);

            foreach (var group in enableCommands)
            {
                var last = group.OrderByDescending(x => x.Date).FirstOrDefault();

                deviceIds.Add(last.RelatedId);

                var enable = bool.Parse(last.Data);

                await deviceService.EnableDevice(new Models.EnableDeviceRequest
                {
                    DeviceId = last.RelatedId,
                    Enable = enable,
                });

                await remoteCommandService.ConsumeMessages(new ConsumeRemoteCommandsModel
                {
                    MessageIds = group.Select(x => x.MessageId).ToList(),
                    Result = enable.ToString()
                });
            }

            return deviceIds;
        }

        private async Task<List<string>> ProcessSwitchCommands(List<RemoteCommandMessage> commands)
        {
            var deviceIds = new List<string>();

            var switchCommands = commands
                .Where(x => x.Type == RemoteCommandTypes.DeviceSwitch)
                .GroupBy(x => x.RelatedId);

            foreach (var group in switchCommands)
            {
                var last = group.OrderByDescending(x => x.Date).FirstOrDefault();

                deviceIds.Add(last.RelatedId);

                SwitchStatusData switchResult = null;

                var connection = await deviceService.GetDeviceConnection<ISwitchDevice>(last.RelatedId);

                if (connection != null)
                {
                    var action = Enum.Parse<SwitchAction>(last.Data);

                    if (action != SwitchAction.None)
                    {
                        await connection.Switch(action, "RemoteCommand");

                        switchResult = await connection.GetStatus();
                    }
                }

                await remoteCommandService.ConsumeMessages(new ConsumeRemoteCommandsModel
                {
                    MessageIds = group.Select(x => x.MessageId).ToList(),
                    Result = switchResult != null ? JsonConvert.SerializeObject(switchResult) : null
                });
            }

            return deviceIds;
        }

        private async Task PublishDeviceStatuses()
        {
            var devices = await deviceService.GetDevices();

            foreach (var device in devices)
            {
                var connection = await deviceService.GetDeviceConnection<ISwitchDevice>(device.DeviceId);

                if (connection != null)
                {
                    var status = await connection.GetStatus();

                    var message = messageBuilder.DeviceStatus(device.DeviceId, device.Enabled, status);

                    await remoteCommandService.Publish(message);
                }
            }
        }
    }
}
