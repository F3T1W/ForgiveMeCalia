# ForgiveMeCalia

CLI audio downloader for personal archival use with `mistresscalia.com`.

The app scans free and Patreon-accessible posts, extracts MP3 links, stores files under `~/Music/MistressCalia`, and keeps track of already downloaded files.

## Features

- Interactive menu when launched without command-line arguments.
- Runtime language switch in the interactive menu: English, Russian, Irish, Korean, Japanese, and Uzbek.
- Download free, paid, or all accessible audio files.
- Import cookies from Safari, Chrome, Firefox, Brave, Chromium, or Edge via `yt-dlp`.
- Use an existing Patreon session through browser cookies.
- Crawl WordPress category pagination.
- Skip files that already exist on disk.
- Keep a local index of downloaded source URLs and prune stale entries.
- Retry HTTP requests after temporary TLS or network failures.
- Print a compact summary and a dedicated error section.

## Requirements

- macOS, Linux, or Windows.
- .NET SDK 10.0 or newer.
- `yt-dlp` for browser cookie import.
- On macOS, reading browser cookies may require Full Disk Access for Terminal, Rider, or whichever app launches the CLI.

If `yt-dlp` is not installed, the app will try to install it through Homebrew.

## Quick Start

Run the interactive menu:

```bash
dotnet run --project src/ForgiveMeCalia.Cli
```

Show music and cookie paths:

```bash
dotnet run --project src/ForgiveMeCalia.Cli -- paths
```

Count catalog posts without downloading:

```bash
dotnet run --project src/ForgiveMeCalia.Cli -- catalog count --all
```

Download free files only:

```bash
dotnet run --project src/ForgiveMeCalia.Cli -- download --free
```

Download paid files only:

```bash
dotnet run --project src/ForgiveMeCalia.Cli -- download --paid
```

Download everything accessible:

```bash
dotnet run --project src/ForgiveMeCalia.Cli -- download --all
```

Set download parallelism:

```bash
dotnet run --project src/ForgiveMeCalia.Cli -- download --all --parallel 4
```

## Cookies and Patreon

For paid files, sign in to `mistresscalia.com` through Patreon in your browser, then import cookies:

```bash
dotnet run --project src/ForgiveMeCalia.Cli -- cookies import --browser safari
```

Supported `--browser` values:

```text
safari, chrome, chromium, brave, firefox, edge
```

If no browser is provided, the app will try several browsers automatically.

On macOS, if cookie access fails:

1. Open System Settings.
2. Go to Privacy & Security -> Full Disk Access.
3. Enable access for Terminal, Rider, or whichever app launches `dotnet run`.
4. Fully restart the terminal or IDE.

## Output Paths

Default music library:

```text
~/Music/MistressCalia/
```

Cookies:

```text
~/Library/Application Support/ForgiveMeCalia/cookies.txt
```

Downloaded URL index:

```text
~/Music/MistressCalia/.download-index.json
```

If a file is deleted manually, the app will prune the stale index entry on the next run and can download the file again.

## Disclaimer

This project is intended for personal archival of content the user already has legitimate access to. Do not use it to bypass paid access, redistribute third-party content, or violate the terms of the site, Patreon, or copyright law.

This project is not affiliated with Mistress Calia, Patreon, or `mistresscalia.com`.

## License

MIT. See `LICENSE`.
