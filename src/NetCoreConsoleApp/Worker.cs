using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreConsoleApp.Options;
using NetCoreConsoleApp.Services;


public class Worker : IHostedService
{
    private readonly IMyService _myService;
    private readonly IHostApplicationLifetime _hostLifetime;
    private readonly string _configKey;
    private readonly ILogger<Worker> _logger;
    private readonly MyOptions _options;

    private int? _exitCode;

    public Worker(
        IMyService service,
        IConfiguration configuration,
        IHostApplicationLifetime hostLifetime,
        ILogger<Worker> logger,
        IOptions<MyOptions> options)
    {
        _myService = service ??
            throw new ArgumentNullException(nameof(service));

        _hostLifetime = hostLifetime ??
            throw new ArgumentNullException(nameof(hostLifetime));

        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));

        _options = options?.Value ??
            throw new ArgumentNullException(nameof(options));

        _configKey = configuration?["ConfigKey"] ??
            throw new ArgumentNullException(nameof(configuration));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Read {key} from settings", _configKey);

        try
        {
            await _myService.PerformLongTaskAsync();

            _exitCode = 0;
        }
        catch (OperationCanceledException)
        {
            _logger?.LogInformation("The job has been killed with CTRL+C");
            _exitCode = -1;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "An error occurred");
            _exitCode = 1;
        }
        finally
        {
            _hostLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        _logger?.LogInformation("Shutting down the service with code {exitCode}", Environment.ExitCode);
        return Task.CompletedTask;
    }
}
