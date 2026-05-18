using System.Text;
using ForgiveMeCalia.Application.Abstractions;
using ForgiveMeCalia.Application.Downloads;
using Spectre.Console;

namespace ForgiveMeCalia.Cli.Reporting;

public sealed class SpectreProgressReporter : IProgressReporter
{
    private readonly object _sync = new();
    private readonly HashSet<string> _completedDownloads = new(StringComparer.Ordinal);
    private int _downloadCompleted;

    public void ReportPhase(string message)
    {
        lock (_sync)
        {
            if (message.Contains("Загрузка", StringComparison.Ordinal))
            {
                _completedDownloads.Clear();
                _downloadCompleted = 0;
            }

            AnsiConsole.MarkupLine($"[cyan]▶[/] {Markup.Escape(message)}");
        }
    }

    public void ReportDiscovery(int current, int total, string itemName)
    {
        lock (_sync)
            AnsiConsole.MarkupLine(
                $"[grey]Обнаружение[/] [yellow]{current}[/]/[yellow]{total}[/] — {Markup.Escape(Truncate(itemName, 70))}");
    }

    public void ReportDownload(int current, int total, string fileName, double fileProgress)
    {
        if (fileProgress < 1.0)
            return;

        lock (_sync)
        {
            var key = $"{current}:{fileName}";
            if (!_completedDownloads.Add(key))
                return;

            _downloadCompleted++;
            AnsiConsole.MarkupLine(
                $"[green]✓[/] [grey]{_downloadCompleted}[/]/[grey]{total}[/] — {Markup.Escape(Truncate(fileName, 70))}");
        }
    }

    public void ReportSkipped(string reason, string itemName)
    {
        // Счётчик «пропущено» есть в итоговой таблице — не засоряем консоль построчным выводом.
    }

    public void ReportWarning(string message)
    {
        lock (_sync)
            AnsiConsole.MarkupLine($"[yellow]![/] {Markup.Escape(message)}");
    }

    public void ReportError(string message, Exception? exception = null)
    {
        var detail = exception is null ? null : DownloadExceptionFormatter.Format(exception);
        lock (_sync)
        {
            AnsiConsole.MarkupLine($"[red]✗[/] {Markup.Escape(message)}");
            if (!string.IsNullOrWhiteSpace(detail))
                AnsiConsole.MarkupLine($"[red]  →[/] {Markup.Escape(detail)}");
        }
    }

    public void ReportSummary(DownloadSummary summary)
    {
        lock (_sync)
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("Метрика");
            table.AddColumn("Значение");
            table.AddRow("Найдено на сайте", summary.Discovered.ToString());
            table.AddRow("Скачано", summary.Downloaded.ToString());
            table.AddRow("Пропущено (уже есть)", summary.Skipped.ToString());
            table.AddRow("Заблокировано (нет доступа)", summary.Locked.ToString());
            table.AddRow("Ошибок", summary.Failed.ToString());
            AnsiConsole.Write(table);

            if (summary.Failures.Count == 0)
                return;

            var body = new StringBuilder();
            foreach (var failure in summary.Failures)
            {
                body.AppendLine($"[bold]{Markup.Escape(failure.Title)}[/]");
                body.AppendLine(Markup.Escape(failure.Message));
                body.AppendLine();
            }

            AnsiConsole.Write(new Panel(body.ToString().TrimEnd())
            {
                Header = new PanelHeader($"Ошибки ({summary.Failures.Count})"),
                Border = BoxBorder.Rounded
            });
        }
    }

    private static string Truncate(string value, int maxLength) =>
        value.Length <= maxLength ? value : value[..(maxLength - 1)] + "…";
}
