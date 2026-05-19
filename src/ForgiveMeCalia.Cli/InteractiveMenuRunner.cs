using ForgiveMeCalia.Application.Localization;
using ForgiveMeCalia.Domain.Enums;
using Spectre.Console;

namespace ForgiveMeCalia.Cli;

internal enum MenuChoice
{
    DownloadFree,
    DownloadPaid,
    DownloadAll,
    CreateCustomAudio,
    CreateLibraryArchive,
    ShowPaths,
    ImportCookies,
    ConfigureParallel,
    ChangeLanguage,
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
        AnsiConsole.MarkupLine($"[grey]{Markup.Escape(AppText.T("app.subtitle"))}[/]");
        AnsiConsole.WriteLine();

        while (true)
        {
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<MenuChoice>()
                    .Title($"[cyan]{Markup.Escape(AppText.T("menu.title"))}[/]")
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
                case MenuChoice.CreateCustomAudio:
                    await AppActions.CreateCustomAudioAsync();
                    break;
                case MenuChoice.CreateLibraryArchive:
                    await RunArchiveCreationAsync();
                    break;
                case MenuChoice.ShowPaths:
                    AppActions.ShowPaths();
                    break;
                case MenuChoice.ImportCookies:
                    await RunCookieImportAsync();
                    break;
                case MenuChoice.ConfigureParallel:
                    parallelCount = AnsiConsole.Prompt(
                        new TextPrompt<int>($"[cyan]{Markup.Escape(AppText.T("menu.parallelPrompt"))}[/] [grey](1-16)[/]")
                            .DefaultValue(parallelCount)
                            .Validate(n => n is >= 1 and <= 16
                                ? ValidationResult.Success()
                                : ValidationResult.Error(AppText.T("menu.parallelError"))));
                    AnsiConsole.MarkupLine($"[green]{Markup.Escape(AppText.T("menu.set", parallelCount))}[/]");
                    break;
                case MenuChoice.ChangeLanguage:
                    ChangeLanguage();
                    break;
                case MenuChoice.Exit:
                    AnsiConsole.MarkupLine($"[grey]{Markup.Escape(AppText.T("menu.goodbye"))}[/]");
                    return;
            }

            AnsiConsole.WriteLine();
            if (!AnsiConsole.Confirm($"[cyan]{Markup.Escape(AppText.T("menu.return"))}[/]", true))
            {
                AnsiConsole.MarkupLine($"[grey]{Markup.Escape(AppText.T("menu.goodbye"))}[/]");
                return;
            }

            AnsiConsole.Clear();
            AnsiConsole.MarkupLine($"[bold]ForgiveMeCalia[/] [grey]- {Markup.Escape(AppText.T("menu.main"))}[/]");
            AnsiConsole.WriteLine();
        }
    }

    private static async Task RunCookieImportAsync()
    {
        var pickBrowser = AnsiConsole.Confirm(
            $"[cyan]{Markup.Escape(AppText.T("menu.browserManual"))}[/] [grey]({Markup.Escape(AppText.T("menu.browserAutoHint"))})[/]",
            false);

        if (!pickBrowser)
        {
            await AppActions.ImportCookiesAsync();
            return;
        }

        var browsers = AppActions.GetSupportedBrowsers();
        var browser = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"[cyan]{Markup.Escape(AppText.T("menu.browser"))}[/]")
                .AddChoices(browsers));

        await AppActions.ImportCookiesAsync(browser);
    }

    private static async Task RunArchiveCreationAsync()
    {
        var usePassword = AnsiConsole.Confirm(
            $"[cyan]{Markup.Escape(AppText.T("archive.usePassword"))}[/]",
            false);

        string? password = null;
        if (usePassword)
        {
            password = AnsiConsole.Prompt(
                new TextPrompt<string>($"[cyan]{Markup.Escape(AppText.T("archive.password"))}[/]")
                    .Secret()
                    .AllowEmpty());
        }

        await AppActions.CreateLibraryArchiveAsync(password);
    }

    private static void ChangeLanguage()
    {
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<AppLanguage>()
                .Title($"[cyan]{Markup.Escape(AppText.T("menu.language"))}[/]")
                .AddChoices(AppText.SupportedLanguages)
                .UseConverter(AppText.LanguageName));

        AppText.CurrentLanguage = selected;
        AnsiConsole.MarkupLine($"[green]{Markup.Escape(AppText.T("menu.languageSet", AppText.LanguageName(selected)))}[/]");
    }

    private static string DescribeChoice(MenuChoice choice) => choice switch
    {
        MenuChoice.DownloadFree => AppText.T("menu.downloadFree"),
        MenuChoice.DownloadPaid => AppText.T("menu.downloadPaid"),
        MenuChoice.DownloadAll => AppText.T("menu.downloadAll"),
        MenuChoice.CreateCustomAudio => AppText.T("menu.createCustomAudio"),
        MenuChoice.CreateLibraryArchive => AppText.T("menu.createLibraryArchive"),
        MenuChoice.ShowPaths => AppText.T("menu.showPaths"),
        MenuChoice.ImportCookies => AppText.T("menu.importCookies"),
        MenuChoice.ConfigureParallel => AppText.T("menu.configureParallel"),
        MenuChoice.ChangeLanguage => AppText.T("menu.changeLanguage"),
        MenuChoice.Exit => AppText.T("menu.exit"),
        _ => choice.ToString()
    };
}
