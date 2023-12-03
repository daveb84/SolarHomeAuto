using Newtonsoft.Json;
using SolarHomeAuto.Domain.Devices.RemoteCommands;
using SolarHomeAuto.Domain.Devices.Types;
using SolarHomeAuto.Domain.Environment;

namespace SolarHomeAuto.Domain.RemoteCommands
{
    public class RemoteCommandMessageBuilder
    {
        private readonly EnvironmentSettings environment;

        public RemoteCommandMessageBuilder(EnvironmentSettings environment)
        {
            this.environment = environment;
        }

        public RemoteCommandMessage DeviceSwitch(string deviceId, SwitchAction action)
        {
            return new RemoteCommandMessage
            {
                Date = DateTimeNow.UtcNow,
                MessageId = Guid.NewGuid(),
                Source = environment.Name,

                Data = action.ToString(),
                Type = RemoteCommandTypes.DeviceSwitch,
                RelatedId = deviceId
            };
        }

        public RemoteCommandMessage DeviceEnable(string deviceId, bool enable)
        {
            return new RemoteCommandMessage
            {
                Date = DateTimeNow.UtcNow,
                MessageId = Guid.NewGuid(),
                Source = environment.Name,

                Data = enable.ToString(),
                Type = RemoteCommandTypes.DeviceEnable,
                RelatedId = deviceId
            };
        }

        public RemoteCommandMessage DeviceStatus(string deviceId, bool enabled, SwitchStatusData status)
        {
            var result = new DeviceRemoteCommandStatus
            {
                Enabled = enabled,
                Status = status
            };

            return new RemoteCommandMessage
            {
                Date = DateTimeNow.UtcNow,
                MessageId = Guid.NewGuid(),
                Source = environment.Name,

                Data = JsonConvert.SerializeObject(result),
                Type = RemoteCommandTypes.DeviceStatus,
                RelatedId = deviceId
            };
        }

        public RemoteCommandMessage WorkerStatus(bool isRunning)
        {
            return new RemoteCommandMessage
            {
                Date = DateTimeNow.UtcNow,
                MessageId = Guid.NewGuid(),
                Source = environment.Name,

                Data = isRunning.ToString(),
                Type = RemoteCommandTypes.WorkerStatusMessageType,
                Consumed = true,
                ConsumedResult = isRunning.ToString()
            };
        }

        public RemoteCommandMessage WorkerToggle(bool start)
        {
            return new RemoteCommandMessage
            {
                Date = DateTimeNow.UtcNow,
                MessageId = Guid.NewGuid(),
                Source = environment.Name,

                Data = start.ToString(),
                Type = RemoteCommandTypes.WorkerToggleMessageType,
            };
        }
    }
}
