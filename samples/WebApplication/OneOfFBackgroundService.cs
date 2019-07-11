using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApplication
{
    public class OneOffBackgroundService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        public OneOffBackgroundService(IServiceProvider services, ILogger<OneOffBackgroundService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service is starting.");

            return DoWorkAsync();
        }

        private async Task DoWorkAsync()
        {
            _logger.LogInformation("Background Service is working.");

            using (var scope = _services.CreateScope())
            {
                var client =scope.ServiceProvider.GetRequiredService<GitHubClient>();

                var result = await client.GetSomething();

                _logger.LogInformation("Background Service:\n{result}", result);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Background Service is stopping.");

            return Task.CompletedTask;
        }
    }
}
