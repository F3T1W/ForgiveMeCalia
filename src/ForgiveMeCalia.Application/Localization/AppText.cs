using System.Globalization;

namespace ForgiveMeCalia.Application.Localization;

public static class AppText
{
    public static AppLanguage CurrentLanguage { get; set; } = AppLanguage.English;

    public static IReadOnlyList<AppLanguage> SupportedLanguages =>
    [
        AppLanguage.English,
        AppLanguage.Russian,
        AppLanguage.Irish,
        AppLanguage.Korean,
        AppLanguage.Japanese,
        AppLanguage.Uzbek
    ];

    public static string LanguageName(AppLanguage language) => language switch
    {
        AppLanguage.English => "English",
        AppLanguage.Russian => "Русский",
        AppLanguage.Irish => "Gaeilge",
        AppLanguage.Korean => "한국어",
        AppLanguage.Japanese => "日本語",
        AppLanguage.Uzbek => "O'zbekcha",
        _ => language.ToString()
    };

    public static string T(string key, params object?[] args)
    {
        var value = TryGet(CurrentLanguage, key)
                    ?? TryGet(AppLanguage.English, key)
                    ?? key;

        return args.Length == 0 ? value : string.Format(CultureInfo.InvariantCulture, value, args);
    }

    private static string? TryGet(AppLanguage language, string key) =>
        Translations.GetValueOrDefault((language, key));

