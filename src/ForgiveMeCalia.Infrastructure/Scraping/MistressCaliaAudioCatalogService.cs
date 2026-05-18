using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Domain.Entities;
using ForgiveMeCalia.Domain.Enums;
using ForgiveMeCalia.Infrastructure.Http;
using Microsoft.Extensions.Options;

namespace ForgiveMeCalia.Infrastructure.Scraping;

public sealed class MistressCaliaAudioCatalogService(
    MistressCaliaSiteClient siteClient,
    WordPressPostParser parser,
    IOptions<DownloaderOptions> options) : IAudioCatalogService
{
    private readonly DownloaderOptions _options = options.Value;

    public async Task<IReadOnlyList<string>> GetPostUrlsAsync(AudioTier tier, CancellationToken cancellationToken)
    {
        var categoryPath = tier switch
        {
            AudioTier.Free => _options.FreeCategoryPath,
            AudioTier.Paid => _options.PaidCategoryPath,
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
            var pagePosts = parser.ParseCategoryPostUrls(html, baseUri);
            foreach (var postUrl in pagePosts)
                discoveredPosts.Add(postUrl);

            foreach (var nextPage in parser.ParsePaginationPaths(html, categoryBase))
            {
                var normalized = NormalizePath(nextPage);
                if (!visitedPages.Contains(normalized))
                    pagesToVisit.Enqueue(normalized);
            }

            if (_options.CatalogRequestDelayMs > 0 && pagesToVisit.Count > 0)
                await Task.Delay(_options.CatalogRequestDelayMs, cancellationToken);
        }

        return discoveredPosts.OrderBy(u => u, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public async Task<AudioPost> GetPostDetailsAsync(string postUrl, AudioTier tier, CancellationToken cancellationToken)
    {
        var path = new Uri(postUrl).AbsolutePath;
        var html = await siteClient.GetStringAsync(path, cancellationToken);
        return parser.Parse(postUrl, tier, html);
    }

    private static string NormalizePath(string path)
    {
        if (!path.StartsWith('/'))
            path = "/" + path;
        return path.TrimEnd('/') + "/";
    }
}
