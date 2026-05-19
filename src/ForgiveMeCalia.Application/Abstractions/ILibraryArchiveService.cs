namespace ForgiveMeCalia.Application.Abstractions;

public interface ILibraryArchiveService
{
    Task<string> CreateArchiveAsync(string? password, CancellationToken cancellationToken);
}
