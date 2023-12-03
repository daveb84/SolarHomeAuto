using Microsoft.AspNetCore.Mvc;
using SolarHomeAuto.UI.Services;
using SolarHomeAuto.Domain.Auth;
using SolarHomeAuto.Domain.Auth.Models;

namespace SolarHomeAuto.WebServer.Controllers
{
    public class AuthController : ControllerBase, IAuthPageService
    {
        private readonly IAuthService authenticator;

        public AuthController(IAuthService authenticator)
        {
            this.authenticator = authenticator;
        }

        public bool AuthEnabled => true;

        [HttpPost]
        [Route("api/Auth")]
        public Task<bool> Authenticate([FromBody]AuthenticateModel model)
        {
            return authenticator.Authenticate(model);
        }

        [HttpPost]
        [Route("api/Auth/Logout")]
        public async Task Logout()
        {
            await authenticator.Logout();
        }
    }
}