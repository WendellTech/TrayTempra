using Microsoft.Win32;

namespace TrayTempra.Services;

public sealed class StartupManager
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private readonly string _appName;

    public StartupManager(string appName)
    {
        _appName = appName;
    }

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        var value = key?.GetValue(_appName) as string;
        return !string.IsNullOrWhiteSpace(value);
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
            ?? Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true);

        if (key is null) return;

        if (enabled)
        {
            var exePath = Environment.ProcessPath;
            if (string.IsNullOrWhiteSpace(exePath)) return;
            key.SetValue(_appName, Quote(exePath));
        }
        else
        {
            key.DeleteValue(_appName, throwOnMissingValue: false);
        }
    }

    private static string Quote(string path) => path.Contains(' ') ? $"\"{path}\"" : path;
}
