using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Domain.Entities;
using ForgiveMeCalia.Domain.Enums;
using ForgiveMeCalia.Infrastructure.Http;

namespace ForgiveMeCalia.Infrastructure.Scraping;

public sealed class MistressCaliaAudioCatalogService(
    MistressCaliaSiteClient siteClient) : IAudioCatalogService
{
    public async Task<IReadOnlyList<string>> GetPostUrlsAsync(AudioTier tier, CancellationToken cancellationToken)
    {
        var categoryPath = tier switch
        {
            AudioTier.Free => DownloaderOptions.FreeCategoryPath,
            AudioTier.Paid => DownloaderOptions.PaidCategoryPath,
            _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
        };

        var baseUri = siteClient.CreateSiteUri();
        var categoryBase = NormalizePath(categoryPath);
        var discoveredPosts = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var pagesToVisit = new Queue<string>();
        var visitedPages = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        pagesToVisit.Enqueue(categoryBase);

        while (pagesToVisit.Count > 0)
        {
            var pagePath = pagesToVisit.Dequeue();
            if (!visitedPages.Add(pagePath))
                continue;

            var html = await siteClient.GetStringAsync(pagePath, cancellationToken);
            var pagePosts = WordPressPostParser.ParseCategoryPostUrls(html, baseUri);
            foreach (var postUrl in pagePosts)
                discoveredPosts.Add(postUrl);

            foreach (var nextPage in WordPressPostParser.ParsePaginationPaths(html, categoryBase))
            {
                var normalized = NormalizePath(nextPage);
                if (!visitedPages.Contains(normalized))
                    pagesToVisit.Enqueue(normalized);
            }

            if (DownloaderOptions.CatalogRequestDelayMs > 0 && pagesToVisit.Count > 0)
                await Task.Delay(DownloaderOptions.CatalogRequestDelayMs, cancellationToken);
        }

        return [.. discoveredPosts.OrderBy(u => u, StringComparer.OrdinalIgnoreCase)];
    }

    public async Task<AudioPost> GetPostDetailsAsync(string postUrl, AudioTier tier, CancellationToken cancellationToken)
    {
        var path = new Uri(postUrl).AbsolutePath;
        var html = await siteClient.GetStringAsync(path, cancellationToken);
        return WordPressPostParser.Parse(postUrl, tier, html);
    }

    private static string NormalizePath(string path)
    {
        if (!path.StartsWith('/'))
            path = "/" + path;
        return path.TrimEnd('/') + "/";
    }
}
