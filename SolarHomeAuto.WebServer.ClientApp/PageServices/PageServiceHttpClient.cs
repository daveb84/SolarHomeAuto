using Newtonsoft.Json;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace SolarHomeAuto.WebServer.ClientApp.PageServices
{
    public class PageServiceHttpClient
    {
        private readonly HttpClient httpClient;
        private readonly NavigationManager navigationManager;

        public PageServiceHttpClient(HttpClient httpClient, NavigationManager navigationManager)
        {
            this.httpClient = httpClient;
            this.navigationManager = navigationManager;
        }

        public async Task<T> SendRequest<T>(HttpMethod method, string url, object body = null, bool handleAuth = true)
        {
            var request = new HttpRequestMessage(method, url);

            if (body != null)
            {
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                request.Content = JsonContent.Create(body, mediaType: null, options: null);
            }

            using (var response = await httpClient.SendAsync(request))
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && handleAuth)
                {
                    navigationManager.NavigateTo("/login");

                    return default;
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to {method} URL: {url}. Status: {response.StatusCode}");
                }

                var content = await response.Content.ReadAsStringAsync();

                if (typeof(T) == typeof(string))
                {
                    return (T)(object)content;
                }

                return JsonConvert.DeserializeObject<T>(content);
            }
        }
    }
}
