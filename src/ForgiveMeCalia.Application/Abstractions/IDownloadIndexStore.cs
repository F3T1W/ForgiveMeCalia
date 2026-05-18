namespace ForgiveMeCalia.Application.Abstractions;

public interface IDownloadIndexStore
{
    Task<HashSet<string>> LoadCompletedSourceUrlsAsync(CancellationToken cancellationToken);
    Task MarkCompletedAsync(string sourceUrl, CancellationToken cancellationToken);
    Task RemoveCompletedAsync(IEnumerable<string> sourceUrls, CancellationToken cancellationToken);
}
