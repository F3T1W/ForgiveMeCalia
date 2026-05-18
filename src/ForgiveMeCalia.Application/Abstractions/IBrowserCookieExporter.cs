namespace ForgiveMeCalia.Application.Abstractions;

public interface IBrowserCookieExporter
{
    Task<CookieExportResult> ExportAsync(string? browser = null, CancellationToken cancellationToken = default);

    IReadOnlyList<string> GetSupportedBrowsers();
}

public sealed record CookieExportResult(
    bool Success,
    string? Browser,
    string CookieFilePath,
    string? ErrorMessage,
    bool IsPermissionDenied,
    bool YtDlpWasInstalled)
{
    public static CookieExportResult Ok(string browser, string path, bool installed = false) =>
        new(true, browser, path, null, false, installed);

    public static CookieExportResult Fail(
        string path,
        string message,
        bool permissionDenied = false,
        bool installed = false) =>
        new(false, null, path, message, permissionDenied, installed);
}
