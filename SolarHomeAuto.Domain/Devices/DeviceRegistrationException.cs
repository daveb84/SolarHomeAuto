namespace SolarHomeAuto.Domain.Devices
{
    public class DeviceRegistrationException : Exception
    {
        public DeviceRegistrationException(string deviceId, Type type, Type wrongType)
            : base($"Device with ID {deviceId} is of type {type?.FullName} and cannot be converted to {wrongType?.FullName}")
        {
        }

        public DeviceRegistrationException(string deviceId, int? numberOfRegistrations = null)
            : base(ErrorMessage(deviceId, numberOfRegistrations))
        {
        }

        private static string ErrorMessage(string deviceId, int? numberOfRegistrations = null)
        {
            if (numberOfRegistrations.HasValue)
            {
                if (numberOfRegistrations <= 0)
                {
                    return $"Device with ID {deviceId} is not registered";
                }

                if (numberOfRegistrations > 1)
                {
                    return $"Device with ID {deviceId} has been registered {numberOfRegistrations} times";
                }
            }

            return $"Device with ID {deviceId} has already been registered";
        }
    }
}
