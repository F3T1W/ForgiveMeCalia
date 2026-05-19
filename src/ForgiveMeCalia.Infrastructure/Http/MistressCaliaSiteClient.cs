namespace ForgiveMeCalia.Infrastructure.Http;

public sealed class MistressCaliaSiteClient(HttpClient httpClient)
{
    public Uri CreateSiteUri(string relativeOrAbsolute = "/")
    {
        if (Uri.TryCreate(relativeOrAbsolute, UriKind.Absolute, out var absolute)
            && absolute.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return absolute;

        return new Uri(httpClient.BaseAddress!, relativeOrAbsolute.TrimStart('/'));
    }

    public Task<string> GetStringAsync(string pathOrUrl, CancellationToken cancellationToken) =>
        HttpRequestRetry.ExecuteAsync(ct => GetStringCoreAsync(pathOrUrl, ct), cancellationToken);

    public Task DownloadToFileAsync(
        Uri source,
        string destinationPath,
        IProgress<double>? progress,
        CancellationToken cancellationToken) =>
        HttpRequestRetry.ExecuteAsync(
            ct => DownloadToFileCoreAsync(source, destinationPath, progress, ct),
            cancellationToken);

    private async Task<string> GetStringCoreAsync(string pathOrUrl, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(
            pathOrUrl,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private async Task DownloadToFileCoreAsync(
        Uri source,
        string destinationPath,
        IProgress<double>? progress,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(
            source,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        var total = response.Content.Headers.ContentLength;
        await using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var fileStream = new FileStream(
            destinationPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        var buffer = new byte[81920];
        long read = 0;
        int bytesRead;
        while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            read += bytesRead;
            if (total is > 0)
                progress?.Report((double)read / total.Value);
        }

        progress?.Report(1);
    }
}
