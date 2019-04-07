using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly GitHubClient _gitHubClient;

        public SampleController(GitHubClient gitHubClient)
        {
            _gitHubClient = gitHubClient;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return await _gitHubClient.GetSomething();
        }
    }
}
