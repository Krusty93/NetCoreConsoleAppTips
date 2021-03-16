using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreConsoleApp.Options;
using NetCoreConsoleApp.Services;

namespace NetCoreConsoleApp
{
    public class Worker : IHostedService
    {
        private readonly IMyService _myService;
        private readonly IHostApplicationLifetime _hostLifetime;
        private readonly string _configKey;
        private readonly ILogger<Worker> _logger;
        private readonly MyOptions _options;

        public Worker(IMyService service, IConfiguration configuration, IHostApplicationLifetime hostLifetime, ILogger<Worker> logger, IOptions<MyOptions> options)
        {
            _myService = service ?? throw new ArgumentNullException(nameof(service));
            _hostLifetime = hostLifetime ?? throw new ArgumentNullException(nameof(hostLifetime));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _configKey = configuration["ConfigKey"];
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Read {_configKey} from settings");

            await _myService.PerformLongTaskAsync();

            _hostLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger?.LogInformation($"Shutting down the service with code {Environment.ExitCode}");
            return Task.CompletedTask;
        }
    }
}