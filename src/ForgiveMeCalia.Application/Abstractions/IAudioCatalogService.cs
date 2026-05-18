using ForgiveMeCalia.Domain.Entities;
using ForgiveMeCalia.Domain.Enums;

namespace ForgiveMeCalia.Application.Abstractions;

public interface IAudioCatalogService
{
    Task<IReadOnlyList<string>> GetPostUrlsAsync(AudioTier tier, CancellationToken cancellationToken);
    Task<AudioPost> GetPostDetailsAsync(string postUrl, AudioTier tier, CancellationToken cancellationToken);
}
