using System.Collections.Concurrent;
using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Options;
using ForgiveMeCalia.Application.Downloads;
using ForgiveMeCalia.Application.Localization;
using ForgiveMeCalia.Application.Services;
using ForgiveMeCalia.Domain.Entities;
using ForgiveMeCalia.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace ForgiveMeCalia.Application.Downloads.Commands.DownloadAudio;

public sealed class DownloadAudioCommandHandler(
    IAudioCatalogService catalog,
    ICookieSessionService cookieSession,
    IDownloadIndexStore indexStore,
    IFileDownloadService fileDownloader,
    DownloadPlanner planner,
    IProgressReporter progress,
    IOptions<DownloaderOptions> options) : IRequestHandler<DownloadAudioCommand, DownloadSummary>
{
    public async Task<DownloadSummary> Handle(DownloadAudioCommand request, CancellationToken cancellationToken)
    {
        if (request.Scope.HasFlag(DownloadScope.Paid))
        {
            progress.ReportPhase(AppText.T("download.checkCookies"));
            await cookieSession.EnsureSessionAsync(tryImportIfMissing: true, cancellationToken);
        }

        var posts = await DiscoverPostsAsync(request.Scope, cancellationToken);
        await PruneStaleIndexEntriesAsync(posts, cancellationToken);

        var locked = posts.Count(p => p.IsLocked || p.Mp3Url is null);
        var plan = planner.BuildPlan(posts);
        var alreadyOnDisk = posts.Count - plan.Count - locked;

        progress.ReportPhase(
            AppText.T("download.queue", plan.Count, alreadyOnDisk, locked));

        var downloaded = 0;
        var skipped = posts.Count - plan.Count - locked;
        var failures = new ConcurrentBag<DownloadFailure>();
        var folderTags = new ConcurrentDictionary<string, ConcurrentDictionary<string, byte>>(StringComparer.OrdinalIgnoreCase);

        if (plan.Count > 0)
            progress.ReportPhase(AppText.T("download.downloading", plan.Count, options.Value.MaxParallelDownloads));

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, options.Value.MaxParallelDownloads),
            CancellationToken = cancellationToken
        };

        var completed = 0;
        await Parallel.ForEachAsync(plan, parallelOptions, async (item, token) =>
        {
            var index = Interlocked.Increment(ref completed);
            try
            {
                Directory.CreateDirectory(item.DestinationDirectory);
                var destinationPath = Path.Combine(item.DestinationDirectory, item.DestinationFileName);

                if (File.Exists(destinationPath))
                {
                    await indexStore.MarkCompletedAsync(item.Post.Mp3Url!, token);
                    progress.ReportSkipped(AppText.T("download.fileExists"), item.Post.Title);
                    Interlocked.Increment(ref skipped);
                    return;
                }

                var fileProgress = new Progress<double>(value =>
                    progress.ReportDownload(index, plan.Count, item.DestinationFileName, value));

                await fileDownloader.DownloadAsync(new Uri(item.Post.Mp3Url!), destinationPath, fileProgress, token);
                await indexStore.MarkCompletedAsync(item.Post.Mp3Url!, token);

                var tagBag = folderTags.GetOrAdd(
                    item.DestinationDirectory,
                    _ => new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase));
                foreach (var tag in item.Post.Tags)
                    tagBag.TryAdd(tag, 0);

                Interlocked.Increment(ref downloaded);
            }
            catch (Exception ex)
            {
                var failure = new DownloadFailure(item.Post.Title, DownloadExceptionFormatter.Format(ex));
                failures.Add(failure);
                progress.ReportError(AppText.T("download.failed", item.Post.Title), ex);
            }
        });

        await WriteTagsFilesAsync(folderTags, cancellationToken);

        var summary = DownloadSummary.Create(posts.Count, downloaded, skipped, locked, failures.ToList());
        progress.ReportSummary(summary);
        return summary;
    }

    private async Task<IReadOnlyList<AudioPost>> DiscoverPostsAsync(DownloadScope scope, CancellationToken cancellationToken)
    {
        var tiers = new List<AudioTier>();
        if (scope.HasFlag(DownloadScope.Free))
            tiers.Add(AudioTier.Free);
        if (scope.HasFlag(DownloadScope.Paid))
            tiers.Add(AudioTier.Paid);

        var posts = new List<AudioPost>();
        foreach (var tier in tiers)
        {
            progress.ReportPhase(tier == AudioTier.Free
                ? AppText.T("download.scanningFree")
                : AppText.T("download.scanningPaid"));

            var urls = await catalog.GetPostUrlsAsync(tier, cancellationToken);
            var index = 0;
            foreach (var url in urls)
            {
                index++;
                try
                {
                    var post = await catalog.GetPostDetailsAsync(url, tier, cancellationToken);
                    posts.Add(post);
                    progress.ReportDiscovery(index, urls.Count, post.Title);
                }
                catch (Exception ex)
                {
                    progress.ReportWarning(
                        AppText.T("download.readFailed", index, urls.Count, DownloadExceptionFormatter.Format(ex)));
                }

                if (options.Value.CatalogRequestDelayMs > 0 && index < urls.Count)
                    await Task.Delay(options.Value.CatalogRequestDelayMs, cancellationToken);
            }
        }

        return posts;
    }

    private async Task PruneStaleIndexEntriesAsync(IReadOnlyList<AudioPost> posts, CancellationToken cancellationToken)
    {
        var indexedUrls = await indexStore.LoadCompletedSourceUrlsAsync(cancellationToken);
        if (indexedUrls.Count == 0)
            return;

        var staleUrls = new List<string>();
        foreach (var post in posts)
        {
            if (post.Mp3Url is null || post.IsLocked || !indexedUrls.Contains(post.Mp3Url))
                continue;

            if (!planner.DestinationExists(post))
                staleUrls.Add(post.Mp3Url);
        }

        if (staleUrls.Count > 0)
            await indexStore.RemoveCompletedAsync(staleUrls, cancellationToken);
    }

    private static async Task WriteTagsFilesAsync(
        ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> folderTags,
        CancellationToken cancellationToken)
    {
        foreach (var (folder, tags) in folderTags)
        {
            var tagsPath = Path.Combine(folder, "tags.txt");
            var existing = File.Exists(tagsPath)
                ? (await File.ReadAllLinesAsync(tagsPath, cancellationToken)).ToHashSet(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var tag in tags.Keys)
                existing.Add(tag);

            var lines = existing.OrderBy(t => t, StringComparer.OrdinalIgnoreCase).ToArray();
            await File.WriteAllLinesAsync(tagsPath, lines, cancellationToken);
        }
    }
}
