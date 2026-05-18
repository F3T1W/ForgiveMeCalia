using ForgiveMeCalia.Domain.Enums;
using Spectre.Console;

namespace ForgiveMeCalia.Cli;

internal enum MenuChoice
{
    DownloadFree,
    DownloadPaid,
    DownloadAll,
    ShowPaths,
    ImportCookies,
    LoginHelp,
    ConfigureParallel,
    Exit
}

internal static class InteractiveMenuRunner
{
    public static async Task RunAsync()
    {
        var parallelCount = 4;

        AnsiConsole.Write(
            new FigletText("ForgiveMeCalia")
                .Color(Color.MediumPurple1));
        AnsiConsole.MarkupLine("[grey]Загрузчик аудио mistresscalia.com[/]");
        AnsiConsole.WriteLine();

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuChoice>()
                    .Title("[cyan]Выберите действие:[/]")
                    .PageSize(10)
                    .AddChoices(Enum.GetValues<MenuChoice>())
                    .UseConverter(DescribeChoice));

            AnsiConsole.WriteLine();

            switch (choice)
            {
                case MenuChoice.DownloadFree:
                    await AppActions.RunDownloadAsync(DownloadScope.Free, parallelCount);
                    break;
                case MenuChoice.DownloadPaid:
                    await AppActions.RunDownloadAsync(DownloadScope.Paid, parallelCount);
                    break;
                case MenuChoice.DownloadAll:
                    await AppActions.RunDownloadAsync(DownloadScope.All, parallelCount);
                    break;
                case MenuChoice.ShowPaths:
                    AppActions.ShowPaths();
                    break;
                case MenuChoice.ImportCookies:
                    await RunCookieImportAsync();
                    break;
                case MenuChoice.LoginHelp:
                    AppActions.ShowLoginHelp();
                    break;
                case MenuChoice.ConfigureParallel:
                    parallelCount = AnsiConsole.Prompt(
                        new TextPrompt<int>("[cyan]Параллельных загрузок[/] [grey](1–16)[/]")
                            .DefaultValue(parallelCount)
                            .Validate(n => n is >= 1 and <= 16
                                ? ValidationResult.Success()
                                : ValidationResult.Error("Введите число от 1 до 16.")));
                    AnsiConsole.MarkupLine($"[green]Установлено:[/] {parallelCount}");
                    break;
                case MenuChoice.Exit:
                    AnsiConsole.MarkupLine("[grey]До встречи.[/]");
                    return;
            }

            AnsiConsole.WriteLine();
            if (!AnsiConsole.Confirm("[cyan]Вернуться в меню?[/]", true))
            {
                AnsiConsole.MarkupLine("[grey]До встречи.[/]");
                return;
            }

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold]ForgiveMeCalia[/] [grey]— главное меню[/]");
            AnsiConsole.WriteLine();
        }
    }

    private static async Task RunCookieImportAsync()
    {
        var pickBrowser = AnsiConsole.Confirm(
            "[cyan]Указать браузер вручную?[/] [grey](иначе перебор safari → chrome → firefox)[/]",
            false);

        if (!pickBrowser)
        {
            await AppActions.ImportCookiesAsync();
            return;
        }

        var browser = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[cyan]Браузер[/]")
                .AddChoices("safari", "chrome", "chromium", "brave", "firefox", "edge"));

        await AppActions.ImportCookiesAsync(browser);
    }

    private static string DescribeChoice(MenuChoice choice) => choice switch
    {
        MenuChoice.DownloadFree => "Скачать бесплатные файлы",
        MenuChoice.DownloadPaid => "Скачать платные файлы (нужны cookies)",
        MenuChoice.DownloadAll => "Скачать всё (бесплатные + платные)",
        MenuChoice.ShowPaths => "Показать пути (Музыка и cookies)",
        MenuChoice.ImportCookies => "Импорт cookies из браузера (yt-dlp + brew)",
        MenuChoice.LoginHelp => "Справка: вход Patreon и права macOS",
        MenuChoice.ConfigureParallel => "Настроить параллельность загрузок",
        MenuChoice.Exit => "Выход",
        _ => choice.ToString()
    };
}
