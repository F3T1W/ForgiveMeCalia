using ForgiveMeCalia.Cli;
using ForgiveMeCalia.Domain.Enums;
using Spectre.Console;
using System.CommandLine;

if (ShouldRunInteractiveMenu(args))
{
    await InteractiveMenuRunner.RunAsync();
    return;
}

var root = new RootCommand("ForgiveMeCalia — загрузчик аудио с mistresscalia.com");

var download = new Command("download", "Скачать аудио в папку «Музыка»");
var free = new Option<bool>("--free", "Только бесплатные файлы");
var paid = new Option<bool>("--paid", "Только платные (нужны cookies Patreon)");
var all = new Option<bool>("--all", "Бесплатные и платные");
var parallel = new Option<int>("--parallel", () => 4, "Число параллельных загрузок");

download.AddOption(free);
download.AddOption(paid);
download.AddOption(all);
download.AddOption(parallel);
download.SetHandler(async (bool isFree, bool isPaid, bool isAll, int parallelCount) =>
{
    var scope = AppActions.ResolveScope(isFree, isPaid, isAll);
    if (scope == DownloadScope.None)
    {
        AnsiConsole.MarkupLine("[red]Укажите --free, --paid или --all[/]");
        return;
    }

    await AppActions.RunDownloadAsync(scope, parallelCount);
}, free, paid, all, parallel);

var catalog = new Command("catalog", "Просмотр каталога на сайте");
var catalogCount = new Command("count", "Посчитать записи в категориях (без загрузки)");
catalogCount.AddOption(free);
catalogCount.AddOption(paid);
catalogCount.AddOption(all);
catalogCount.SetHandler(async (bool isFree, bool isPaid, bool isAll) =>
{
    var scope = AppActions.ResolveScope(isFree, isPaid, isAll);
    if (scope == DownloadScope.None)
    {
        AnsiConsole.MarkupLine("[red]Укажите --free, --paid или --all[/]");
        return;
    }

    await AppActions.CountCatalogAsync(scope);
}, free, paid, all);
catalog.AddCommand(catalogCount);

var paths = new Command("paths", "Показать пути к музыке и cookies");
paths.SetHandler(AppActions.ShowPaths);

var loginHelp = new Command("login-help", "Как экспортировать cookies без GUI в программе");
loginHelp.SetHandler(AppActions.ShowLoginHelp);

var cookies = new Command("cookies", "Работа с cookies Patreon");
var cookiesImport = new Command("import", "Импорт cookies из браузера через yt-dlp");
var browserOption = new Option<string?>("--browser", "safari, chrome, firefox, edge, brave, chromium");
cookiesImport.AddOption(browserOption);
cookiesImport.SetHandler(async (string? browser) => await AppActions.ImportCookiesAsync(browser), browserOption);
cookies.AddCommand(cookiesImport);

var menu = new Command("menu", "Интерактивное меню");
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
