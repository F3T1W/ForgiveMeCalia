namespace ForgiveMeCalia.Application.Options;

public sealed class DownloaderOptions
{
    public const string SectionName = "Downloader";

    public string BaseUrl { get; set; } = "https://mistresscalia.com";
    public string FreeCategoryPath { get; set; } = "/category/audio/free-files/";
    public string PaidCategoryPath { get; set; } = "/category/audio/paid-files/";
    public int MaxParallelDownloads { get; set; } = 4;
    public int CatalogRequestDelayMs { get; set; } = 300;
    public string LibraryFolderName { get; set; } = "MistressCalia";
}
