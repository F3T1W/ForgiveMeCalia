using System.Globalization;
using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Downloads;
using ForgiveMeCalia.Application.Downloads.Commands.DownloadAudio;
using ForgiveMeCalia.Application.Localization;
using ForgiveMeCalia.Domain.Enums;
using ForgiveMeCalia.Infrastructure.Auth;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;

namespace ForgiveMeCalia.Cli;

internal static class AppActions
{
    private sealed record AudioFileChoice(string DisplayName, string FullPath);

    public static async Task RunDownloadAsync(DownloadScope scope, int parallelCount)
    {
        if (scope == DownloadScope.None)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(AppText.T("action.noScope"))}[/]");
            return;
        }

        try
        {
            using var host = AppHostFactory.Create(parallelCount);
            var mediator = host.Services.GetRequiredService<IMediator>();
            await mediator.Send(new DownloadAudioCommand(scope));
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(AppText.T("action.interrupted"))}[/]");
            AnsiConsole.MarkupLine(Markup.Escape(DownloadExceptionFormatter.Format(ex)));
            AnsiConsole.MarkupLine(
                $"[grey]{Markup.Escape(AppText.T("action.networkHint"))}[/]");
        }
    }

    public static void ShowPaths()
    {
        using var host = AppHostFactory.Create();
        var library = host.Services.GetRequiredService<ILibraryPathProvider>();
        AnsiConsole.MarkupLine($"[cyan]{Markup.Escape(AppText.T("action.music"))}:[/] {Markup.Escape(library.GetLibraryRoot())}");
        AnsiConsole.MarkupLine($"[cyan]{Markup.Escape(AppText.T("action.cookies"))}:[/] {Markup.Escape(library.GetCookieFilePath())}");
        AnsiConsole.MarkupLine($"[cyan]{Markup.Escape(AppText.T("action.custom"))}:[/] {Markup.Escape(library.GetCustomRoot())}");
    }

    public static async Task CreateCustomAudioAsync()
    {
        using var host = AppHostFactory.Create();
        var library = host.Services.GetRequiredService<ILibraryPathProvider>();
        var customAudio = host.Services.GetRequiredService<ICustomAudioService>();
        var files = FindDownloadedAudioFiles(library)
            .OrderBy(file => file.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (files.Count < 2)
        {
            AnsiConsole.MarkupLine($"[yellow]{Markup.Escape(AppText.T("custom.notEnoughFiles"))}[/]");
            return;
        }

        var induction = PromptAudioFile("custom.selectInduction", files);
        var mainHypnosis = PromptAudioFile("custom.selectMain", files);

        if (string.Equals(induction.FullPath, mainHypnosis.FullPath, StringComparison.OrdinalIgnoreCase)
            && !AnsiConsole.Confirm($"[yellow]{Markup.Escape(AppText.T("custom.sameFileConfirm"))}[/]", false))
        {
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(AppText.T("custom.cancelled"))}[/]");
            return;
        }

        try
        {
            var outputPath = await customAudio.CreateAsync(
                induction.FullPath,
                mainHypnosis.FullPath,
                CancellationToken.None);

            AnsiConsole.MarkupLine($"[green]{Markup.Escape(AppText.T("custom.created", outputPath))}[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(AppText.T("custom.failed", DownloadExceptionFormatter.Format(ex)))}[/]");
        }
    }

    public static async Task CreateLibraryArchiveAsync(string? password)
    {
        using var host = AppHostFactory.Create();
        var archiveService = host.Services.GetRequiredService<ILibraryArchiveService>();
        var passwordProtected = !string.IsNullOrWhiteSpace(password);

        try
        {
            var archivePath = await AnsiConsole.Status()
                .Spinner(Spinner.Known.Dots)
                .StartAsync(AppText.T("archive.creating"), _ =>
                    archiveService.CreateArchiveAsync(password, CancellationToken.None));

            WriteArchiveCreatedPanel(archivePath, passwordProtected);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(AppText.T("archive.failed", DownloadExceptionFormatter.Format(ex)))}[/]");
        }
    }

    public static async Task ImportCookiesAsync(string? browser = null)
    {
        using var host = AppHostFactory.Create();
        var exporter = host.Services.GetRequiredService<IBrowserCookieExporter>();
        var library = host.Services.GetRequiredService<ILibraryPathProvider>();

        AnsiConsole.MarkupLine($"[cyan]{Markup.Escape(AppText.T("action.importingCookies"))}[/]");
        if (browser is null)
            AnsiConsole.MarkupLine($"[grey]{Markup.Escape(AppText.T("action.tryingBrowsers"))}[/]");

        var result = await exporter.ExportAsync(browser);
        if (result.Success)
        {
            AnsiConsole.MarkupLine(
                $"[green]{Markup.Escape(AppText.T("action.doneBrowserFile", result.Browser, result.CookieFilePath))}[/]");
            if (result.YtDlpWasInstalled)
                AnsiConsole.MarkupLine($"[grey]{Markup.Escape(AppText.T("action.ytDlpInstalled"))}[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[red]{Markup.Escape(AppText.T("action.cookieImportFailed"))}[/]");
        if (result.IsPermissionDenied)
        {
            AnsiConsole.Write(new Panel(YtDlpBrowserCookieExporter.GetPermissionHelp())
            {
                Header = new PanelHeader(AppText.T("action.cookieAccessHeader")),
                Border = BoxBorder.Rounded
            });
        }

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            AnsiConsole.MarkupLine(Markup.Escape(result.ErrorMessage));

        AnsiConsole.MarkupLine($"[grey]{Markup.Escape(AppText.T("action.filePath"))}:[/] {Markup.Escape(library.GetCookieFilePath())}");
    }

    public static void ShowLoginHelp()
    {
        using var host = AppHostFactory.Create();
        var library = host.Services.GetRequiredService<ILibraryPathProvider>();
        var cookiePath = library.GetCookieFilePath();
        AnsiConsole.Write(new Panel(
            AppText.T("action.loginHelp", cookiePath))
        {
            Header = new PanelHeader(AppText.T("action.authHeader")),
            Border = BoxBorder.Rounded
        });
    }

    public static IReadOnlyList<string> GetSupportedBrowsers()
    {
        using var host = AppHostFactory.Create();
        var exporter = host.Services.GetRequiredService<IBrowserCookieExporter>();
        return exporter.GetSupportedBrowsers();
    }

    private static AudioFileChoice PromptAudioFile(string titleKey, IReadOnlyList<AudioFileChoice> files) =>
        AnsiConsole.Prompt(
            new SelectionPrompt<AudioFileChoice>()
                .Title($"[cyan]{Markup.Escape(AppText.T(titleKey))}[/]")
                .PageSize(20)
                .AddChoices(files)
                .UseConverter(file => Markup.Escape(file.DisplayName)));

    private static IEnumerable<AudioFileChoice> FindDownloadedAudioFiles(ILibraryPathProvider library)
    {
        var libraryRoot = library.GetLibraryRoot();
        var candidateRoots = new[]
            {
                library.GetTierRoot("Free"),
                library.GetTierRoot("Paid"),
                library.GetTierRoot("free"),
                library.GetTierRoot("paid")
            }
            .Distinct(StringComparer.OrdinalIgnoreCase);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var root in candidateRoots)
        {
            if (!Directory.Exists(root))
                continue;

            foreach (var path in Directory.EnumerateFiles(root, "*.mp3", SearchOption.AllDirectories))
            {
                var fullPath = Path.GetFullPath(path);
                if (!seen.Add(fullPath))
                    continue;

                var displayName = Path.GetRelativePath(libraryRoot, fullPath);
                yield return new AudioFileChoice(displayName, fullPath);
            }
        }
    }

    private static void WriteArchiveCreatedPanel(string archivePath, bool passwordProtected)
    {
        var fileInfo = new FileInfo(archivePath);
        var table = new Table().NoBorder();
        table.AddColumn(AppText.T("progress.metric"));
        table.AddColumn(AppText.T("progress.value"));
        table.AddRow(AppText.T("archive.path"), Markup.Escape(archivePath));
        table.AddRow(AppText.T("archive.size"), FormatBytes(fileInfo.Length));
        table.AddRow(AppText.T("archive.passwordProtected"), AppText.T(passwordProtected ? "common.yes" : "common.no"));

        AnsiConsole.Write(new Panel(table)
        {
            Header = new PanelHeader(AppText.T("archive.createdTitle")),
            Border = BoxBorder.Rounded
        });
    }

    private static string FormatBytes(long bytes)
    {
        string[] units = ["B", "KB", "MB", "GB", "TB"];
        var value = (double)bytes;
        var unitIndex = 0;
        while (value >= 1024 && unitIndex < units.Length - 1)
        {
            value /= 1024;
            unitIndex++;
        }

        return $"{value.ToString("0.##", CultureInfo.InvariantCulture)} {units[unitIndex]}";
    }

    public static async Task CountCatalogAsync(DownloadScope scope)
    {
        using var host = AppHostFactory.Create();
        var catalog = host.Services.GetRequiredService<IAudioCatalogService>();

        if (scope.HasFlag(DownloadScope.Free))
        {
            var urls = await catalog.GetPostUrlsAsync(AudioTier.Free, CancellationToken.None);
            AnsiConsole.MarkupLine($"[cyan]{Markup.Escape(AppText.T("action.freeCount", urls.Count))}[/]");
        }

        if (scope.HasFlag(DownloadScope.Paid))
        {
            var urls = await catalog.GetPostUrlsAsync(AudioTier.Paid, CancellationToken.None);
            AnsiConsole.MarkupLine($"[cyan]{Markup.Escape(AppText.T("action.paidCount", urls.Count))}[/]");
        }
    }

    public static DownloadScope ResolveScope(bool isFree, bool isPaid, bool isAll) =>
        isAll || isFree && isPaid
            ? DownloadScope.All
            : isFree
                ? DownloadScope.Free
                : isPaid
                    ? DownloadScope.Paid
                    : DownloadScope.None;
}
