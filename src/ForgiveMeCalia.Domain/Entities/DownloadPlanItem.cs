using ForgiveMeCalia.Domain.Enums;

namespace ForgiveMeCalia.Domain.Entities;

public sealed class DownloadPlanItem
{
    public required AudioPost Post { get; init; }
    public required string DestinationDirectory { get; init; }
    public required string DestinationFileName { get; init; }
    public required AudioTier Tier { get; init; }
}
