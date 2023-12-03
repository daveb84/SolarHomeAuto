using SolarHomeAuto.Domain.Auth.Models;

namespace SolarHomeAuto.UI.Services
{
    public interface IAuthPageService
    {
        bool AuthEnabled { get; }
        Task<bool> Authenticate(AuthenticateModel credentials);

        Task Logout();
    }
}
