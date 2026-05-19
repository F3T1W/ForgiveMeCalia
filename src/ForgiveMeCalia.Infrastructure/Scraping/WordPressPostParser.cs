using AngleSharp.Html.Parser;
using ForgiveMeCalia.Domain.Entities;
using ForgiveMeCalia.Domain.Enums;
using ForgiveMeCalia.Domain.Services;

namespace ForgiveMeCalia.Infrastructure.Scraping;

public static class WordPressPostParser
{
    private static readonly HtmlParser Parser = new();

    public static AudioPost Parse(string postUrl, AudioTier tier, string html)
    {
        var document = Parser.ParseDocument(html);
        var slug = new Uri(postUrl).AbsolutePath.Trim('/').Split('/').Last();
        var title = document.QuerySelector("h1.wp-block-post-title, h1.entry-title")
                         ?.TextContent
                         .Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            title = document.QuerySelector("article h1, main h1")?.TextContent.Trim();
        }

        if (string.IsNullOrWhiteSpace(title))
            title = SlugToTitle(slug);

        var mp3Links = document.QuerySelectorAll("a[href], audio[src], source[src]")
            .Select(e => e.GetAttribute("href") ?? e.GetAttribute("src"))
            .Where(u => u is not null && u.Contains(".mp3", StringComparison.OrdinalIgnoreCase))
            .Select(u => u!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var isLocked = html.Contains("patreon-flow", StringComparison.OrdinalIgnoreCase)
                       || html.Contains("patron-only", StringComparison.OrdinalIgnoreCase)
                       || document.QuerySelector(".patreon-locked-content, .patron-only") is not null || mp3Links.Count == 0 && document.Body?.InnerHtml.Contains("locked", StringComparison.OrdinalIgnoreCase) == true;

        var tags = document.QuerySelectorAll("a[rel='tag']")
            .Select(a => a.TextContent.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var series = SeriesNameParser.Parse(title, slug);

        return new AudioPost
        {
            Slug = slug,
            Title = title,
            Tier = tier,
            Mp3Url = mp3Links.FirstOrDefault(),
            IsLocked = isLocked && mp3Links.Count == 0,
            Tags = tags,
            SeriesKey = series.SeriesKey,
            SeriesPartNumber = series.PartNumber
        };

        static string SlugToTitle(string value) =>
            string.Join(' ', value.Split('-', StringSplitOptions.RemoveEmptyEntries));
    }

    public static IReadOnlyList<string> ParseCategoryPostUrls(string html, Uri baseUri)
    {
        var document = Parser.ParseDocument(html);
        var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var link in document.QuerySelectorAll(".wp-block-post-title a[href], h2.entry-title a[href]"))
        {
            var href = link.GetAttribute("href");
            if (string.IsNullOrWhiteSpace(href) || href.StartsWith('#'))
                continue;

            if (!Uri.TryCreate(baseUri, href, out var uri))
                continue;

            if (!uri.Host.Equals(baseUri.Host, StringComparison.OrdinalIgnoreCase))
                continue;

            if (uri.AbsolutePath.Contains("/category/", StringComparison.OrdinalIgnoreCase)
                || uri.AbsolutePath.Contains("/tag/", StringComparison.OrdinalIgnoreCase)
                || uri.AbsolutePath.Contains("/page/", StringComparison.OrdinalIgnoreCase))
                continue;

            urls.Add(uri.GetLeftPart(UriPartial.Path).TrimEnd('/') + "/");
        }

        return [.. urls.OrderBy(u => u, StringComparer.OrdinalIgnoreCase)];
    }

    public static IReadOnlyList<string> ParsePaginationPaths(string html, string categoryBasePath)
    {
        var document = Parser.ParseDocument(html);
        var categoryBase = GetCategoryBasePath(categoryBasePath);
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { categoryBase };

        foreach (var link in document.QuerySelectorAll("a[href]"))
        {
            var href = link.GetAttribute("href");
            if (string.IsNullOrWhiteSpace(href))
                continue;

            var path = ResolveSitePath(href, categoryBase);
            if (path is null || !IsCategoryPagePath(categoryBase, path))
                continue;

            paths.Add(path);
        }

        return [.. paths];
    }

    private static string GetCategoryBasePath(string path)
    {
        path = NormalizePath(path);
        const string pageMarker = "/page/";
        var pageIndex = path.LastIndexOf(pageMarker, StringComparison.OrdinalIgnoreCase);
        return pageIndex < 0 ? path : path[..pageIndex].TrimEnd('/') + "/";
    }

    private static string? ResolveSitePath(string href, string categoryBase)
    {
        if (Uri.TryCreate(href, UriKind.Absolute, out var absolute))
            return NormalizePath(absolute.AbsolutePath);

        if (href[0] == '/')
            return NormalizePath(href);

        var baseUri = new Uri($"https://placeholder.local{categoryBase}");
        return Uri.TryCreate(baseUri, href, out var relative)
            ? NormalizePath(relative.AbsolutePath)
            : null;
    }

    private static bool IsCategoryPagePath(string categoryBase, string path)
    {
        var basePath = GetCategoryBasePath(categoryBase).TrimEnd('/');
        path = NormalizePath(path).TrimEnd('/');

        if (path.Equals(basePath, StringComparison.OrdinalIgnoreCase))
            return true;

        if (!path.StartsWith(basePath + "/page/", StringComparison.OrdinalIgnoreCase))
            return false;

        var pageNumber = path[(basePath.Length + "/page/".Length)..];
        return pageNumber.Length > 0 && pageNumber.All(char.IsDigit);
    }

    private static string NormalizePath(string path)
    {
        if (path[0] != '/')
            path = "/" + path;

        return path.TrimEnd('/') + "/";
    }
}
