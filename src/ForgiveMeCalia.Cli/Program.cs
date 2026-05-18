using ForgiveMeCalia.Cli;
using ForgiveMeCalia.Domain.Enums;
using Spectre.Console;
using System.CommandLine;

if (ShouldRunInteractiveMenu(args))
{
    await InteractiveMenuRunner.RunAsync();
    return;
}

var root = new RootCommand("ForgiveMeCalia - audio downloader for mistresscalia.com");

var download = new Command("download", "Download audio into the Music folder");
var free = new Option<bool>("--free", "Free files only");
var paid = new Option<bool>("--paid", "Paid files only (Patreon cookies required)");
var all = new Option<bool>("--all", "Free and paid files");
var parallel = new Option<int>("--parallel", () => 4, "Number of parallel downloads");

download.AddOption(free);
download.AddOption(paid);
download.AddOption(all);
download.AddOption(parallel);
download.SetHandler(async (bool isFree, bool isPaid, bool isAll, int parallelCount) =>
{
    var scope = AppActions.ResolveScope(isFree, isPaid, isAll);
    if (scope == DownloadScope.None)
    {
        AnsiConsole.MarkupLine("[red]Specify --free, --paid, or --all[/]");
        return;
    }

    await AppActions.RunDownloadAsync(scope, parallelCount);
}, free, paid, all, parallel);

var catalog = new Command("catalog", "Inspect the remote catalog");
var catalogCount = new Command("count", "Count category posts without downloading");
catalogCount.AddOption(free);
catalogCount.AddOption(paid);
catalogCount.AddOption(all);
catalogCount.SetHandler(async (bool isFree, bool isPaid, bool isAll) =>
{
    var scope = AppActions.ResolveScope(isFree, isPaid, isAll);
    if (scope == DownloadScope.None)
    {
        AnsiConsole.MarkupLine("[red]Specify --free, --paid, or --all[/]");
        return;
    }

    await AppActions.CountCatalogAsync(scope);
}, free, paid, all);
catalog.AddCommand(catalogCount);

var paths = new Command("paths", "Show music and cookie paths");
paths.SetHandler(AppActions.ShowPaths);

var loginHelp = new Command("login-help", "Explain browser cookie import");
loginHelp.SetHandler(AppActions.ShowLoginHelp);

var cookies = new Command("cookies", "Manage Patreon cookies");
var cookiesImport = new Command("import", "Import browser cookies through yt-dlp");
var browserOption = new Option<string?>("--browser", "Browser name supported by yt-dlp");
cookiesImport.AddOption(browserOption);
cookiesImport.SetHandler(async (string? browser) => await AppActions.ImportCookiesAsync(browser), browserOption);
cookies.AddCommand(cookiesImport);

var menu = new Command("menu", "Interactive menu");
menu.SetHandler(async () => await InteractiveMenuRunner.RunAsync());

root.AddCommand(download);
root.AddCommand(catalog);
root.AddCommand(paths);
root.AddCommand(loginHelp);
root.AddCommand(cookies);
root.AddCommand(menu);

await root.InvokeAsync(args);

static bool ShouldRunInteractiveMenu(string[] arguments)
{
    if (arguments.Length == 0)
        return true;

    if (arguments is ["menu"] or ["-m"] or ["--menu"])
        return true;

    return false;
}
