using Microsoft.AspNetCore.HeaderPropagation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
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
                // Initizalize the headers collections as workaround
                var headerPropagationValues = scope.ServiceProvider.GetRequiredService<HeaderPropagationValues>();
                headerPropagationValues.Headers = new Dictionary<string, StringValues>(StringComparer.OrdinalIgnoreCase);
                //eventually set headers coming from other sources (e.g. consuming a queue)
                headerPropagationValues.Headers.Add("User-Agent", "background-service");

                var client = scope.ServiceProvider.GetRequiredService<GitHubClient>();

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
