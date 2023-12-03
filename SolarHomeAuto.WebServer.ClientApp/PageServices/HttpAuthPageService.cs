using Microsoft.AspNetCore.Components;
using SolarHomeAuto.Domain.Auth.Models;
using SolarHomeAuto.UI.Services;

namespace SolarHomeAuto.WebServer.ClientApp.PageServices
{
    public class HttpAuthPageService : IAuthPageService
    {
        private readonly PageServiceHttpClient httpClient;
        private readonly NavigationManager navigationManager;

        public HttpAuthPageService(PageServiceHttpClient httpClient, NavigationManager navigationManager)
        {
            this.httpClient = httpClient;
            this.navigationManager = navigationManager;
        }

        public bool AuthEnabled => true;

        public async Task<bool> Authenticate(AuthenticateModel credentials)
        {
            var result = await httpClient.SendRequest<bool>(HttpMethod.Post, "/api/Auth", credentials, false);

            if (result)
            {
                navigationManager.NavigateTo("/");
            }

            return result;
        }

        public async Task Logout()
        {
            await httpClient.SendRequest<string>(HttpMethod.Post, "/api/Auth/Logout", false);

            navigationManager.NavigateTo("/login");
        }
    }
}
