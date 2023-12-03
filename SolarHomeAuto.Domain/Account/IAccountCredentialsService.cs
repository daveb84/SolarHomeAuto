namespace SolarHomeAuto.Domain.Account
{
    public interface IAccountCredentialsService
    {
        Task<T> GetAccountCredentials<T>(string key);
    }
}
