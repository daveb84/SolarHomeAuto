using SolarHomeAuto.Domain.Auth.Models;

namespace SolarHomeAuto.Domain.Auth
{
    public interface IAuthService
    {
        Task Logout();
        Task<bool> Authenticate(AuthenticateModel credentials);
    }
}