    private static readonly Dictionary<(AppLanguage Language, string Key), string> Translations = new()
    {
        // English
        [(AppLanguage.English, "app.subtitle")] = "Audio downloader for mistresscalia.com",
        [(AppLanguage.English, "menu.title")] = "Choose an action:",
        [(AppLanguage.English, "menu.downloadFree")] = "Download free files",
        [(AppLanguage.English, "menu.downloadPaid")] = "Download paid files (cookies required)",
        [(AppLanguage.English, "menu.downloadAll")] = "Download everything (free + paid)",
        [(AppLanguage.English, "menu.createCustomAudio")] = "Create custom audio (induction + hypnosis)",
        [(AppLanguage.English, "menu.createLibraryArchive")] = "Create ZIP archive (Free + Paid + Custom)",
        [(AppLanguage.English, "menu.showPaths")] = "Show paths (music and cookies)",
        [(AppLanguage.English, "menu.importCookies")] = "Import browser cookies",
        [(AppLanguage.English, "menu.configureParallel")] = "Configure download parallelism",
        [(AppLanguage.English, "menu.changeLanguage")] = "Change language",
        [(AppLanguage.English, "menu.exit")] = "Exit",
        [(AppLanguage.English, "menu.parallelPrompt")] = "Parallel downloads",
        [(AppLanguage.English, "menu.parallelError")] = "Enter a number from 1 to 16.",
        [(AppLanguage.English, "menu.set")] = "Set: {0}",
        [(AppLanguage.English, "menu.goodbye")] = "Goodbye.",
        [(AppLanguage.English, "menu.return")] = "Return to menu?",
        [(AppLanguage.English, "menu.main")] = "main menu",
        [(AppLanguage.English, "menu.browserManual")] = "Specify browser manually?",
        [(AppLanguage.English, "menu.browserAutoHint")] = "otherwise tries available browsers automatically",
        [(AppLanguage.English, "menu.browser")] = "Browser",
        [(AppLanguage.English, "menu.language")] = "Language",
        [(AppLanguage.English, "menu.languageSet")] = "Language set to {0}.",

        [(AppLanguage.English, "action.noScope")] = "No download scope selected.",
        [(AppLanguage.English, "action.interrupted")] = "Download interrupted.",
        [(AppLanguage.English, "action.networkHint")] = "This is often a temporary network/TLS issue on macOS. Run the download again.",
        [(AppLanguage.English, "action.music")] = "Music",
        [(AppLanguage.English, "action.cookies")] = "Cookies",
        [(AppLanguage.English, "action.custom")] = "Custom",
        [(AppLanguage.English, "action.importingCookies")] = "Importing cookies through yt-dlp...",
        [(AppLanguage.English, "action.tryingBrowsers")] = "Trying available browsers automatically...",
        [(AppLanguage.English, "action.doneBrowserFile")] = "Done. Browser: {0}, file: {1}",
        [(AppLanguage.English, "action.ytDlpInstalled")] = "yt-dlp was installed through Homebrew.",
        [(AppLanguage.English, "action.cookieImportFailed")] = "Failed to import cookies.",
        [(AppLanguage.English, "action.cookieAccessHeader")] = "Browser Cookie Access",
        [(AppLanguage.English, "action.filePath")] = "File path",
        [(AppLanguage.English, "action.authHeader")] = "Browser Authentication",
        [(AppLanguage.English, "action.loginHelp")] =
            "1. Sign in to https://mistresscalia.com through Patreon in Safari (or Chrome/Firefox).\n" +
            "2. Select browser cookie import in the menu. The app will:\n" +
            "   - install yt-dlp through brew if needed;\n" +
            "   - export cookies to {0}\n" +
            "3. On macOS, Terminal/Rider may need Full Disk Access\n" +
            "   (System Settings -> Privacy & Security).\n" +
            "4. Start a paid or full download.\n\n" +
            "Free files do not require cookies.",
        [(AppLanguage.English, "action.freeCount")] = "Free: {0} posts",
        [(AppLanguage.English, "action.paidCount")] = "Paid: {0} posts",

        [(AppLanguage.English, "custom.notEnoughFiles")] = "At least two downloaded MP3 files are required.",
        [(AppLanguage.English, "custom.selectInduction")] = "Select induction file",
        [(AppLanguage.English, "custom.selectMain")] = "Select main hypnosis file",
        [(AppLanguage.English, "custom.sameFileConfirm")] = "You selected the same file twice. Continue?",
        [(AppLanguage.English, "custom.cancelled")] = "Custom audio creation cancelled.",
        [(AppLanguage.English, "custom.created")] = "Custom audio created: {0}",
        [(AppLanguage.English, "custom.failed")] = "Failed to create custom audio: {0}",

        [(AppLanguage.English, "archive.usePassword")] = "Protect the ZIP with a password?",
        [(AppLanguage.English, "archive.password")] = "ZIP password (leave empty for no password)",
        [(AppLanguage.English, "archive.creating")] = "Creating archive...",
        [(AppLanguage.English, "archive.createdTitle")] = "Archive created",
        [(AppLanguage.English, "archive.created")] = "Archive created: {0}",
        [(AppLanguage.English, "archive.failed")] = "Failed to create archive: {0}",
        [(AppLanguage.English, "archive.noContent")] = "There is no content to archive in Free, Paid, or Custom.",
        [(AppLanguage.English, "archive.path")] = "Path",
        [(AppLanguage.English, "archive.size")] = "Size",
        [(AppLanguage.English, "archive.passwordProtected")] = "Password protected",
        [(AppLanguage.English, "common.yes")] = "Yes",
        [(AppLanguage.English, "common.no")] = "No",

        [(AppLanguage.English, "download.checkCookies")] = "Checking Patreon session (cookies)...",
        [(AppLanguage.English, "download.queue")] = "Queued for download: {0} files (already on disk: {1}, locked: {2}).",
        [(AppLanguage.English, "download.downloading")] = "Downloading {0} files (parallelism: {1})...",
        [(AppLanguage.English, "download.fileExists")] = "file already exists",
        [(AppLanguage.English, "download.failed")] = "Failed to download \"{0}\"",
        [(AppLanguage.English, "download.scanningFree")] = "Scanning free posts...",
        [(AppLanguage.English, "download.scanningPaid")] = "Scanning paid posts...",
        [(AppLanguage.English, "download.readFailed")] = "Failed to read ({0}/{1}): {2}",
        [(AppLanguage.English, "download.validatorScope")] = "Specify --free, --paid, or --all.",

        [(AppLanguage.English, "progress.discovery")] = "Discovery",
        [(AppLanguage.English, "progress.metric")] = "Metric",
        [(AppLanguage.English, "progress.value")] = "Value",
        [(AppLanguage.English, "progress.found")] = "Found on site",
        [(AppLanguage.English, "progress.downloaded")] = "Downloaded",
        [(AppLanguage.English, "progress.skipped")] = "Skipped (already exists)",
        [(AppLanguage.English, "progress.locked")] = "Locked (no access)",
        [(AppLanguage.English, "progress.errors")] = "Errors",

        [(AppLanguage.English, "cookies.ytDlpMissing")] = "yt-dlp was not found and automatic installation is unavailable.\nInstall it with your package manager or download it from:\nhttps://github.com/yt-dlp/yt-dlp",
        [(AppLanguage.English, "cookies.noCookiesForHost")] = "Browser \"{0}\": no cookies found for {1}.\nSign in to the site through Patreon in this browser and import cookies again.",
        [(AppLanguage.English, "cookies.browserError")] = "Browser \"{0}\": {1}",
        [(AppLanguage.English, "cookies.exportFailed")] = "Failed to export cookies.",
        [(AppLanguage.English, "cookies.permissionMac")] = "macOS blocked access to browser cookies (Operation not permitted). sudo does not bypass this.",
        [(AppLanguage.English, "cookies.permissionOther")] = "The browser cookie database could not be read. Check browser and file permissions.",
        [(AppLanguage.English, "cookies.lastError")] = "Last error:",
        [(AppLanguage.English, "cookies.permissionHelpMac")] =
            "1. Open System Settings -> Privacy & Security -> Full Disk Access.\n" +
            "2. Enable Terminal (or Rider / iTerm / whichever app launches dotnet run).\n" +
            "3. Restart the terminal and import cookies again.\n" +
            "4. Safari must be signed in to mistresscalia.com through Patreon.\n\n" +
            "If Safari still cannot be accessed after granting permission, sign in with Firefox/Chrome and import from firefox or chrome.",
        [(AppLanguage.English, "cookies.permissionHelpOther")] =
            "1. Close the browser before importing cookies, then try again.\n" +
            "2. Make sure the app that launches dotnet run can read the browser profile.\n" +
            "3. Sign in to mistresscalia.com through Patreon before importing cookies.",
        [(AppLanguage.English, "cookies.importFailed")] = "Failed to import cookies.",
        [(AppLanguage.English, "cookies.fileMissing")] = "Cookie file was not found: {0}\n\nSelect browser cookie import in the menu or run:\n  dotnet run --project src/ForgiveMeCalia.Cli -- cookies import",
        [(AppLanguage.English, "cookies.fileEmpty")] = "Cookie file is empty: {0}\nImport browser cookies again.",
        [(AppLanguage.English, "errors.httpRetry")] = "HTTP retry failed without exception.",
        [(AppLanguage.English, "errors.mp3Required")] = "MP3 URL is required.",

        // Russian
        [(AppLanguage.Russian, "app.subtitle")] = "Загрузчик аудио для mistresscalia.com",
        [(AppLanguage.Russian, "menu.title")] = "Выберите действие:",
        [(AppLanguage.Russian, "menu.downloadFree")] = "Скачать бесплатные файлы",
        [(AppLanguage.Russian, "menu.downloadPaid")] = "Скачать платные файлы (нужны cookies)",
        [(AppLanguage.Russian, "menu.downloadAll")] = "Скачать всё (бесплатные + платные)",
        [(AppLanguage.Russian, "menu.createCustomAudio")] = "Создать кастомное аудио (индукция + гипноз)",
        [(AppLanguage.Russian, "menu.createLibraryArchive")] = "Создать ZIP-архив (Free + Paid + Custom)",
        [(AppLanguage.Russian, "menu.showPaths")] = "Показать пути (музыка и cookies)",
        [(AppLanguage.Russian, "menu.importCookies")] = "Импортировать cookies браузера",
        [(AppLanguage.Russian, "menu.configureParallel")] = "Настроить параллельность загрузок",
        [(AppLanguage.Russian, "menu.changeLanguage")] = "Сменить язык",
        [(AppLanguage.Russian, "menu.exit")] = "Выход",
        [(AppLanguage.Russian, "menu.parallelPrompt")] = "Параллельных загрузок",
        [(AppLanguage.Russian, "menu.parallelError")] = "Введите число от 1 до 16.",
        [(AppLanguage.Russian, "menu.set")] = "Установлено: {0}",
        [(AppLanguage.Russian, "menu.goodbye")] = "До встречи.",
        [(AppLanguage.Russian, "menu.return")] = "Вернуться в меню?",
        [(AppLanguage.Russian, "menu.main")] = "главное меню",
        [(AppLanguage.Russian, "menu.browserManual")] = "Указать браузер вручную?",
        [(AppLanguage.Russian, "menu.browserAutoHint")] = "иначе будут автоматически проверены доступные браузеры",
        [(AppLanguage.Russian, "menu.browser")] = "Браузер",
        [(AppLanguage.Russian, "menu.language")] = "Язык",
        [(AppLanguage.Russian, "menu.languageSet")] = "Язык изменён на {0}.",

        [(AppLanguage.Russian, "action.noScope")] = "Тип загрузки не выбран.",
        [(AppLanguage.Russian, "action.interrupted")] = "Загрузка прервана.",
        [(AppLanguage.Russian, "action.networkHint")] = "Часто это временный сбой сети/TLS на macOS. Запустите загрузку ещё раз.",
        [(AppLanguage.Russian, "action.music")] = "Музыка",
        [(AppLanguage.Russian, "action.cookies")] = "Cookies",
        [(AppLanguage.Russian, "action.custom")] = "Custom",
        [(AppLanguage.Russian, "action.importingCookies")] = "Импорт cookies через yt-dlp...",
        [(AppLanguage.Russian, "action.tryingBrowsers")] = "Автоматически проверяю доступные браузеры...",
        [(AppLanguage.Russian, "action.doneBrowserFile")] = "Готово. Браузер: {0}, файл: {1}",
        [(AppLanguage.Russian, "action.ytDlpInstalled")] = "yt-dlp установлен через Homebrew.",
        [(AppLanguage.Russian, "action.cookieImportFailed")] = "Не удалось импортировать cookies.",
        [(AppLanguage.Russian, "action.cookieAccessHeader")] = "Доступ к cookies браузера",
        [(AppLanguage.Russian, "action.filePath")] = "Путь к файлу",
        [(AppLanguage.Russian, "action.authHeader")] = "Авторизация в браузере",
        [(AppLanguage.Russian, "action.loginHelp")] =
            "1. Войдите на https://mistresscalia.com через Patreon в Safari (или Chrome/Firefox).\n" +
            "2. Выберите импорт cookies браузера в меню. Приложение:\n" +
            "   - установит yt-dlp через brew, если нужно;\n" +
            "   - экспортирует cookies в {0}\n" +
            "3. На macOS Terminal/Rider может понадобиться Полный доступ к диску\n" +
            "   (Системные настройки -> Конфиденциальность и безопасность).\n" +
            "4. Запустите платную или полную загрузку.\n\n" +
            "Бесплатные файлы не требуют cookies.",
        [(AppLanguage.Russian, "action.freeCount")] = "Бесплатные: {0} записей",
        [(AppLanguage.Russian, "action.paidCount")] = "Платные: {0} записей",

        [(AppLanguage.Russian, "custom.notEnoughFiles")] = "Нужно минимум два скачанных MP3-файла.",
        [(AppLanguage.Russian, "custom.selectInduction")] = "Выберите файл индукции",
        [(AppLanguage.Russian, "custom.selectMain")] = "Выберите файл основного гипноза",
        [(AppLanguage.Russian, "custom.sameFileConfirm")] = "Вы выбрали один и тот же файл дважды. Продолжить?",
        [(AppLanguage.Russian, "custom.cancelled")] = "Создание кастомного аудио отменено.",
        [(AppLanguage.Russian, "custom.created")] = "Кастомное аудио создано: {0}",
        [(AppLanguage.Russian, "custom.failed")] = "Не удалось создать кастомное аудио: {0}",

        [(AppLanguage.Russian, "archive.usePassword")] = "Защитить ZIP паролем?",
        [(AppLanguage.Russian, "archive.password")] = "Пароль ZIP (оставьте пустым, чтобы не ставить пароль)",
        [(AppLanguage.Russian, "archive.creating")] = "Создаю архив...",
        [(AppLanguage.Russian, "archive.createdTitle")] = "Архив создан",
        [(AppLanguage.Russian, "archive.created")] = "Архив создан: {0}",
        [(AppLanguage.Russian, "archive.failed")] = "Не удалось создать архив: {0}",
        [(AppLanguage.Russian, "archive.noContent")] = "В Free, Paid или Custom нет содержимого для архивации.",
        [(AppLanguage.Russian, "archive.path")] = "Путь",
        [(AppLanguage.Russian, "archive.size")] = "Размер",
        [(AppLanguage.Russian, "archive.passwordProtected")] = "Защищён паролем",
        [(AppLanguage.Russian, "common.yes")] = "Да",
        [(AppLanguage.Russian, "common.no")] = "Нет",

        [(AppLanguage.Russian, "download.checkCookies")] = "Проверка Patreon-сессии (cookies)...",
        [(AppLanguage.Russian, "download.queue")] = "К загрузке: {0} файлов (уже на диске: {1}, заблокировано: {2}).",
        [(AppLanguage.Russian, "download.downloading")] = "Загрузка {0} файлов (параллельно: {1})...",
        [(AppLanguage.Russian, "download.fileExists")] = "файл уже существует",
        [(AppLanguage.Russian, "download.failed")] = "Не удалось скачать \"{0}\"",
        [(AppLanguage.Russian, "download.scanningFree")] = "Сканирование бесплатных записей...",
        [(AppLanguage.Russian, "download.scanningPaid")] = "Сканирование платных записей...",
        [(AppLanguage.Russian, "download.readFailed")] = "Не удалось прочитать ({0}/{1}): {2}",
        [(AppLanguage.Russian, "download.validatorScope")] = "Укажите --free, --paid или --all.",

        [(AppLanguage.Russian, "progress.discovery")] = "Обнаружение",
        [(AppLanguage.Russian, "progress.metric")] = "Метрика",
        [(AppLanguage.Russian, "progress.value")] = "Значение",
        [(AppLanguage.Russian, "progress.found")] = "Найдено на сайте",
        [(AppLanguage.Russian, "progress.downloaded")] = "Скачано",
        [(AppLanguage.Russian, "progress.skipped")] = "Пропущено (уже есть)",
        [(AppLanguage.Russian, "progress.locked")] = "Заблокировано (нет доступа)",
        [(AppLanguage.Russian, "progress.errors")] = "Ошибок",

        [(AppLanguage.Russian, "cookies.ytDlpMissing")] = "yt-dlp не найден, автоматическая установка недоступна.\nУстановите его через менеджер пакетов или скачайте с:\nhttps://github.com/yt-dlp/yt-dlp",
        [(AppLanguage.Russian, "cookies.noCookiesForHost")] = "Браузер \"{0}\": cookies для {1} не найдены.\nВойдите на сайт через Patreon в этом браузере и повторите импорт.",
        [(AppLanguage.Russian, "cookies.browserError")] = "Браузер \"{0}\": {1}",
        [(AppLanguage.Russian, "cookies.exportFailed")] = "Не удалось экспортировать cookies.",
        [(AppLanguage.Russian, "cookies.permissionMac")] = "macOS заблокировала доступ к cookies браузера (Operation not permitted). sudo это не обходит.",
        [(AppLanguage.Russian, "cookies.permissionOther")] = "Не удалось прочитать базу cookies браузера. Проверьте права браузера и файлов.",
        [(AppLanguage.Russian, "cookies.lastError")] = "Последняя ошибка:",
        [(AppLanguage.Russian, "cookies.permissionHelpMac")] =
            "1. Откройте Системные настройки -> Конфиденциальность и безопасность -> Полный доступ к диску.\n" +
            "2. Включите Terminal (или Rider / iTerm / приложение, которое запускает dotnet run).\n" +
            "3. Перезапустите терминал и повторите импорт cookies.\n" +
            "4. В Safari должен быть выполнен вход на mistresscalia.com через Patreon.\n\n" +
            "Если Safari всё равно недоступен, войдите через Firefox/Chrome и импортируйте из firefox или chrome.",
        [(AppLanguage.Russian, "cookies.permissionHelpOther")] =
            "1. Закройте браузер перед импортом cookies и повторите попытку.\n" +
            "2. Убедитесь, что приложение, запускающее dotnet run, может читать профиль браузера.\n" +
            "3. Перед импортом cookies войдите на mistresscalia.com через Patreon.",
        [(AppLanguage.Russian, "cookies.importFailed")] = "Не удалось импортировать cookies.",
        [(AppLanguage.Russian, "cookies.fileMissing")] = "Файл cookies не найден: {0}\n\nВыберите импорт cookies браузера в меню или выполните:\n  dotnet run --project src/ForgiveMeCalia.Cli -- cookies import",
        [(AppLanguage.Russian, "cookies.fileEmpty")] = "Файл cookies пуст: {0}\nПовторите импорт cookies браузера.",
        [(AppLanguage.Russian, "errors.httpRetry")] = "Повтор HTTP-запроса завершился без исключения.",
        [(AppLanguage.Russian, "errors.mp3Required")] = "Требуется MP3 URL.",

        // Irish
        [(AppLanguage.Irish, "app.subtitle")] = "Íoslódálaí fuaime do mistresscalia.com",
        [(AppLanguage.Irish, "menu.title")] = "Roghnaigh gníomh:",
        [(AppLanguage.Irish, "menu.downloadFree")] = "Íoslódáil comhaid saor in aisce",
        [(AppLanguage.Irish, "menu.downloadPaid")] = "Íoslódáil comhaid íoctha (cookies ag teastáil)",
        [(AppLanguage.Irish, "menu.downloadAll")] = "Íoslódáil gach rud (saor + íoctha)",
        [(AppLanguage.Irish, "menu.createCustomAudio")] = "Cruthaigh fuaim shaincheaptha (ionduchtú + hipnóis)",
        [(AppLanguage.Irish, "menu.createLibraryArchive")] = "Cruthaigh cartlann ZIP (Free + Paid + Custom)",
        [(AppLanguage.Irish, "menu.showPaths")] = "Taispeáin cosáin (ceol agus cookies)",
        [(AppLanguage.Irish, "menu.importCookies")] = "Iompórtáil cookies ón mbrabhsálaí",
        [(AppLanguage.Irish, "menu.configureParallel")] = "Socraigh comhthreomhaireacht íoslódála",
        [(AppLanguage.Irish, "menu.changeLanguage")] = "Athraigh teanga",
        [(AppLanguage.Irish, "menu.exit")] = "Scoir",
        [(AppLanguage.Irish, "menu.parallelPrompt")] = "Íoslódálacha comhthreomhara",
        [(AppLanguage.Irish, "menu.parallelError")] = "Cuir isteach uimhir ó 1 go 16.",
        [(AppLanguage.Irish, "menu.set")] = "Socraithe: {0}",
        [(AppLanguage.Irish, "menu.goodbye")] = "Slán.",
        [(AppLanguage.Irish, "menu.return")] = "Fill ar an roghchlár?",
        [(AppLanguage.Irish, "menu.main")] = "príomh-roghchlár",
        [(AppLanguage.Irish, "menu.browserManual")] = "Sonraigh brabhsálaí de láimh?",
        [(AppLanguage.Irish, "menu.browserAutoHint")] = "mura ndéantar, bainfear triail as na brabhsálaithe atá ar fáil go huathoibríoch",
        [(AppLanguage.Irish, "menu.browser")] = "Brabhsálaí",
        [(AppLanguage.Irish, "menu.language")] = "Teanga",
        [(AppLanguage.Irish, "menu.languageSet")] = "Socraíodh an teanga go {0}.",
        [(AppLanguage.Irish, "action.noScope")] = "Níor roghnaíodh raon íoslódála.",
        [(AppLanguage.Irish, "action.interrupted")] = "Cuireadh an íoslódáil ar ceal.",
        [(AppLanguage.Irish, "action.networkHint")] = "Is minic gur fadhb shealadach líonra/TLS ar macOS í seo. Rith an íoslódáil arís.",
        [(AppLanguage.Irish, "action.music")] = "Ceol",
        [(AppLanguage.Irish, "action.cookies")] = "Cookies",
        [(AppLanguage.Irish, "action.custom")] = "Saincheaptha",
        [(AppLanguage.Irish, "action.importingCookies")] = "Ag iompórtáil cookies trí yt-dlp...",
        [(AppLanguage.Irish, "action.tryingBrowsers")] = "Ag baint triail as brabhsálaithe atá ar fáil go huathoibríoch...",
        [(AppLanguage.Irish, "action.doneBrowserFile")] = "Déanta. Brabhsálaí: {0}, comhad: {1}",
        [(AppLanguage.Irish, "action.ytDlpInstalled")] = "Suiteáladh yt-dlp trí Homebrew.",
        [(AppLanguage.Irish, "action.cookieImportFailed")] = "Theip ar iompórtáil cookies.",
        [(AppLanguage.Irish, "action.cookieAccessHeader")] = "Rochtain ar Cookies Brabhsálaí",
        [(AppLanguage.Irish, "action.filePath")] = "Cosán comhaid",
        [(AppLanguage.Irish, "action.authHeader")] = "Fíordheimhniú Brabhsálaí",
        [(AppLanguage.Irish, "action.loginHelp")] =
            "1. Sínigh isteach i https://mistresscalia.com trí Patreon i Safari (nó Chrome/Firefox).\n" +
            "2. Roghnaigh iompórtáil cookies brabhsálaí sa roghchlár. Déanfaidh an aip:\n" +
            "   - yt-dlp a shuiteáil trí brew más gá;\n" +
            "   - cookies a easpórtáil chuig {0}\n" +
            "3. Ar macOS, b'fhéidir go mbeidh Full Disk Access de dhíth ar Terminal/Rider\n" +
            "   (System Settings -> Privacy & Security).\n" +
            "4. Tosaigh íoslódáil íoctha nó iomlán.\n\n" +
            "Ní theastaíonn cookies ó chomhaid saor in aisce.",
        [(AppLanguage.Irish, "action.freeCount")] = "Saor: {0} post",
        [(AppLanguage.Irish, "action.paidCount")] = "Íoctha: {0} post",
        [(AppLanguage.Irish, "custom.notEnoughFiles")] = "Tá ar a laghad dhá chomhad MP3 íoslódáilte ag teastáil.",
        [(AppLanguage.Irish, "custom.selectInduction")] = "Roghnaigh comhad ionduchtaithe",
        [(AppLanguage.Irish, "custom.selectMain")] = "Roghnaigh príomhchomhad hipnóis",
        [(AppLanguage.Irish, "custom.sameFileConfirm")] = "Roghnaigh tú an comhad céanna faoi dhó. Lean ar aghaidh?",
        [(AppLanguage.Irish, "custom.cancelled")] = "Cealaíodh cruthú na fuaime saincheaptha.",
        [(AppLanguage.Irish, "custom.created")] = "Cruthaíodh fuaim shaincheaptha: {0}",
        [(AppLanguage.Irish, "custom.failed")] = "Theip ar fhuaim shaincheaptha a chruthú: {0}",
        [(AppLanguage.Irish, "archive.usePassword")] = "Cosain an ZIP le pasfhocal?",
        [(AppLanguage.Irish, "archive.password")] = "Pasfhocal ZIP (fág folamh mura dteastaíonn pasfhocal)",
        [(AppLanguage.Irish, "archive.creating")] = "Ag cruthú cartlainne...",
        [(AppLanguage.Irish, "archive.createdTitle")] = "Cruthaíodh an chartlann",
        [(AppLanguage.Irish, "archive.created")] = "Cruthaíodh an chartlann: {0}",
        [(AppLanguage.Irish, "archive.failed")] = "Theip ar chartlann a chruthú: {0}",
        [(AppLanguage.Irish, "archive.noContent")] = "Níl aon ábhar le cartlannú i Free, Paid, nó Custom.",
        [(AppLanguage.Irish, "archive.path")] = "Cosán",
        [(AppLanguage.Irish, "archive.size")] = "Méid",
        [(AppLanguage.Irish, "archive.passwordProtected")] = "Cosanta le pasfhocal",
        [(AppLanguage.Irish, "common.yes")] = "Tá",
        [(AppLanguage.Irish, "common.no")] = "Níl",
        [(AppLanguage.Irish, "download.checkCookies")] = "Ag seiceáil seisiún Patreon (cookies)...",
        [(AppLanguage.Irish, "download.queue")] = "Sa scuaine le híoslódáil: {0} comhad (ar diosca cheana: {1}, faoi ghlas: {2}).",
        [(AppLanguage.Irish, "download.downloading")] = "Ag íoslódáil {0} comhad (comhthreomhaireacht: {1})...",
        [(AppLanguage.Irish, "download.fileExists")] = "tá an comhad ann cheana",
        [(AppLanguage.Irish, "download.failed")] = "Theip ar \"{0}\" a íoslódáil",
        [(AppLanguage.Irish, "download.scanningFree")] = "Ag scanadh post saor in aisce...",
        [(AppLanguage.Irish, "download.scanningPaid")] = "Ag scanadh post íoctha...",
        [(AppLanguage.Irish, "download.readFailed")] = "Theip ar léamh ({0}/{1}): {2}",
        [(AppLanguage.Irish, "download.validatorScope")] = "Sonraigh --free, --paid, nó --all.",
        [(AppLanguage.Irish, "progress.discovery")] = "Aimsiú",
        [(AppLanguage.Irish, "progress.metric")] = "Méadrach",
        [(AppLanguage.Irish, "progress.value")] = "Luach",
        [(AppLanguage.Irish, "progress.found")] = "Aimsithe ar an suíomh",
        [(AppLanguage.Irish, "progress.downloaded")] = "Íoslódáilte",
        [(AppLanguage.Irish, "progress.skipped")] = "Scipeáilte (ann cheana)",
        [(AppLanguage.Irish, "progress.locked")] = "Faoi ghlas (gan rochtain)",
        [(AppLanguage.Irish, "progress.errors")] = "Earráidí",
        [(AppLanguage.Irish, "cookies.ytDlpMissing")] = "Níor aimsíodh yt-dlp agus níl suiteáil uathoibríoch ar fáil.\nSuiteáil é le do bhainisteoir pacáistí nó íoslódáil ó:\nhttps://github.com/yt-dlp/yt-dlp",
        [(AppLanguage.Irish, "cookies.noCookiesForHost")] = "Brabhsálaí \"{0}\": níor aimsíodh cookies do {1}.\nSínigh isteach trí Patreon sa bhrabhsálaí seo agus iompórtáil cookies arís.",
        [(AppLanguage.Irish, "cookies.browserError")] = "Brabhsálaí \"{0}\": {1}",
        [(AppLanguage.Irish, "cookies.exportFailed")] = "Theip ar easpórtáil cookies.",
        [(AppLanguage.Irish, "cookies.permissionMac")] = "Chuir macOS bac ar rochtain ar cookies brabhsálaí (Operation not permitted). Ní sheachnaíonn sudo é seo.",
        [(AppLanguage.Irish, "cookies.permissionOther")] = "Níorbh fhéidir bunachar cookies an bhrabhsálaí a léamh. Seiceáil ceadanna an bhrabhsálaí agus na gcomhad.",
        [(AppLanguage.Irish, "cookies.lastError")] = "Earráid dheireanach:",
        [(AppLanguage.Irish, "cookies.permissionHelpMac")] =
            "1. Oscail System Settings -> Privacy & Security -> Full Disk Access.\n" +
            "2. Cumasaigh Terminal (nó Rider / iTerm / an aip a sheolann dotnet run).\n" +
            "3. Atosaigh an teirminéal agus iompórtáil cookies arís.\n" +
            "4. Caithfidh Safari a bheith sínithe isteach i mistresscalia.com trí Patreon.\n\n" +
            "Mura féidir Safari a rochtain fós, sínigh isteach le Firefox/Chrome agus iompórtáil ó firefox nó chrome.",
        [(AppLanguage.Irish, "cookies.permissionHelpOther")] =
            "1. Dún an brabhsálaí sula n-iompórtálann tú cookies, ansin bain triail eile as.\n" +
            "2. Cinntigh gur féidir leis an aip a sheolann dotnet run próifíl an bhrabhsálaí a léamh.\n" +
            "3. Sínigh isteach i mistresscalia.com trí Patreon sula n-iompórtálann tú cookies.",
        [(AppLanguage.Irish, "cookies.importFailed")] = "Theip ar iompórtáil cookies.",
        [(AppLanguage.Irish, "cookies.fileMissing")] = "Níor aimsíodh comhad cookies: {0}\n\nRoghnaigh iompórtáil cookies brabhsálaí sa roghchlár nó rith:\n  dotnet run --project src/ForgiveMeCalia.Cli -- cookies import",
        [(AppLanguage.Irish, "cookies.fileEmpty")] = "Tá comhad cookies folamh: {0}\nIompórtáil cookies brabhsálaí arís.",
        [(AppLanguage.Irish, "errors.httpRetry")] = "Theip ar athiarracht HTTP gan eisceacht.",
        [(AppLanguage.Irish, "errors.mp3Required")] = "Tá MP3 URL ag teastáil.",

        // Korean
        [(AppLanguage.Korean, "app.subtitle")] = "mistresscalia.com 오디오 다운로더",
        [(AppLanguage.Korean, "menu.title")] = "작업 선택:",
        [(AppLanguage.Korean, "menu.downloadFree")] = "무료 파일 다운로드",
        [(AppLanguage.Korean, "menu.downloadPaid")] = "유료 파일 다운로드 (cookies 필요)",
        [(AppLanguage.Korean, "menu.downloadAll")] = "모두 다운로드 (무료 + 유료)",
        [(AppLanguage.Korean, "menu.createCustomAudio")] = "커스텀 오디오 만들기 (인덕션 + 최면)",
        [(AppLanguage.Korean, "menu.createLibraryArchive")] = "ZIP 아카이브 만들기 (Free + Paid + Custom)",
        [(AppLanguage.Korean, "menu.showPaths")] = "경로 보기 (음악 및 cookies)",
        [(AppLanguage.Korean, "menu.importCookies")] = "브라우저 cookies 가져오기",
        [(AppLanguage.Korean, "menu.configureParallel")] = "병렬 다운로드 설정",
        [(AppLanguage.Korean, "menu.changeLanguage")] = "언어 변경",
        [(AppLanguage.Korean, "menu.exit")] = "종료",
        [(AppLanguage.Korean, "menu.parallelPrompt")] = "병렬 다운로드 수",
        [(AppLanguage.Korean, "menu.parallelError")] = "1에서 16 사이의 숫자를 입력하세요.",
        [(AppLanguage.Korean, "menu.set")] = "설정됨: {0}",
        [(AppLanguage.Korean, "menu.goodbye")] = "안녕히 가세요.",
        [(AppLanguage.Korean, "menu.return")] = "메뉴로 돌아갈까요?",
        [(AppLanguage.Korean, "menu.main")] = "메인 메뉴",
        [(AppLanguage.Korean, "menu.browserManual")] = "브라우저를 직접 지정할까요?",
        [(AppLanguage.Korean, "menu.browserAutoHint")] = "아니면 사용 가능한 브라우저를 자동으로 시도합니다",
        [(AppLanguage.Korean, "menu.browser")] = "브라우저",
        [(AppLanguage.Korean, "menu.language")] = "언어",
        [(AppLanguage.Korean, "menu.languageSet")] = "언어가 {0}(으)로 설정되었습니다.",
        [(AppLanguage.Korean, "action.noScope")] = "다운로드 범위가 선택되지 않았습니다.",
        [(AppLanguage.Korean, "action.interrupted")] = "다운로드가 중단되었습니다.",
        [(AppLanguage.Korean, "action.networkHint")] = "macOS의 일시적인 네트워크/TLS 문제인 경우가 많습니다. 다운로드를 다시 실행하세요.",
        [(AppLanguage.Korean, "action.music")] = "음악",
        [(AppLanguage.Korean, "action.cookies")] = "Cookies",
        [(AppLanguage.Korean, "action.custom")] = "커스텀",
        [(AppLanguage.Korean, "action.importingCookies")] = "yt-dlp로 cookies를 가져오는 중...",
        [(AppLanguage.Korean, "action.tryingBrowsers")] = "사용 가능한 브라우저를 자동으로 시도하는 중...",
        [(AppLanguage.Korean, "action.doneBrowserFile")] = "완료. 브라우저: {0}, 파일: {1}",
        [(AppLanguage.Korean, "action.ytDlpInstalled")] = "yt-dlp가 Homebrew로 설치되었습니다.",
        [(AppLanguage.Korean, "action.cookieImportFailed")] = "cookies 가져오기에 실패했습니다.",
        [(AppLanguage.Korean, "action.cookieAccessHeader")] = "브라우저 Cookie 접근",
        [(AppLanguage.Korean, "action.filePath")] = "파일 경로",
        [(AppLanguage.Korean, "action.authHeader")] = "브라우저 인증",
        [(AppLanguage.Korean, "action.loginHelp")] =
            "1. Safari(또는 Chrome/Firefox)에서 Patreon을 통해 https://mistresscalia.com 에 로그인하세요.\n" +
            "2. 메뉴에서 브라우저 cookies 가져오기를 선택하세요. 앱은 다음을 수행합니다:\n" +
            "   - 필요한 경우 brew로 yt-dlp 설치;\n" +
            "   - cookies를 {0} 로 내보내기\n" +
            "3. macOS에서는 Terminal/Rider에 Full Disk Access가 필요할 수 있습니다\n" +
            "   (System Settings -> Privacy & Security).\n" +
            "4. 유료 또는 전체 다운로드를 시작하세요.\n\n" +
            "무료 파일에는 cookies가 필요하지 않습니다.",
        [(AppLanguage.Korean, "action.freeCount")] = "무료: {0}개 게시물",
        [(AppLanguage.Korean, "action.paidCount")] = "유료: {0}개 게시물",
        [(AppLanguage.Korean, "custom.notEnoughFiles")] = "다운로드된 MP3 파일이 최소 두 개 필요합니다.",
        [(AppLanguage.Korean, "custom.selectInduction")] = "인덕션 파일 선택",
        [(AppLanguage.Korean, "custom.selectMain")] = "메인 최면 파일 선택",
        [(AppLanguage.Korean, "custom.sameFileConfirm")] = "같은 파일을 두 번 선택했습니다. 계속할까요?",
        [(AppLanguage.Korean, "custom.cancelled")] = "커스텀 오디오 생성이 취소되었습니다.",
        [(AppLanguage.Korean, "custom.created")] = "커스텀 오디오 생성됨: {0}",
        [(AppLanguage.Korean, "custom.failed")] = "커스텀 오디오 생성 실패: {0}",
        [(AppLanguage.Korean, "archive.usePassword")] = "ZIP을 비밀번호로 보호할까요?",
        [(AppLanguage.Korean, "archive.password")] = "ZIP 비밀번호 (비밀번호 없음은 비워두기)",
        [(AppLanguage.Korean, "archive.creating")] = "아카이브 생성 중...",
        [(AppLanguage.Korean, "archive.createdTitle")] = "아카이브 생성됨",
        [(AppLanguage.Korean, "archive.created")] = "아카이브 생성됨: {0}",
        [(AppLanguage.Korean, "archive.failed")] = "아카이브 생성 실패: {0}",
        [(AppLanguage.Korean, "archive.noContent")] = "Free, Paid 또는 Custom에 압축할 내용이 없습니다.",
        [(AppLanguage.Korean, "archive.path")] = "경로",
        [(AppLanguage.Korean, "archive.size")] = "크기",
        [(AppLanguage.Korean, "archive.passwordProtected")] = "비밀번호 보호",
        [(AppLanguage.Korean, "common.yes")] = "예",
        [(AppLanguage.Korean, "common.no")] = "아니요",
        [(AppLanguage.Korean, "download.checkCookies")] = "Patreon 세션 확인 중 (cookies)...",
        [(AppLanguage.Korean, "download.queue")] = "다운로드 대기: {0}개 파일 (이미 디스크에 있음: {1}, 잠김: {2}).",
        [(AppLanguage.Korean, "download.downloading")] = "{0}개 파일 다운로드 중 (병렬: {1})...",
        [(AppLanguage.Korean, "download.fileExists")] = "파일이 이미 있습니다",
        [(AppLanguage.Korean, "download.failed")] = "\"{0}\" 다운로드 실패",
        [(AppLanguage.Korean, "download.scanningFree")] = "무료 게시물 스캔 중...",
        [(AppLanguage.Korean, "download.scanningPaid")] = "유료 게시물 스캔 중...",
        [(AppLanguage.Korean, "download.readFailed")] = "읽기 실패 ({0}/{1}): {2}",
        [(AppLanguage.Korean, "download.validatorScope")] = "--free, --paid 또는 --all을 지정하세요.",
        [(AppLanguage.Korean, "progress.discovery")] = "검색",
        [(AppLanguage.Korean, "progress.metric")] = "항목",
        [(AppLanguage.Korean, "progress.value")] = "값",
        [(AppLanguage.Korean, "progress.found")] = "사이트에서 찾음",
        [(AppLanguage.Korean, "progress.downloaded")] = "다운로드됨",
        [(AppLanguage.Korean, "progress.skipped")] = "건너뜀 (이미 있음)",
        [(AppLanguage.Korean, "progress.locked")] = "잠김 (접근 불가)",
        [(AppLanguage.Korean, "progress.errors")] = "오류",
        [(AppLanguage.Korean, "cookies.ytDlpMissing")] = "yt-dlp를 찾을 수 없고 자동 설치도 사용할 수 없습니다.\n패키지 관리자로 설치하거나 다음에서 다운로드하세요:\nhttps://github.com/yt-dlp/yt-dlp",
        [(AppLanguage.Korean, "cookies.noCookiesForHost")] = "브라우저 \"{0}\": {1}의 cookies를 찾을 수 없습니다.\n이 브라우저에서 Patreon으로 사이트에 로그인한 뒤 다시 가져오세요.",
        [(AppLanguage.Korean, "cookies.browserError")] = "브라우저 \"{0}\": {1}",
        [(AppLanguage.Korean, "cookies.exportFailed")] = "cookies 내보내기에 실패했습니다.",
        [(AppLanguage.Korean, "cookies.permissionMac")] = "macOS가 브라우저 cookies 접근을 차단했습니다 (Operation not permitted). sudo로 우회할 수 없습니다.",
        [(AppLanguage.Korean, "cookies.permissionOther")] = "브라우저 cookie 데이터베이스를 읽을 수 없습니다. 브라우저 및 파일 권한을 확인하세요.",
        [(AppLanguage.Korean, "cookies.lastError")] = "마지막 오류:",
        [(AppLanguage.Korean, "cookies.permissionHelpMac")] =
            "1. System Settings -> Privacy & Security -> Full Disk Access를 엽니다.\n" +
            "2. Terminal(또는 Rider / iTerm / dotnet run을 실행하는 앱)을 허용합니다.\n" +
            "3. 터미널을 다시 시작하고 cookies를 다시 가져옵니다.\n" +
            "4. Safari에서 Patreon을 통해 mistresscalia.com에 로그인되어 있어야 합니다.\n\n" +
            "권한을 부여해도 Safari 접근이 안 되면 Firefox/Chrome으로 로그인하고 firefox 또는 chrome에서 가져오세요.",
        [(AppLanguage.Korean, "cookies.permissionHelpOther")] =
            "1. cookies를 가져오기 전에 브라우저를 닫고 다시 시도하세요.\n" +
            "2. dotnet run을 실행하는 앱이 브라우저 프로필을 읽을 수 있는지 확인하세요.\n" +
            "3. cookies를 가져오기 전에 Patreon을 통해 mistresscalia.com에 로그인하세요.",
        [(AppLanguage.Korean, "cookies.importFailed")] = "cookies 가져오기에 실패했습니다.",
        [(AppLanguage.Korean, "cookies.fileMissing")] = "Cookie 파일을 찾을 수 없습니다: {0}\n\n메뉴에서 브라우저 cookies 가져오기를 선택하거나 실행하세요:\n  dotnet run --project src/ForgiveMeCalia.Cli -- cookies import",
        [(AppLanguage.Korean, "cookies.fileEmpty")] = "Cookie 파일이 비어 있습니다: {0}\n브라우저 cookies를 다시 가져오세요.",
        [(AppLanguage.Korean, "errors.httpRetry")] = "예외 없이 HTTP 재시도가 실패했습니다.",
        [(AppLanguage.Korean, "errors.mp3Required")] = "MP3 URL이 필요합니다.",

        // Japanese
        [(AppLanguage.Japanese, "app.subtitle")] = "mistresscalia.com 用オーディオダウンローダー",
        [(AppLanguage.Japanese, "menu.title")] = "操作を選択:",
        [(AppLanguage.Japanese, "menu.downloadFree")] = "無料ファイルをダウンロード",
        [(AppLanguage.Japanese, "menu.downloadPaid")] = "有料ファイルをダウンロード (cookies が必要)",
        [(AppLanguage.Japanese, "menu.downloadAll")] = "すべてをダウンロード (無料 + 有料)",
        [(AppLanguage.Japanese, "menu.createCustomAudio")] = "カスタム音声を作成 (導入 + 催眠)",
        [(AppLanguage.Japanese, "menu.createLibraryArchive")] = "ZIP アーカイブを作成 (Free + Paid + Custom)",
        [(AppLanguage.Japanese, "menu.showPaths")] = "パスを表示 (音楽と cookies)",
        [(AppLanguage.Japanese, "menu.importCookies")] = "ブラウザー cookies をインポート",
        [(AppLanguage.Japanese, "menu.configureParallel")] = "並列ダウンロード数を設定",
        [(AppLanguage.Japanese, "menu.changeLanguage")] = "言語を変更",
        [(AppLanguage.Japanese, "menu.exit")] = "終了",
        [(AppLanguage.Japanese, "menu.parallelPrompt")] = "並列ダウンロード数",
        [(AppLanguage.Japanese, "menu.parallelError")] = "1 から 16 の数値を入力してください。",
        [(AppLanguage.Japanese, "menu.set")] = "設定しました: {0}",
        [(AppLanguage.Japanese, "menu.goodbye")] = "終了します。",
        [(AppLanguage.Japanese, "menu.return")] = "メニューに戻りますか?",
        [(AppLanguage.Japanese, "menu.main")] = "メインメニュー",
        [(AppLanguage.Japanese, "menu.browserManual")] = "ブラウザーを手動で指定しますか?",
        [(AppLanguage.Japanese, "menu.browserAutoHint")] = "指定しない場合は利用可能なブラウザーを自動的に試します",
        [(AppLanguage.Japanese, "menu.browser")] = "ブラウザー",
        [(AppLanguage.Japanese, "menu.language")] = "言語",
        [(AppLanguage.Japanese, "menu.languageSet")] = "言語を {0} に設定しました。",
        [(AppLanguage.Japanese, "action.noScope")] = "ダウンロード範囲が選択されていません。",
        [(AppLanguage.Japanese, "action.interrupted")] = "ダウンロードが中断されました。",
        [(AppLanguage.Japanese, "action.networkHint")] = "macOS の一時的なネットワーク/TLS 問題であることがよくあります。もう一度実行してください。",
        [(AppLanguage.Japanese, "action.music")] = "音楽",
        [(AppLanguage.Japanese, "action.cookies")] = "Cookies",
        [(AppLanguage.Japanese, "action.custom")] = "カスタム",
        [(AppLanguage.Japanese, "action.importingCookies")] = "yt-dlp 経由で cookies をインポートしています...",
        [(AppLanguage.Japanese, "action.tryingBrowsers")] = "利用可能なブラウザーを自動的に試しています...",
        [(AppLanguage.Japanese, "action.doneBrowserFile")] = "完了。ブラウザー: {0}, ファイル: {1}",
        [(AppLanguage.Japanese, "action.ytDlpInstalled")] = "yt-dlp を Homebrew でインストールしました。",
        [(AppLanguage.Japanese, "action.cookieImportFailed")] = "cookies のインポートに失敗しました。",
        [(AppLanguage.Japanese, "action.cookieAccessHeader")] = "ブラウザー Cookie アクセス",
        [(AppLanguage.Japanese, "action.filePath")] = "ファイルパス",
        [(AppLanguage.Japanese, "action.authHeader")] = "ブラウザー認証",
        [(AppLanguage.Japanese, "action.loginHelp")] =
            "1. Safari (または Chrome/Firefox) で Patreon 経由で https://mistresscalia.com にサインインします。\n" +
            "2. メニューでブラウザー cookies のインポートを選択します。アプリは次を行います:\n" +
            "   - 必要に応じて brew で yt-dlp をインストール;\n" +
            "   - cookies を {0} にエクスポート\n" +
            "3. macOS では Terminal/Rider に Full Disk Access が必要な場合があります\n" +
            "   (System Settings -> Privacy & Security)。\n" +
            "4. 有料または全体ダウンロードを開始します。\n\n" +
            "無料ファイルには cookies は不要です。",
        [(AppLanguage.Japanese, "action.freeCount")] = "無料: {0} 件",
        [(AppLanguage.Japanese, "action.paidCount")] = "有料: {0} 件",
        [(AppLanguage.Japanese, "custom.notEnoughFiles")] = "ダウンロード済み MP3 ファイルが少なくとも 2 つ必要です。",
        [(AppLanguage.Japanese, "custom.selectInduction")] = "導入ファイルを選択",
        [(AppLanguage.Japanese, "custom.selectMain")] = "メイン催眠ファイルを選択",
        [(AppLanguage.Japanese, "custom.sameFileConfirm")] = "同じファイルを 2 回選択しました。続行しますか?",
        [(AppLanguage.Japanese, "custom.cancelled")] = "カスタム音声の作成をキャンセルしました。",
        [(AppLanguage.Japanese, "custom.created")] = "カスタム音声を作成しました: {0}",
        [(AppLanguage.Japanese, "custom.failed")] = "カスタム音声の作成に失敗しました: {0}",
        [(AppLanguage.Japanese, "archive.usePassword")] = "ZIP をパスワードで保護しますか?",
        [(AppLanguage.Japanese, "archive.password")] = "ZIP パスワード (不要なら空のまま)",
        [(AppLanguage.Japanese, "archive.creating")] = "アーカイブを作成中...",
        [(AppLanguage.Japanese, "archive.createdTitle")] = "アーカイブを作成しました",
        [(AppLanguage.Japanese, "archive.created")] = "アーカイブを作成しました: {0}",
        [(AppLanguage.Japanese, "archive.failed")] = "アーカイブの作成に失敗しました: {0}",
        [(AppLanguage.Japanese, "archive.noContent")] = "Free、Paid、Custom にアーカイブする内容がありません。",
        [(AppLanguage.Japanese, "archive.path")] = "パス",
        [(AppLanguage.Japanese, "archive.size")] = "サイズ",
        [(AppLanguage.Japanese, "archive.passwordProtected")] = "パスワード保護",
        [(AppLanguage.Japanese, "common.yes")] = "はい",
        [(AppLanguage.Japanese, "common.no")] = "いいえ",
        [(AppLanguage.Japanese, "download.checkCookies")] = "Patreon セッションを確認しています (cookies)...",
        [(AppLanguage.Japanese, "download.queue")] = "ダウンロード待ち: {0} ファイル (既にディスク上: {1}, ロック: {2})。",
        [(AppLanguage.Japanese, "download.downloading")] = "{0} ファイルをダウンロード中 (並列数: {1})...",
        [(AppLanguage.Japanese, "download.fileExists")] = "ファイルは既に存在します",
        [(AppLanguage.Japanese, "download.failed")] = "\"{0}\" のダウンロードに失敗しました",
        [(AppLanguage.Japanese, "download.scanningFree")] = "無料投稿をスキャン中...",
        [(AppLanguage.Japanese, "download.scanningPaid")] = "有料投稿をスキャン中...",
        [(AppLanguage.Japanese, "download.readFailed")] = "読み取りに失敗 ({0}/{1}): {2}",
        [(AppLanguage.Japanese, "download.validatorScope")] = "--free、--paid、または --all を指定してください。",
        [(AppLanguage.Japanese, "progress.discovery")] = "検出",
        [(AppLanguage.Japanese, "progress.metric")] = "項目",
        [(AppLanguage.Japanese, "progress.value")] = "値",
        [(AppLanguage.Japanese, "progress.found")] = "サイト上で検出",
        [(AppLanguage.Japanese, "progress.downloaded")] = "ダウンロード済み",
        [(AppLanguage.Japanese, "progress.skipped")] = "スキップ (既に存在)",
        [(AppLanguage.Japanese, "progress.locked")] = "ロック (アクセスなし)",
        [(AppLanguage.Japanese, "progress.errors")] = "エラー",
        [(AppLanguage.Japanese, "cookies.ytDlpMissing")] = "yt-dlp が見つからず、自動インストールも利用できません。\nパッケージマネージャーでインストールするか、次からダウンロードしてください:\nhttps://github.com/yt-dlp/yt-dlp",
        [(AppLanguage.Japanese, "cookies.noCookiesForHost")] = "ブラウザー \"{0}\": {1} の cookies が見つかりません。\nこのブラウザーで Patreon 経由でサイトにサインインし、再度インポートしてください。",
        [(AppLanguage.Japanese, "cookies.browserError")] = "ブラウザー \"{0}\": {1}",
        [(AppLanguage.Japanese, "cookies.exportFailed")] = "cookies のエクスポートに失敗しました。",
        [(AppLanguage.Japanese, "cookies.permissionMac")] = "macOS がブラウザー cookies へのアクセスをブロックしました (Operation not permitted)。sudo では回避できません。",
        [(AppLanguage.Japanese, "cookies.permissionOther")] = "ブラウザーの cookie データベースを読み取れません。ブラウザーとファイルの権限を確認してください。",
        [(AppLanguage.Japanese, "cookies.lastError")] = "最後のエラー:",
        [(AppLanguage.Japanese, "cookies.permissionHelpMac")] =
            "1. System Settings -> Privacy & Security -> Full Disk Access を開きます。\n" +
            "2. Terminal (または Rider / iTerm / dotnet run を起動するアプリ) を許可します。\n" +
            "3. ターミナルを再起動して cookies を再度インポートします。\n" +
            "4. Safari で Patreon 経由で mistresscalia.com にサインインしている必要があります。\n\n" +
            "権限を付与しても Safari にアクセスできない場合は、Firefox/Chrome でサインインして firefox または chrome からインポートしてください。",
        [(AppLanguage.Japanese, "cookies.permissionHelpOther")] =
            "1. cookies をインポートする前にブラウザーを閉じて、もう一度試してください。\n" +
            "2. dotnet run を起動するアプリがブラウザープロファイルを読めることを確認してください。\n" +
            "3. cookies をインポートする前に Patreon 経由で mistresscalia.com にサインインしてください。",
        [(AppLanguage.Japanese, "cookies.importFailed")] = "cookies のインポートに失敗しました。",
        [(AppLanguage.Japanese, "cookies.fileMissing")] = "Cookie ファイルが見つかりません: {0}\n\nメニューでブラウザー cookies のインポートを選択するか、次を実行してください:\n  dotnet run --project src/ForgiveMeCalia.Cli -- cookies import",
        [(AppLanguage.Japanese, "cookies.fileEmpty")] = "Cookie ファイルが空です: {0}\nブラウザー cookies を再度インポートしてください。",
        [(AppLanguage.Japanese, "errors.httpRetry")] = "例外なしで HTTP リトライに失敗しました。",
        [(AppLanguage.Japanese, "errors.mp3Required")] = "MP3 URL が必要です。",

        // Uzbek
        [(AppLanguage.Uzbek, "app.subtitle")] = "mistresscalia.com uchun audio yuklab oluvchi",
        [(AppLanguage.Uzbek, "menu.title")] = "Amalni tanlang:",
        [(AppLanguage.Uzbek, "menu.downloadFree")] = "Bepul fayllarni yuklab olish",
        [(AppLanguage.Uzbek, "menu.downloadPaid")] = "Pullik fayllarni yuklab olish (cookies kerak)",
        [(AppLanguage.Uzbek, "menu.downloadAll")] = "Hammasini yuklab olish (bepul + pullik)",
        [(AppLanguage.Uzbek, "menu.createCustomAudio")] = "Maxsus audio yaratish (induksiya + gipnoz)",
        [(AppLanguage.Uzbek, "menu.createLibraryArchive")] = "ZIP arxiv yaratish (Free + Paid + Custom)",
        [(AppLanguage.Uzbek, "menu.showPaths")] = "Yo'llarni ko'rsatish (musiqa va cookies)",
        [(AppLanguage.Uzbek, "menu.importCookies")] = "Brauzer cookies fayllarini import qilish",
        [(AppLanguage.Uzbek, "menu.configureParallel")] = "Parallel yuklab olishni sozlash",
        [(AppLanguage.Uzbek, "menu.changeLanguage")] = "Tilni o'zgartirish",
        [(AppLanguage.Uzbek, "menu.exit")] = "Chiqish",
        [(AppLanguage.Uzbek, "menu.parallelPrompt")] = "Parallel yuklab olishlar",
        [(AppLanguage.Uzbek, "menu.parallelError")] = "1 dan 16 gacha son kiriting.",
        [(AppLanguage.Uzbek, "menu.set")] = "O'rnatildi: {0}",
        [(AppLanguage.Uzbek, "menu.goodbye")] = "Xayr.",
        [(AppLanguage.Uzbek, "menu.return")] = "Menyuga qaytilsinmi?",
        [(AppLanguage.Uzbek, "menu.main")] = "asosiy menyu",
        [(AppLanguage.Uzbek, "menu.browserManual")] = "Brauzerni qo'lda ko'rsatasizmi?",
        [(AppLanguage.Uzbek, "menu.browserAutoHint")] = "aks holda mavjud brauzerlar avtomatik sinab ko'riladi",
        [(AppLanguage.Uzbek, "menu.browser")] = "Brauzer",
        [(AppLanguage.Uzbek, "menu.language")] = "Til",
        [(AppLanguage.Uzbek, "menu.languageSet")] = "Til {0} ga o'zgartirildi.",
        [(AppLanguage.Uzbek, "action.noScope")] = "Yuklab olish turi tanlanmagan.",
        [(AppLanguage.Uzbek, "action.interrupted")] = "Yuklab olish to'xtatildi.",
        [(AppLanguage.Uzbek, "action.networkHint")] = "Bu ko'pincha macOS dagi vaqtinchalik tarmoq/TLS muammosi. Yuklab olishni qayta ishga tushiring.",
        [(AppLanguage.Uzbek, "action.music")] = "Musiqa",
        [(AppLanguage.Uzbek, "action.cookies")] = "Cookies",
        [(AppLanguage.Uzbek, "action.custom")] = "Maxsus",
        [(AppLanguage.Uzbek, "action.importingCookies")] = "yt-dlp orqali cookies import qilinmoqda...",
        [(AppLanguage.Uzbek, "action.tryingBrowsers")] = "Mavjud brauzerlar avtomatik sinab ko'rilmoqda...",
        [(AppLanguage.Uzbek, "action.doneBrowserFile")] = "Tayyor. Brauzer: {0}, fayl: {1}",
        [(AppLanguage.Uzbek, "action.ytDlpInstalled")] = "yt-dlp Homebrew orqali o'rnatildi.",
        [(AppLanguage.Uzbek, "action.cookieImportFailed")] = "Cookies import qilib bo'lmadi.",
        [(AppLanguage.Uzbek, "action.cookieAccessHeader")] = "Brauzer cookies ruxsati",
        [(AppLanguage.Uzbek, "action.filePath")] = "Fayl yo'li",
        [(AppLanguage.Uzbek, "action.authHeader")] = "Brauzer autentifikatsiyasi",
        [(AppLanguage.Uzbek, "action.loginHelp")] =
            "1. Safari (yoki Chrome/Firefox) da Patreon orqali https://mistresscalia.com saytiga kiring.\n" +
            "2. Menyuda brauzer cookies importini tanlang. Ilova:\n" +
            "   - kerak bo'lsa, brew orqali yt-dlp o'rnatadi;\n" +
            "   - cookies fayllarini {0} ga eksport qiladi\n" +
            "3. macOS da Terminal/Rider uchun Full Disk Access kerak bo'lishi mumkin\n" +
            "   (System Settings -> Privacy & Security).\n" +
            "4. Pullik yoki to'liq yuklab olishni boshlang.\n\n" +
            "Bepul fayllar uchun cookies kerak emas.",
        [(AppLanguage.Uzbek, "action.freeCount")] = "Bepul: {0} ta post",
        [(AppLanguage.Uzbek, "action.paidCount")] = "Pullik: {0} ta post",
        [(AppLanguage.Uzbek, "custom.notEnoughFiles")] = "Kamida ikkita yuklab olingan MP3 fayl kerak.",
        [(AppLanguage.Uzbek, "custom.selectInduction")] = "Induksiya faylini tanlang",
        [(AppLanguage.Uzbek, "custom.selectMain")] = "Asosiy gipnoz faylini tanlang",
        [(AppLanguage.Uzbek, "custom.sameFileConfirm")] = "Bir xil faylni ikki marta tanladingiz. Davom etilsinmi?",
        [(AppLanguage.Uzbek, "custom.cancelled")] = "Maxsus audio yaratish bekor qilindi.",
        [(AppLanguage.Uzbek, "custom.created")] = "Maxsus audio yaratildi: {0}",
        [(AppLanguage.Uzbek, "custom.failed")] = "Maxsus audio yaratib bo'lmadi: {0}",
        [(AppLanguage.Uzbek, "archive.usePassword")] = "ZIP parol bilan himoyalansinmi?",
        [(AppLanguage.Uzbek, "archive.password")] = "ZIP paroli (parolsiz uchun bo'sh qoldiring)",
        [(AppLanguage.Uzbek, "archive.creating")] = "Arxiv yaratilmoqda...",
        [(AppLanguage.Uzbek, "archive.createdTitle")] = "Arxiv yaratildi",
        [(AppLanguage.Uzbek, "archive.created")] = "Arxiv yaratildi: {0}",
        [(AppLanguage.Uzbek, "archive.failed")] = "Arxiv yaratib bo'lmadi: {0}",
        [(AppLanguage.Uzbek, "archive.noContent")] = "Free, Paid yoki Custom ichida arxivlash uchun kontent yo'q.",
        [(AppLanguage.Uzbek, "archive.path")] = "Yo'l",
        [(AppLanguage.Uzbek, "archive.size")] = "Hajm",
        [(AppLanguage.Uzbek, "archive.passwordProtected")] = "Parol bilan himoyalangan",
        [(AppLanguage.Uzbek, "common.yes")] = "Ha",
        [(AppLanguage.Uzbek, "common.no")] = "Yo'q",
        [(AppLanguage.Uzbek, "download.checkCookies")] = "Patreon sessiyasi tekshirilmoqda (cookies)...",
        [(AppLanguage.Uzbek, "download.queue")] = "Yuklab olish navbatida: {0} fayl (diskda bor: {1}, bloklangan: {2}).",
        [(AppLanguage.Uzbek, "download.downloading")] = "{0} fayl yuklab olinmoqda (parallel: {1})...",
        [(AppLanguage.Uzbek, "download.fileExists")] = "fayl allaqachon mavjud",
        [(AppLanguage.Uzbek, "download.failed")] = "\"{0}\" yuklab olinmadi",
        [(AppLanguage.Uzbek, "download.scanningFree")] = "Bepul postlar skanerlanmoqda...",
        [(AppLanguage.Uzbek, "download.scanningPaid")] = "Pullik postlar skanerlanmoqda...",
        [(AppLanguage.Uzbek, "download.readFailed")] = "O'qib bo'lmadi ({0}/{1}): {2}",
        [(AppLanguage.Uzbek, "download.validatorScope")] = "--free, --paid yoki --all ni ko'rsating.",
        [(AppLanguage.Uzbek, "progress.discovery")] = "Topish",
        [(AppLanguage.Uzbek, "progress.metric")] = "Ko'rsatkich",
        [(AppLanguage.Uzbek, "progress.value")] = "Qiymat",
        [(AppLanguage.Uzbek, "progress.found")] = "Saytda topildi",
        [(AppLanguage.Uzbek, "progress.downloaded")] = "Yuklab olindi",
        [(AppLanguage.Uzbek, "progress.skipped")] = "O'tkazib yuborildi (allaqachon bor)",
        [(AppLanguage.Uzbek, "progress.locked")] = "Bloklangan (ruxsat yo'q)",
        [(AppLanguage.Uzbek, "progress.errors")] = "Xatolar",
        [(AppLanguage.Uzbek, "cookies.ytDlpMissing")] = "yt-dlp topilmadi va avtomatik o'rnatish mavjud emas.\nUni paket menejeri orqali o'rnating yoki bu yerdan yuklab oling:\nhttps://github.com/yt-dlp/yt-dlp",
        [(AppLanguage.Uzbek, "cookies.noCookiesForHost")] = "Brauzer \"{0}\": {1} uchun cookies topilmadi.\nShu brauzerda Patreon orqali saytga kiring va cookies ni qayta import qiling.",
        [(AppLanguage.Uzbek, "cookies.browserError")] = "Brauzer \"{0}\": {1}",
        [(AppLanguage.Uzbek, "cookies.exportFailed")] = "Cookies eksport qilib bo'lmadi.",
        [(AppLanguage.Uzbek, "cookies.permissionMac")] = "macOS brauzer cookieslariga kirishni blokladi (Operation not permitted). sudo buni chetlab o'tmaydi.",
        [(AppLanguage.Uzbek, "cookies.permissionOther")] = "Brauzer cookie bazasini o'qib bo'lmadi. Brauzer va fayl ruxsatlarini tekshiring.",
        [(AppLanguage.Uzbek, "cookies.lastError")] = "Oxirgi xato:",
        [(AppLanguage.Uzbek, "cookies.permissionHelpMac")] =
            "1. System Settings -> Privacy & Security -> Full Disk Access ni oching.\n" +
            "2. Terminal (yoki Rider / iTerm / dotnet run ishga tushiradigan ilova) uchun ruxsat bering.\n" +
            "3. Terminalni qayta ishga tushiring va cookies ni yana import qiling.\n" +
            "4. Safari Patreon orqali mistresscalia.com saytiga kirgan bo'lishi kerak.\n\n" +
            "Ruxsatdan keyin ham Safari ishlamasa, Firefox/Chrome orqali kiring va firefox yoki chrome dan import qiling.",
        [(AppLanguage.Uzbek, "cookies.permissionHelpOther")] =
            "1. Cookies importidan oldin brauzerni yoping va qayta urinib ko'ring.\n" +
            "2. dotnet run ni ishga tushiradigan ilova brauzer profilini o'qiy olishiga ishonch hosil qiling.\n" +
            "3. Cookies importidan oldin Patreon orqali mistresscalia.com saytiga kiring.",
        [(AppLanguage.Uzbek, "cookies.importFailed")] = "Cookies import qilib bo'lmadi.",
        [(AppLanguage.Uzbek, "cookies.fileMissing")] = "Cookie fayli topilmadi: {0}\n\nMenyuda brauzer cookies importini tanlang yoki ishga tushiring:\n  dotnet run --project src/ForgiveMeCalia.Cli -- cookies import",
        [(AppLanguage.Uzbek, "cookies.fileEmpty")] = "Cookie fayli bo'sh: {0}\nBrauzer cookies ni qayta import qiling.",
        [(AppLanguage.Uzbek, "errors.httpRetry")] = "HTTP qayta urinish istisnosiz muvaffaqiyatsiz tugadi.",
        [(AppLanguage.Uzbek, "errors.mp3Required")] = "MP3 URL kerak."
    };
}
