using System.Diagnostics;
using System.Text;

namespace ForgiveMeCalia.Infrastructure.Platform;

internal static class ExternalProcessRunner
{
    public static async Task<ProcessResult> RunAsync(
        string fileName,
        IReadOnlyList<string> arguments,
        CancellationToken cancellationToken)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        foreach (var argument in arguments)
            psi.ArgumentList.Add(argument);

        using var process = new Process();
        process.StartInfo = psi;
        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is not null)
                stdout.AppendLine(e.Data);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is not null)
                stderr.AppendLine(e.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken);

        return new ProcessResult(
            process.ExitCode,
            stdout.ToString(),
            stderr.ToString());
    }

    public static async Task<bool> ExistsOnPathAsync(string command, CancellationToken cancellationToken)
    {
        if (OperatingSystem.IsWindows())
        {
            var where = await RunAsync("where", [command], cancellationToken);
            return where.ExitCode == 0 && !string.IsNullOrWhiteSpace(where.StandardOutput);
        }

        var which = await RunAsync("which", [command], cancellationToken);
        return which.ExitCode == 0 && !string.IsNullOrWhiteSpace(which.StandardOutput);
    }
}

internal sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError)
{
    public string CombinedOutput => $"{StandardOutput}{Environment.NewLine}{StandardError}".Trim();
}
