namespace ForgiveMeCalia.Domain.Enums;

[Flags]
public enum DownloadScope
{
    None = 0,
    Free = 1,
    Paid = 2,
    All = Free | Paid
}
