using ForgiveMeCalia.Application;
using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Cli.Reporting;
using ForgiveMeCalia.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ForgiveMeCalia.Cli;

internal static class AppHostFactory
{
    public static IHost Create(int parallelCount = 4) =>
        Host.CreateDefaultBuilder()
            .ConfigureLogging(logging => logging.SetMinimumLevel(LogLevel.Warning))
            .ConfigureServices(services =>
            {
                services.Configure<DownloaderOptions>(o => o.MaxParallelDownloads = parallelCount);
                services.AddApplication();
                services.AddInfrastructure();
                services.AddSingleton<IProgressReporter, SpectreProgressReporter>();
            })
            .Build();
}
