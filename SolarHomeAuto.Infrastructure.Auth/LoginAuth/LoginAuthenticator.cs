using SolarHomeAuto.Domain.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SolarHomeAuto.Domain.Auth.Models;

namespace SolarHomeAuto.Infrastructure.Auth.LoginAuth
{
    public class LoginAuthenticator : IAuthService
    {
        private readonly AuthSettings settings;
        private readonly IHttpContextAccessor httpContent;

        public LoginAuthenticator(AuthSettings settings, IHttpContextAccessor httpContent)
        {
            this.settings = settings;
            this.httpContent = httpContent;
        }

        public async Task<bool> Authenticate(AuthenticateModel credentials)
        {
            if (string.IsNullOrEmpty(credentials?.Password)) return false;

            var hashed = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(credentials.Password)));

            if (string.Equals(hashed, settings.AuthKeyHash, StringComparison.OrdinalIgnoreCase) || settings.AllowAnyAuth)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "SimpleUser")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await httpContent.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return true;
            }

            return false;
        }

        public async Task Logout()
        {
            await httpContent.HttpContext.SignOutAsync();
        }
    }
}
