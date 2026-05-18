using System.Text.Json;
using ForgiveMeCalia.Application.Abstractions;

namespace ForgiveMeCalia.Infrastructure.Persistence;

public sealed class JsonDownloadIndexStore(ILibraryPathProvider libraryPaths) : IDownloadIndexStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<HashSet<string>> LoadCompletedSourceUrlsAsync(CancellationToken cancellationToken)
    {
        var path = GetIndexPath();
        if (!File.Exists(path))
            return [];

        await using var stream = File.OpenRead(path);
        var items = await JsonSerializer.DeserializeAsync<List<string>>(stream, SerializerOptions, cancellationToken);
        return items?.ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];
    }

    public async Task MarkCompletedAsync(string sourceUrl, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var set = await LoadCompletedSourceUrlsAsync(cancellationToken);
            if (!set.Add(sourceUrl))
                return;

            await SaveAsync(set, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task RemoveCompletedAsync(IEnumerable<string> sourceUrls, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var set = await LoadCompletedSourceUrlsAsync(cancellationToken);
            var changed = false;
            foreach (var sourceUrl in sourceUrls)
                changed |= set.Remove(sourceUrl);

            if (!changed)
                return;

            await SaveAsync(set, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task SaveAsync(HashSet<string> set, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(GetIndexPath())!);
        await using var stream = File.Create(GetIndexPath());
        await JsonSerializer.SerializeAsync(stream, set.OrderBy(x => x).ToList(), SerializerOptions, cancellationToken);
    }

    private string GetIndexPath() =>
        Path.Combine(libraryPaths.GetLibraryRoot(), ".download-index.json");
}
