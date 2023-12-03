using SolarHomeAuto.Domain.Auth.Models;

namespace SolarHomeAuto.UI.Services.Default
{
    public class NullAuthPageService : IAuthPageService
    {
        public bool AuthEnabled => false;

        public Task<bool> Authenticate(AuthenticateModel credentials)
        {
            return Task.FromResult(false);
        }

        public Task Logout()
        {
            return Task.CompletedTask;
        }
    }
}
