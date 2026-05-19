using System.Globalization;
using System.Net;

namespace ForgiveMeCalia.Infrastructure.Auth;

public static class NetscapeCookieReader
{
    public static IReadOnlyList<Cookie> Read(string filePath)
    {
        var cookies = new List<Cookie>();
        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            var parts = line.Split('\t');
            if (parts.Length < 7)
                continue;

            var domain = parts[0];
            var path = parts[2];
            var secure = parts[3].Equals("TRUE", StringComparison.OrdinalIgnoreCase);
            var expires = long.Parse(parts[4], CultureInfo.InvariantCulture);
            var name = parts[5];
            var value = parts[6];

            var cookie = new Cookie(name, value, path, domain)
            {
                Secure = secure
            };

            if (expires > 0)
                cookie.Expires = DateTimeOffset.FromUnixTimeSeconds(expires).UtcDateTime;

            cookies.Add(cookie);
        }

        return cookies;
    }
}
