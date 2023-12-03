using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SolarHomeAuto.Domain.Auth;
using SolarHomeAuto.Domain.Auth.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SolarHomeAuto.Infrastructure.Auth.ApiKeyAuth
{
    internal class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly AuthSettings settings;

        public ApiKeyAuthFilter(AuthSettings settings)
        {
            this.settings = settings;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var key = context.HttpContext?.Request?.Headers[AuthConstants.ApiKeyHeaderKey].ToString();

            var hashed = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key ?? string.Empty)));

            if (!string.Equals(hashed, settings.ApiKeyHash, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
        }
    }
}
