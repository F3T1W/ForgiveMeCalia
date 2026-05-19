using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Options;

namespace ForgiveMeCalia.Infrastructure.Paths;

public sealed class LibraryPathProvider : ILibraryPathProvider
{
    public string GetLibraryRoot()
    {
        var music = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        if (string.IsNullOrWhiteSpace(music))
            music = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Music");

        return Path.Combine(music, DownloaderOptions.LibraryFolderName);
    }

    public string GetTierRoot(string tierFolderName) =>
        Path.Combine(GetLibraryRoot(), tierFolderName);

    public string GetCustomRoot() =>
        Path.Combine(GetLibraryRoot(), "Custom");

    public string GetCookieFilePath()
    {
        var configRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        if (string.IsNullOrWhiteSpace(configRoot))
            configRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");

        return Path.Combine(configRoot, "ForgiveMeCalia", "cookies.txt");
    }
}
