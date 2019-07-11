using System.Net.Http;
using System.Threading.Tasks;

namespace WebApplication
{
    public class GitHubClient
    {
        public GitHubClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private HttpClient _httpClient;

        public async Task<string> GetSomething()
        {
            var response = await _httpClient.GetAsync("/users/alefranz");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
