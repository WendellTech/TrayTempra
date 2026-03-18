using System.Text.Json;
using TrayTempra.Models;

namespace TrayTempra.Services;

public sealed class SettingsStorage
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _settingsPath;
    private readonly object _sync = new();

    public SettingsStorage(string appFolderPath)
    {
        Directory.CreateDirectory(appFolderPath);
        _settingsPath = Path.Combine(appFolderPath, "settings.json");
    }

    public AppSettings Load()
    {
        lock (_sync)
        {
            if (!File.Exists(_settingsPath))
            {
                var defaults = new AppSettings().Normalize();
                Save(defaults);
                return defaults;
            }

            try
            {
                var raw = File.ReadAllText(_settingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(raw) ?? new AppSettings();
                settings.Normalize();
                return settings;
            }
            catch
            {
                var fallback = new AppSettings().Normalize();
                Save(fallback);
                return fallback;
            }
        }
    }

    public void Save(AppSettings settings)
    {
        lock (_sync)
        {
            settings.Normalize();
            var json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(_settingsPath, json);
        }
    }
}
