using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Localization;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Infrastructure.Http;

namespace ForgiveMeCalia.Infrastructure.Auth;

public sealed class CookieSessionService(
    SiteCookieContainer cookieContainer,
    ILibraryPathProvider libraryPaths,
    IBrowserCookieExporter cookieExporter) : ICookieSessionService
{
    public async Task EnsureSessionAsync(bool tryImportIfMissing, CancellationToken cancellationToken)
    {
        var cookiePath = libraryPaths.GetCookieFilePath();

        if (!File.Exists(cookiePath) && tryImportIfMissing)
        {
            var export = await cookieExporter.ExportAsync(cancellationToken: cancellationToken);
            if (!export.Success)
                throw new InvalidOperationException(export.ErrorMessage ?? AppText.T("cookies.importFailed"));
        }

        if (!File.Exists(cookiePath))
        {
            throw new InvalidOperationException(AppText.T("cookies.fileMissing", cookiePath));
        }

        var cookies = NetscapeCookieReader.Read(cookiePath);
        if (cookies.Count == 0)
        {
            throw new InvalidOperationException(
                AppText.T("cookies.fileEmpty", cookiePath));
        }

        var siteUri = new Uri(DownloaderOptions.BaseUrl);
        foreach (var cookie in cookies)
            cookieContainer.Add(siteUri, cookie);
    }
}
