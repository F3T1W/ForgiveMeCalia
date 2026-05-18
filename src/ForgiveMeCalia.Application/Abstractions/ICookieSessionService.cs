namespace ForgiveMeCalia.Application.Abstractions;

public interface ICookieSessionService
{
    Task EnsureSessionAsync(bool tryImportIfMissing, CancellationToken cancellationToken);
}
