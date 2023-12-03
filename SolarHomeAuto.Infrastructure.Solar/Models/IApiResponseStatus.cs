namespace SolarHomeAuto.Infrastructure.Solar.Models
{
    internal interface IApiResponseStatus
    {
        string Message { get; }
        string Code { get; }
        bool Success { get; }
    }
}
