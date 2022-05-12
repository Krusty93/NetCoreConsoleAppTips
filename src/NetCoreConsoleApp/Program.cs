using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetCoreConsoleApp.Options;
using NetCoreConsoleApp.Services;
using Serilog;

[Command(
    Name = "NetCoreConsoleApp"
    , Description = "My app is very cool 😎")]
[HelpOption(
    "-h"
    , LongName = "help"
    , Description = "Get info")]
[VersionOptionFromMember(
    "-v"
    , MemberName = nameof(GetVersion))]
class Program
{
    [Option(
        "-o"
        , "Some option"
        , CommandOptionType.SingleValue
        , LongName = "option")]
    [Range(1, 5)]
    public int Option { get; } = 1;

    public static Task<int> Main(string[] args) =>
        CommandLineApplication.ExecuteAsync<Program>(args);

    public async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        var host = CreateHostBuilder();
        await host.RunConsoleAsync(cancellationToken);
        return Environment.ExitCode;
    }

    private IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
            })
            .UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
            })
            .ConfigureAppConfiguration((hostContext, builder) =>
            {
                builder.AddEnvironmentVariables();
            })
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();

                services.Configure<MyOptions>(options =>
                {
                    options.Value = Option;
                });

                services.AddTransient<IMyService, MyService>();
            });
    }

    private static string GetVersion()
        => typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
}

