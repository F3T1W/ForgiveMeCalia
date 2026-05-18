namespace ForgiveMeCalia.Application.Abstractions;

public interface IFileDownloadService
{
    Task DownloadAsync(
        Uri source,
        string destinationPath,
        IProgress<double>? progress,
        CancellationToken cancellationToken);
}
