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

    public static DownloadScope ResolveScope(bool isFree, bool isPaid, bool isAll)
    {
        if (isAll)
            return DownloadScope.All;
        if (isFree && isPaid)
            return DownloadScope.All;
        if (isFree)
            return DownloadScope.Free;
        if (isPaid)
            return DownloadScope.Paid;
        return DownloadScope.None;
    }
}
