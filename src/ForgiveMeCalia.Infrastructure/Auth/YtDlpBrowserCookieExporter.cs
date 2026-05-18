using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Infrastructure.Platform;
using Microsoft.Extensions.Options;

namespace ForgiveMeCalia.Infrastructure.Auth;

public sealed class YtDlpBrowserCookieExporter(
    ILibraryPathProvider libraryPaths,
    IOptions<DownloaderOptions> options) : IBrowserCookieExporter
{
    /// <summary>
    /// yt-dlp требует URL с известным экстрактором, чтобы запустить экспорт cookies из браузера.
    /// Сам URL не важен — в файл попадают все cookies браузера, затем оставляем только нужный домен.
    /// </summary>
    private const string CookieExportProbeUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";

    private static readonly string[] DefaultBrowsers =
        ["safari", "chrome", "chromium", "brave", "firefox", "edge"];

    public IReadOnlyList<string> GetSupportedBrowsers() => DefaultBrowsers;

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
                """
                yt-dlp не найден, а Homebrew недоступен.
                Установите вручную: brew install yt-dlp
                или скачайте yt-dlp с https://github.com/yt-dlp/yt-dlp
                """);
        }

        var browsers = browser is { Length: > 0 }
            ? [browser]
            : DefaultBrowsers;

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

    private async Task<CookieExportResult> TryExportWithBrowserAsync(
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
            var siteHost = new Uri(options.Value.BaseUrl).Host;
            FilterCookiesForSite(cookiePath, siteHost);
            if (HasCookiesForHost(cookiePath, siteHost))
                return CookieExportResult.Ok(browser, cookiePath);

            if (result.ExitCode == 0)
            {
                return CookieExportResult.Fail(
                    cookiePath,
                    $"""
                    Браузер «{browser}»: cookies для {siteHost} не найдены.
                    Войдите на сайт через Patreon в этом браузере и повторите импорт.
                    """);
            }
        }

        var output = result.CombinedOutput;
        var permissionDenied = output.Contains("Operation not permitted", StringComparison.OrdinalIgnoreCase)
                               || output.Contains("Permission denied", StringComparison.OrdinalIgnoreCase);

        return CookieExportResult.Fail(
            cookiePath,
            $"Браузер «{browser}»: {Shorten(output)}",
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
            return lastError ?? "Не удалось экспортировать cookies.";

        return $"""
            macOS заблокировал доступ к cookies браузера (Operation not permitted).
            sudo это не обходит — нужны права для программы, из которой запускается ForgiveMeCalia.

            {GetMacPermissionHelp()}

            Последняя ошибка:
            {lastError}
            """;
    }

    public static string GetMacPermissionHelp() =>
        """
        1. Системные настройки → Конфиденциальность и безопасность → Полный доступ к диску.
        2. Включите Terminal (или Rider / iTerm — то, из чего вы запускаете dotnet run).
        3. Перезапустите терминал и снова выберите «Импорт cookies».
        4. В Safari должен быть выполнен вход на mistresscalia.com через Patreon.

        Если Safari не открывается даже с правами — войдите в Firefox/Chrome и выберите импорт с браузера firefox или chrome.
        """;

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
