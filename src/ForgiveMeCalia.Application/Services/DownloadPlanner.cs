using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Localization;
using ForgiveMeCalia.Domain.Entities;
using ForgiveMeCalia.Domain.Enums;
using ForgiveMeCalia.Domain.Services;

namespace ForgiveMeCalia.Application.Services;

public sealed class DownloadPlanner(ILibraryPathProvider libraryPaths)
{
    public IReadOnlyList<DownloadPlanItem> BuildPlan(IReadOnlyList<AudioPost> posts)
    {
        var tierGroups = posts.GroupBy(p => p.Tier);
        var plan = new List<DownloadPlanItem>();

        foreach (var tierGroup in tierGroups)
        {
            var tierFolder = GetTierFolderName(tierGroup.Key);
            var tierRoot = libraryPaths.GetTierRoot(tierFolder);
            var seriesAssignments = AssignSeriesFolders([.. tierGroup]);

            foreach (var post in tierGroup)
            {
                if (post.Mp3Url is null || post.IsLocked)
                    continue;

                var (destinationDirectory, fileName) = GetDestination(post, tierRoot, seriesAssignments);
                if (DestinationExists(post, seriesAssignments)
                    || File.Exists(Path.Combine(destinationDirectory, fileName)))
                    continue;

                plan.Add(new DownloadPlanItem
                {
                    Post = post,
                    DestinationDirectory = destinationDirectory,
                    DestinationFileName = fileName,
                    Tier = post.Tier
                });
            }
        }

        return [.. plan
            .OrderBy(p => p.Tier)
            .ThenBy(p => p.DestinationDirectory, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Post.SeriesPartNumber ?? 0)];
    }

    private static Dictionary<AudioPost, string> AssignSeriesFolders(IReadOnlyList<AudioPost> posts)
    {
        var result = new Dictionary<AudioPost, string>();
        var seriesGroups = posts
            .Where(p => p.SeriesKey is not null)
            .GroupBy(p => p.SeriesKey!, StringComparer.OrdinalIgnoreCase);

        foreach (var group in seriesGroups)
        {
            var folderName = group
                .Select(p => SeriesNameParser.Parse(p.Title, p.Slug).FolderName)
                .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
                .First();

            foreach (var post in group)
                result[post] = folderName;
        }

        foreach (var post in posts.Where(p => p.SeriesKey is null))
            result[post] = SeriesNameParser.Parse(post.Title, post.Slug).FolderName;

        return result;
    }

    private (string Directory, string FileName) GetDestination(
        AudioPost post,
        string? tierRoot = null,
        Dictionary<AudioPost, string>? seriesAssignments = null)
    {
        tierRoot ??= libraryPaths.GetTierRoot(GetTierFolderName(post.Tier));

        seriesAssignments ??= AssignSeriesFolders([post]);
        var folderName = seriesAssignments[post];
        return (Path.Combine(tierRoot, folderName), BuildFileName(post));
    }

    public bool DestinationExists(
        AudioPost post,
        Dictionary<AudioPost, string>? seriesAssignments = null)
    {
        if (post.Mp3Url is null || post.IsLocked)
            return false;

        seriesAssignments ??= AssignSeriesFolders([post]);

        var roots = GetCompatibleTierFolderNames(post.Tier);

        return roots
            .Select(root => GetDestination(post, libraryPaths.GetTierRoot(root), seriesAssignments))
            .Any(destination => File.Exists(Path.Combine(destination.Directory, destination.FileName)));
    }

    private static string GetTierFolderName(AudioTier tier) => tier switch
    {
        AudioTier.Free => "Free",
        AudioTier.Paid => "Paid",
        _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
    };

    private static string[] GetCompatibleTierFolderNames(AudioTier tier) => tier switch
    {
        AudioTier.Free => ["Free", "free"],
        AudioTier.Paid => ["Paid", "paid"],
        _ => throw new ArgumentOutOfRangeException(nameof(tier), tier, null)
    };

    private static string BuildFileName(AudioPost post)
    {
        if (post.Mp3Url is null)
            throw new InvalidOperationException(AppText.T("errors.mp3Required"));

        var extension = Path.GetExtension(new Uri(post.Mp3Url).AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".mp3";

        var baseName = SeriesNameParser.SanitizeFolderName(post.Title);
        if (post.SeriesPartNumber is { } part)
            return $"{baseName} (Part {part}){extension}";

        return $"{baseName}{extension}";
    }
}
