using Microsoft.AspNetCore.Mvc;

namespace SolarHomeAuto.Infrastructure.Auth.ApiKeyAuth
{
    public class ApiKeyAuthAttribute : TypeFilterAttribute
    {
        public ApiKeyAuthAttribute() : base(typeof(ApiKeyAuthFilter))
        {
        }
    }
}
