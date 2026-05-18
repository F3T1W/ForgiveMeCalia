using ForgiveMeCalia.Application.Abstractions;
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
            var tierFolder = tierGroup.Key switch
            {
                AudioTier.Free => "free",
                AudioTier.Paid => "paid",
                _ => throw new ArgumentOutOfRangeException()
            };

            var tierRoot = libraryPaths.GetTierRoot(tierFolder);
            var seriesAssignments = AssignSeriesFolders(tierGroup.ToList());

            foreach (var post in tierGroup)
            {
                if (post.Mp3Url is null || post.IsLocked)
                    continue;

                var (destinationDirectory, fileName) = GetDestination(post, tierRoot, seriesAssignments);
                if (File.Exists(Path.Combine(destinationDirectory, fileName)))
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

        return plan
            .OrderBy(p => p.Tier)
            .ThenBy(p => p.DestinationDirectory, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Post.SeriesPartNumber ?? 0)
            .ToList();
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

    public (string Directory, string FileName) GetDestination(
        AudioPost post,
        string? tierRoot = null,
        Dictionary<AudioPost, string>? seriesAssignments = null)
    {
        tierRoot ??= libraryPaths.GetTierRoot(post.Tier switch
        {
            AudioTier.Free => "free",
            AudioTier.Paid => "paid",
            _ => throw new ArgumentOutOfRangeException()
        });

        seriesAssignments ??= AssignSeriesFolders([post]);
        var folderName = seriesAssignments[post];
        return (Path.Combine(tierRoot, folderName), BuildFileName(post));
    }

    private static string BuildFileName(AudioPost post)
    {
        if (post.Mp3Url is null)
            throw new InvalidOperationException("MP3 URL is required.");

        var extension = Path.GetExtension(new Uri(post.Mp3Url).AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension))
            extension = ".mp3";

        var baseName = SeriesNameParser.SanitizeFolderName(post.Title);
        if (post.SeriesPartNumber is int part)
            return $"{baseName} (Part {part}){extension}";

        return $"{baseName}{extension}";
    }
}
