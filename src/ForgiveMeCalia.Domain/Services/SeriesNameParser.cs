using System.Globalization;
using System.Text.RegularExpressions;

namespace ForgiveMeCalia.Domain.Services;

public static partial class SeriesNameParser
{
    private static readonly (Regex Pattern, Func<Match, int?> PartResolver)[] Rules =
    [
        (PartNumberSuffix(), m => int.Parse(m.Groups["part"].Value, CultureInfo.InvariantCulture)),
        (PartWordSuffix(), m => WordToNumber(m.Groups["part"].Value)),
        (PartRomanSuffix(), m => RomanToNumber(m.Groups["part"].Value))
    ];

    public static SeriesParseResult Parse(string title, string slug)
    {
        foreach (var source in new[] { title, SlugToTitle(slug), slug.Replace('-', ' ') })
        {
            foreach (var (pattern, resolver) in Rules)
            {
                var match = pattern.Match(source);
                if (!match.Success)
                    continue;

                var baseName = match.Groups["base"].Value.Trim().Trim('-', ' ');
                var part = resolver(match);
                if (part is null or < 1)
                    continue;

                return new SeriesParseResult(
                    SeriesKey: NormalizeKey(baseName),
                    FolderName: SanitizeFolderName(baseName),
                    PartNumber: part.Value);
            }
        }

        return SeriesParseResult.Standalone(SanitizeFolderName(title));
    }

    private static string SlugToTitle(string slug) =>
        string.Join(' ', slug.Split('-', StringSplitOptions.RemoveEmptyEntries));

    private static string NormalizeKey(string value) =>
        NonKeyCharacters().Replace(value.ToLowerInvariant(), "-").Trim('-');

    public static string SanitizeFolderName(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        var sanitized = new string([.. value.Select(ch => invalid.Contains(ch) ? '_' : ch)])
            .Trim();

        return string.IsNullOrWhiteSpace(sanitized) ? "untitled" : sanitized;
    }

    private static int? WordToNumber(string word) => word.ToLowerInvariant() switch
    {
        "one" or "first" => 1,
        "two" or "second" => 2,
        "three" or "third" => 3,
        "four" or "fourth" => 4,
        "five" or "fifth" => 5,
        "six" or "sixth" => 6,
        "seven" or "seventh" => 7,
        "eight" or "eighth" => 8,
        "nine" or "ninth" => 9,
        "ten" or "tenth" => 10,
        _ => int.TryParse(word, out var n) ? n : null
    };

    private static int? RomanToNumber(string roman)
    {
        var map = new Dictionary<char, int>
        {
            ['i'] = 1,
            ['v'] = 5,
            ['x'] = 10
        };

        var sum = 0;
        var prev = 0;
        foreach (var c in roman.ToLowerInvariant().Reverse())
        {
            if (!map.TryGetValue(c, out var value))
                return null;

            sum += value < prev ? -value : value;
            prev = value;
        }

        return sum > 0 ? sum : null;
    }

    [GeneratedRegex(@"^(?<base>.+?)[\s\-–—]*part[\s\-–—]*(?<part>\d+)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex PartNumberSuffix();

    [GeneratedRegex(@"^(?<base>.+?)[\s\-–—]*part[\s\-–—]*(?<part>one|two|three|four|five|six|seven|eight|nine|ten|first|second|third|fourth|fifth|sixth|seventh|eighth|ninth|tenth)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex PartWordSuffix();

    [GeneratedRegex(@"^(?<base>.+?)[\s\-–—]*part[\s\-–—]*(?<part>i{1,3}|iv|v|vi{0,3}|ix|x)\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex PartRomanSuffix();
    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonKeyCharacters();
}

public sealed record SeriesParseResult(
    string? SeriesKey,
    string FolderName,
    int? PartNumber)
{
    public static SeriesParseResult Standalone(string folderName) =>
        new(null, folderName, null);
}
