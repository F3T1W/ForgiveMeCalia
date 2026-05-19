namespace ForgiveMeCalia.Application.Options;

public sealed class DownloaderOptions
{
    public static string BaseUrl => "https://mistresscalia.com";
    public static string FreeCategoryPath => "/category/audio/free-files/";
    public static string PaidCategoryPath => "/category/audio/paid-files/";
    public int MaxParallelDownloads { get; set; } = 4;
    public static int CatalogRequestDelayMs => 300;
    public static string LibraryFolderName => "MistressCalia";
}
