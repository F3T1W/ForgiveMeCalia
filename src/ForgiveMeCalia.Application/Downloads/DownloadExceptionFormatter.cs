namespace ForgiveMeCalia.Application.Downloads;

public static class DownloadExceptionFormatter
{
    public static string Format(Exception exception)
    {
        var parts = new List<string>();
        for (var ex = exception; ex is not null; ex = ex.InnerException)
        {
            var message = ex.Message.Trim();
            if (message.Length > 0 && (parts.Count == 0 || parts[^1] != message))
                parts.Add(message);
        }

        return parts.Count == 0 ? exception.GetType().Name : string.Join(" → ", parts);
    }
}
