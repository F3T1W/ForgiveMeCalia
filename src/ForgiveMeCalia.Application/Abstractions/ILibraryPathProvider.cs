namespace ForgiveMeCalia.Application.Abstractions;

public interface ILibraryPathProvider
{
    string GetLibraryRoot();
    string GetTierRoot(string tierFolderName);
    string GetCustomRoot();
    string GetCookieFilePath();
}
