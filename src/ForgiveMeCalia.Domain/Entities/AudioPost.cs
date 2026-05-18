using ForgiveMeCalia.Domain.Enums;

namespace ForgiveMeCalia.Domain.Entities;

public sealed class AudioPost
{
    public required string PostUrl { get; init; }
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public required AudioTier Tier { get; init; }
    public string? Mp3Url { get; init; }
    public bool IsLocked { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public string? SeriesKey { get; init; }
    public int? SeriesPartNumber { get; init; }
}
