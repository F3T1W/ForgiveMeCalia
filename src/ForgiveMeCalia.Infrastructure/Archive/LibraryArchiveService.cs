using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Localization;
using ICSharpCode.SharpZipLib.Zip;

namespace ForgiveMeCalia.Infrastructure.Archive;

public sealed class LibraryArchiveService(ILibraryPathProvider libraryPaths) : ILibraryArchiveService
{
    private static readonly string[] FolderNames = ["Free", "Paid", "Custom"];

    public async Task<string> CreateArchiveAsync(string? password, CancellationToken cancellationToken)
    {
        var libraryRoot = libraryPaths.GetLibraryRoot();
        Directory.CreateDirectory(libraryRoot);

        var entries = FolderNames
            .Select(folderName => new DirectoryInfo(Path.Combine(libraryRoot, folderName)))
            .Where(directory => directory.Exists)
            .SelectMany(GetArchiveEntries)
            .ToList();

        if (entries.Count == 0)
            throw new InvalidOperationException(AppText.T("archive.noContent"));

        var archivePath = GetAvailableArchivePath(libraryRoot);
        await using var fileStream = new FileStream(
            archivePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true);
        await using var zipStream = new ZipOutputStream(fileStream);

        zipStream.SetLevel(9);
        if (!string.IsNullOrWhiteSpace(password))
            zipStream.Password = password;

        foreach (var entry in entries)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var zipEntry = new ZipEntry(entry.ArchiveName)
            {
                DateTime = entry.LastWriteTime,
                Size = entry.IsDirectory ? 0 : entry.FileLength
            };

            if (!string.IsNullOrWhiteSpace(password) && !entry.IsDirectory)
                zipEntry.AESKeySize = 256;

            await zipStream.PutNextEntryAsync(zipEntry, cancellationToken);
            if (!entry.IsDirectory)
                await CopyFileAsync(entry.FullPath, zipStream, cancellationToken);

            await zipStream.CloseEntryAsync(cancellationToken);
        }

        await zipStream.FinishAsync(cancellationToken);
        return archivePath;
    }

    private static IEnumerable<ArchiveEntryInfo> GetArchiveEntries(DirectoryInfo root)
    {
        yield return ArchiveEntryInfo.Directory(root.Name + "/", root.LastWriteTime);

        foreach (var directory in root.EnumerateDirectories("*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(root.Parent!.FullName, directory.FullName);
            yield return ArchiveEntryInfo.Directory(ToZipPath(relativePath) + "/", directory.LastWriteTime);
        }

        foreach (var file in root.EnumerateFiles("*", SearchOption.AllDirectories))
        {
            var relativePath = Path.GetRelativePath(root.Parent!.FullName, file.FullName);
            yield return ArchiveEntryInfo.File(
                file.FullName,
                ToZipPath(relativePath),
                file.LastWriteTime,
                file.Length);
        }
    }

    private static async Task CopyFileAsync(
        string sourcePath,
        Stream destination,
        CancellationToken cancellationToken)
    {
        await using var source = new FileStream(
            sourcePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920,
            useAsync: true);

        await source.CopyToAsync(destination, cancellationToken);
    }

    private static string GetAvailableArchivePath(string libraryRoot)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        var baseName = $"MistressCalia-{timestamp}";
        var archivePath = Path.Combine(libraryRoot, $"{baseName}.zip");

        var index = 1;
        while (File.Exists(archivePath))
        {
            archivePath = Path.Combine(libraryRoot, $"{baseName}-{index}.zip");
            index++;
        }

        return archivePath;
    }

    private static string ToZipPath(string path) =>
        path.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');

    private sealed record ArchiveEntryInfo(
        string FullPath,
        string ArchiveName,
        DateTime LastWriteTime,
        long FileLength,
        bool IsDirectory)
    {
        public static ArchiveEntryInfo Directory(string archiveName, DateTime lastWriteTime) =>
            new(string.Empty, archiveName, lastWriteTime, 0, true);

        public static ArchiveEntryInfo File(
            string fullPath,
            string archiveName,
            DateTime lastWriteTime,
            long fileLength) =>
            new(fullPath, archiveName, lastWriteTime, fileLength, false);
    }
}
