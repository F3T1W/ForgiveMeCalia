using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Domain.Services;

namespace ForgiveMeCalia.Infrastructure.Audio;

public sealed class CustomAudioService(ILibraryPathProvider libraryPaths) : ICustomAudioService
{
    public async Task<string> CreateAsync(
        string inductionPath,
        string mainHypnosisPath,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(inductionPath))
            throw new FileNotFoundException("Induction file was not found.", inductionPath);
        if (!File.Exists(mainHypnosisPath))
            throw new FileNotFoundException("Main hypnosis file was not found.", mainHypnosisPath);

        var customRoot = libraryPaths.GetCustomRoot();
        Directory.CreateDirectory(customRoot);

        var outputPath = GetAvailableOutputPath(customRoot, inductionPath, mainHypnosisPath);
        await using var output = new FileStream(
            outputPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);

        await CopyMp3PayloadAsync(inductionPath, output, isFirstFile: true, cancellationToken);
        await CopyMp3PayloadAsync(mainHypnosisPath, output, isFirstFile: false, cancellationToken);

        return outputPath;
    }

    private static string GetAvailableOutputPath(string customRoot, string inductionPath, string mainHypnosisPath)
    {
        var inductionName = Path.GetFileNameWithoutExtension(inductionPath);
        var mainName = Path.GetFileNameWithoutExtension(mainHypnosisPath);
        var baseName = SeriesNameParser.SanitizeFolderName($"{inductionName} + {mainName}");
        var outputPath = Path.Combine(customRoot, $"{baseName}.mp3");

        var index = 1;
        while (File.Exists(outputPath))
        {
            outputPath = Path.Combine(customRoot, $"{baseName} ({index}).mp3");
            index++;
        }

        return outputPath;
    }

    private static async Task CopyMp3PayloadAsync(
        string sourcePath,
        Stream destination,
        bool isFirstFile,
        CancellationToken cancellationToken)
    {
        await using var source = new FileStream(
            sourcePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        var startOffset = isFirstFile ? 0 : GetAudioStartOffset(source);
        var endOffset = GetAudioEndOffset(source);

        if (endOffset <= startOffset)
            return;

        source.Position = startOffset;
        var remaining = endOffset - startOffset;
        var buffer = new byte[81920];
        while (remaining > 0)
        {
            var readLength = (int)Math.Min(buffer.Length, remaining);
            var bytesRead = await source.ReadAsync(buffer.AsMemory(0, readLength), cancellationToken);
            if (bytesRead == 0)
                break;

            await destination.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
            remaining -= bytesRead;
        }
    }

    private static long GetAudioStartOffset(Stream stream)
    {
        if (stream.Length < 10)
            return 0;

        Span<byte> header = stackalloc byte[10];
        stream.Position = 0;
        var bytesRead = stream.Read(header);
        if (bytesRead < header.Length || header[0] != 'I' || header[1] != 'D' || header[2] != '3')
            return 0;

        var tagSize =
            ((header[6] & 0x7F) << 21)
            | ((header[7] & 0x7F) << 14)
            | ((header[8] & 0x7F) << 7)
            | (header[9] & 0x7F);

        return Math.Min(stream.Length, 10L + tagSize);
    }

    private static long GetAudioEndOffset(Stream stream)
    {
        if (stream.Length < 128)
            return stream.Length;

        Span<byte> footer = stackalloc byte[3];
        stream.Position = stream.Length - 128;
        var bytesRead = stream.Read(footer);

        return bytesRead == footer.Length && footer[0] == 'T' && footer[1] == 'A' && footer[2] == 'G'
            ? stream.Length - 128
            : stream.Length;
    }
}
