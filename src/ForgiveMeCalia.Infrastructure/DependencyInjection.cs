using System.Net;
using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Infrastructure.Archive;
using ForgiveMeCalia.Infrastructure.Auth;
using ForgiveMeCalia.Infrastructure.Audio;
using ForgiveMeCalia.Infrastructure.Downloads;
using ForgiveMeCalia.Infrastructure.Http;
using ForgiveMeCalia.Infrastructure.Persistence;
using ForgiveMeCalia.Infrastructure.Paths;
using ForgiveMeCalia.Infrastructure.Scraping;
using Microsoft.Extensions.DependencyInjection;

namespace ForgiveMeCalia.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<SiteCookieContainer>();

        services.AddHttpClient<MistressCaliaSiteClient>(client =>
            {
                client.BaseAddress = new Uri(DownloaderOptions.BaseUrl.TrimEnd('/') + "/");
                client.DefaultRequestHeaders.UserAgent.ParseAdd(
                    "ForgiveMeCalia/1.0 (+personal archival tool)");
            })
            .ConfigurePrimaryHttpMessageHandler(provider => new SocketsHttpHandler
            {
                // Avoid Brotli: long macOS sessions have shown fewer SSL/decompression failures this way.
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                CookieContainer = provider.GetRequiredService<SiteCookieContainer>(),
                UseCookies = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(2),
                ConnectTimeout = TimeSpan.FromSeconds(30)
            });

        services.AddSingleton<ILibraryPathProvider, LibraryPathProvider>();
        services.AddSingleton<IAudioCatalogService, MistressCaliaAudioCatalogService>();
        services.AddSingleton<IFileDownloadService, FileDownloadService>();
        services.AddSingleton<ICustomAudioService, CustomAudioService>();
        services.AddSingleton<ILibraryArchiveService, LibraryArchiveService>();
        services.AddSingleton<IDownloadIndexStore, JsonDownloadIndexStore>();
        services.AddSingleton<IBrowserCookieExporter, YtDlpBrowserCookieExporter>();
        services.AddSingleton<ICookieSessionService, CookieSessionService>();
    }
}
