using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Localization;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Infrastructure.Platform;

namespace ForgiveMeCalia.Infrastructure.Auth;

public sealed class YtDlpBrowserCookieExporter(
    ILibraryPathProvider libraryPaths) : IBrowserCookieExporter
{
    /// <summary>
    /// yt-dlp needs a URL handled by a known extractor before it exports browser cookies.
    /// The URL itself is irrelevant: yt-dlp exports all browser cookies, then we keep only the target domain.
    /// </summary>
    private const string CookieExportProbeUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

    private static readonly string[] MacBrowsers =
        ["safari", "chrome", "chromium", "brave", "firefox", "edge"];

    private static readonly string[] DesktopBrowsers =
        ["chrome", "chromium", "brave", "firefox", "edge"];

    public IReadOnlyList<string> GetSupportedBrowsers() => OperatingSystem.IsMacOS()
        ? MacBrowsers
        : DesktopBrowsers;

    public async Task<CookieExportResult> ExportAsync(
        string? browser = null,
        CancellationToken cancellationToken = default)
    {
        var cookiePath = libraryPaths.GetCookieFilePath();
        Directory.CreateDirectory(Path.GetDirectoryName(cookiePath)!);

        var ytDlpInstalled = await EnsureYtDlpAsync(cancellationToken);
        if (!ytDlpInstalled)
        {
            return CookieExportResult.Fail(
                cookiePath,
                AppText.T("cookies.ytDlpMissing"));
        }

        var browsers = browser is { Length: > 0 }
            ? [browser]
            : GetSupportedBrowsers();

        string? lastError = null;
        var permissionDenied = false;

        foreach (var candidate in browsers)
        {
            var attempt = await TryExportWithBrowserAsync(candidate, cookiePath, cancellationToken);
            if (attempt.Success)
                return CookieExportResult.Ok(candidate, cookiePath, ytDlpInstalled);

            lastError = attempt.ErrorMessage;
            permissionDenied |= attempt.IsPermissionDenied;
        }

        return CookieExportResult.Fail(
            cookiePath,
            BuildFailureMessage(lastError, permissionDenied),
            permissionDenied,
            ytDlpInstalled);
    }

    private static async Task<CookieExportResult> TryExportWithBrowserAsync(
        string browser,
        string cookiePath,
        CancellationToken cancellationToken)
    {
        if (File.Exists(cookiePath))
            File.Delete(cookiePath);

        var args = new List<string>
        {
            "--cookies-from-browser", browser,
            "--cookies", cookiePath,
            "--skip-download",
            "--quiet",
            "--no-warnings",
            CookieExportProbeUrl
        };

        var result = await ExternalProcessRunner.RunAsync("yt-dlp", args, cancellationToken);
        if (File.Exists(cookiePath) && new FileInfo(cookiePath).Length > 0)
        {
            var siteHost = new Uri(DownloaderOptions.BaseUrl).Host;
            FilterCookiesForSite(cookiePath, siteHost);
            if (HasCookiesForHost(cookiePath, siteHost))
                return CookieExportResult.Ok(browser, cookiePath);

            if (result.ExitCode == 0)
            {
                return CookieExportResult.Fail(
                    cookiePath,
                    AppText.T("cookies.noCookiesForHost", browser, siteHost));
            }
        }

        var output = result.CombinedOutput;
        var permissionDenied = output.Contains("Operation not permitted", StringComparison.OrdinalIgnoreCase)
                               || output.Contains("Permission denied", StringComparison.OrdinalIgnoreCase);

        return CookieExportResult.Fail(
            cookiePath,
            AppText.T("cookies.browserError", browser, Shorten(output)),
            permissionDenied);
    }

    private static void FilterCookiesForSite(string cookiePath, string siteHost)
    {
        var hostKey = siteHost.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
            ? siteHost[4..]
            : siteHost;

        var filtered = File.ReadAllLines(cookiePath)
            .Where(line => line.StartsWith('#')
                           || string.IsNullOrWhiteSpace(line)
                           || line.Split('\t')[0].Contains(hostKey, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        File.WriteAllLines(cookiePath, filtered);
    }

    private static bool HasCookiesForHost(string cookiePath, string siteHost)
    {
        var hostKey = siteHost.StartsWith("www.", StringComparison.OrdinalIgnoreCase)
            ? siteHost[4..]
            : siteHost;

        return File.ReadAllLines(cookiePath).Any(line =>
            !line.StartsWith('#')
            && !string.IsNullOrWhiteSpace(line)
            && line.Split('\t')[0].Contains(hostKey, StringComparison.OrdinalIgnoreCase));
    }

    private static async Task<bool> EnsureYtDlpAsync(CancellationToken cancellationToken)
    {
        if (await ExternalProcessRunner.ExistsOnPathAsync("yt-dlp", cancellationToken))
            return true;

        if (!await ExternalProcessRunner.ExistsOnPathAsync("brew", cancellationToken))
            return false;

        var install = await ExternalProcessRunner.RunAsync(
            "brew",
            ["install", "yt-dlp"],
            cancellationToken);

        return install.ExitCode == 0
               && await ExternalProcessRunner.ExistsOnPathAsync("yt-dlp", cancellationToken);
    }

    private static string BuildFailureMessage(string? lastError, bool permissionDenied)
    {
        if (!permissionDenied)
            return lastError ?? AppText.T("cookies.exportFailed");

        var intro = OperatingSystem.IsMacOS()
            ? AppText.T("cookies.permissionMac")
            : AppText.T("cookies.permissionOther");

        return $"""
            {intro}

            {GetPermissionHelp()}

            {AppText.T("cookies.lastError")}
            {lastError}
            """;
    }

    public static string GetPermissionHelp() =>
        OperatingSystem.IsMacOS()
            ? AppText.T("cookies.permissionHelpMac")
            : AppText.T("cookies.permissionHelpOther");

    private static string Shorten(string value)
    {
        var line = value.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault(l => l.Contains("error", StringComparison.OrdinalIgnoreCase)
                                 || l.Contains("Errno", StringComparison.OrdinalIgnoreCase)
                                 || l.Contains("Operation", StringComparison.OrdinalIgnoreCase))
                         ?? value.Trim();

        return line.Length <= 300 ? line : line[..300] + "...";
    }
}
