namespace ForgiveMeCalia.Application.Abstractions;

public interface IProgressReporter
{
    void ReportPhase(string message);
    void ReportDiscovery(int current, int total, string itemName);
    void ReportDownload(int current, int total, string fileName, double fileProgress);
    void ReportSkipped(string reason, string itemName);
    void ReportWarning(string message);
    void ReportError(string message, Exception? exception = null);
    void ReportSummary(DownloadSummary summary);
}

public sealed record DownloadSummary(
    int Discovered,
    int Downloaded,
    int Skipped,
    int Locked,
    int Failed,
    IReadOnlyList<DownloadFailure> Failures)
{
    public static DownloadSummary Create(
        int discovered,
        int downloaded,
        int skipped,
        int locked,
        IReadOnlyList<DownloadFailure> failures) =>
        new(discovered, downloaded, skipped, locked, failures.Count, failures);
}
