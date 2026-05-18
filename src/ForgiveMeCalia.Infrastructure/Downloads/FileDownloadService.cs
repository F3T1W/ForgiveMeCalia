using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Infrastructure.Http;

namespace ForgiveMeCalia.Infrastructure.Downloads;

public sealed class FileDownloadService(MistressCaliaSiteClient siteClient) : IFileDownloadService
{
    public Task DownloadAsync(
        Uri source,
        string destinationPath,
        IProgress<double>? progress,
        CancellationToken cancellationToken) =>
        siteClient.DownloadToFileAsync(source, destinationPath, progress, cancellationToken);
}
