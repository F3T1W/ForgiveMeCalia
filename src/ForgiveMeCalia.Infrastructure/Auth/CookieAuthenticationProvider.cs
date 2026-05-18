using ForgiveMeCalia.Application.Abstractions;

namespace ForgiveMeCalia.Infrastructure.Auth;

public sealed class CookieAuthenticationProvider(ICookieSessionService sessions) : IAuthenticationProvider
{
    public bool HasSession { get; private set; }

    public async Task EnsureSessionAsync(CancellationToken cancellationToken)
    {
        await sessions.EnsureSessionAsync(tryImportIfMissing: true, cancellationToken);
        HasSession = true;
    }
}
