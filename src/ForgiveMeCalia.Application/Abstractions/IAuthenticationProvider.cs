namespace ForgiveMeCalia.Application.Abstractions;

public interface IAuthenticationProvider
{
    bool HasSession { get; }
    Task EnsureSessionAsync(CancellationToken cancellationToken);
}
