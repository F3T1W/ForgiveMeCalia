using System.Net;
using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Infrastructure.Http;
using Microsoft.Extensions.Options;

namespace ForgiveMeCalia.Infrastructure.Auth;

public sealed class CookieSessionService(
    SiteCookieContainer cookieContainer,
    ILibraryPathProvider libraryPaths,
    IBrowserCookieExporter cookieExporter,
    IOptions<DownloaderOptions> options) : ICookieSessionService
{
    public async Task EnsureSessionAsync(bool tryImportIfMissing, CancellationToken cancellationToken)
    {
        var cookiePath = libraryPaths.GetCookieFilePath();

        if (!File.Exists(cookiePath) && tryImportIfMissing)
        {
            var export = await cookieExporter.ExportAsync(cancellationToken: cancellationToken);
            if (!export.Success)
                throw new InvalidOperationException(export.ErrorMessage ?? "Не удалось импортировать cookies.");
        }

        if (!File.Exists(cookiePath))
        {
            throw new InvalidOperationException(
                $"""
                Файл cookies не найден: {cookiePath}

                В меню выберите «Импорт cookies из браузера» или выполните:
                  dotnet run --project src/ForgiveMeCalia.Cli -- cookies import
                """);
        }

        var cookies = NetscapeCookieReader.Read(cookiePath);
        if (cookies.Count == 0)
        {
            throw new InvalidOperationException(
                $"Файл cookies пуст: {cookiePath}{Environment.NewLine}Повторите импорт из браузера.");
        }

        var siteUri = new Uri(options.Value.BaseUrl);
        foreach (var cookie in cookies)
            cookieContainer.Add(siteUri, cookie);
    }
}
