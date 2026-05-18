namespace ForgiveMeCalia.Infrastructure.Http;

internal static class HttpRequestRetry
{
    private static readonly TimeSpan[] RetryDelays =
    [
        TimeSpan.FromSeconds(1),
        TimeSpan.FromSeconds(2),
        TimeSpan.FromSeconds(4),
        TimeSpan.FromSeconds(8)
    ];

    public static async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> action,
        CancellationToken cancellationToken)
    {
        Exception? last = null;

        for (var attempt = 0; attempt <= RetryDelays.Length; attempt++)
        {
            try
            {
                return await action(cancellationToken);
            }
            catch (Exception ex) when (attempt < RetryDelays.Length && IsTransient(ex) && !cancellationToken.IsCancellationRequested)
            {
                last = ex;
                await Task.Delay(RetryDelays[attempt], cancellationToken);
            }
        }

        throw last ?? new InvalidOperationException("HTTP retry failed without exception.");
    }

    public static Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken) =>
        ExecuteAsync(async ct =>
        {
            await action(ct);
            return true;
        }, cancellationToken);

    private static bool IsTransient(Exception exception)
    {
        for (var ex = exception; ex is not null; ex = ex.InnerException)
        {
            if (ex is HttpRequestException or IOException or System.Security.Authentication.AuthenticationException)
                return true;

            var typeName = ex.GetType().FullName;
            if (typeName?.Contains("SslException", StringComparison.Ordinal) == true)
                return true;
        }

        return false;
    }
}
