using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Downloads;
using ForgiveMeCalia.Application.Downloads.Commands.DownloadAudio;
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
            AnsiConsole.MarkupLine("[red]Не выбран тип загрузки.[/]");
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
            AnsiConsole.MarkupLine("[red]Загрузка прервана.[/]");
            AnsiConsole.MarkupLine(Markup.Escape(DownloadExceptionFormatter.Format(ex)));
            AnsiConsole.MarkupLine(
                "[grey]Часто это временный сбой сети/TLS на macOS — запустите загрузку ещё раз.[/]");
        }
    }

    public static void ShowPaths()
    {
        using var host = AppHostFactory.Create();
        var library = host.Services.GetRequiredService<ILibraryPathProvider>();
        AnsiConsole.MarkupLine($"[cyan]Музыка:[/] {Markup.Escape(library.GetLibraryRoot())}");
        AnsiConsole.MarkupLine($"[cyan]Cookies:[/] {Markup.Escape(library.GetCookieFilePath())}");
    }

    public static async Task ImportCookiesAsync(string? browser = null)
    {
        using var host = AppHostFactory.Create();
        var exporter = host.Services.GetRequiredService<IBrowserCookieExporter>();
        var library = host.Services.GetRequiredService<ILibraryPathProvider>();

        AnsiConsole.MarkupLine("[cyan]Импорт cookies через yt-dlp…[/]");
        if (browser is null)
            AnsiConsole.MarkupLine("[grey]Пробую браузеры: safari → chrome → firefox …[/]");

        var result = await exporter.ExportAsync(browser);
        if (result.Success)
        {
            AnsiConsole.MarkupLine(
                $"[green]Готово.[/] Браузер: [yellow]{result.Browser}[/], файл: {Markup.Escape(result.CookieFilePath)}");
            if (result.YtDlpWasInstalled)
                AnsiConsole.MarkupLine("[grey]yt-dlp установлен через Homebrew.[/]");
            return;
        }

        AnsiConsole.MarkupLine("[red]Не удалось импортировать cookies.[/]");
        if (result.IsPermissionDenied)
        {
            AnsiConsole.Write(new Panel(YtDlpBrowserCookieExporter.GetMacPermissionHelp())
            {
                Header = new PanelHeader("macOS: нужен «Полный доступ к диску»"),
                Border = BoxBorder.Rounded
            });
        }

        if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
            AnsiConsole.MarkupLine(Markup.Escape(result.ErrorMessage));

        AnsiConsole.MarkupLine($"[grey]Путь к файлу:[/] {Markup.Escape(library.GetCookieFilePath())}");
    }

    public static void ShowLoginHelp()
    {
        using var host = AppHostFactory.Create();
        var library = host.Services.GetRequiredService<ILibraryPathProvider>();
        var cookiePath = library.GetCookieFilePath();
        AnsiConsole.Write(new Panel(
            $"""
            1. Войдите на https://mistresscalia.com через Patreon в Safari (или Chrome/Firefox).
            2. В меню выберите «Импорт cookies из браузера» — программа сама:
               • установит yt-dlp через brew (если нет);
               • выгрузит cookies в {cookiePath}
            3. На Mac может понадобиться «Полный доступ к диску» для Terminal/Rider
               (Системные настройки → Конфиденциальность).
            4. Запустите загрузку платных или всех файлов.

            Бесплатные файлы cookies не требуют.
            """)
        {
            Header = new PanelHeader("Авторизация без GUI"),
            Border = BoxBorder.Rounded
        });
    }

    public static async Task CountCatalogAsync(DownloadScope scope)
    {
        using var host = AppHostFactory.Create();
        var catalog = host.Services.GetRequiredService<IAudioCatalogService>();

        if (scope.HasFlag(DownloadScope.Free))
        {
            var urls = await catalog.GetPostUrlsAsync(AudioTier.Free, CancellationToken.None);
            AnsiConsole.MarkupLine($"[cyan]Бесплатные:[/] {urls.Count} записей");
        }

        if (scope.HasFlag(DownloadScope.Paid))
        {
            var urls = await catalog.GetPostUrlsAsync(AudioTier.Paid, CancellationToken.None);
            AnsiConsole.MarkupLine($"[cyan]Платные:[/] {urls.Count} записей");
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
