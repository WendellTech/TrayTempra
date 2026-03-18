using TrayTempra.Models;
using TrayTempra.Services;
using TrayTempra.UI;

namespace TrayTempra;

public sealed class TrayTempraAppContext : ApplicationContext
{
    private const string AppName = "TrayTempra";

    private readonly NotifyIcon _notifyIcon;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly SettingsStorage _settingsStorage;
    private readonly StartupManager _startupManager;
    private readonly ProgressPopupForm _popup;
    private readonly Icon _mainIcon;
    private readonly bool _ownsMainIcon;

    private AppSettings _settings;
    private Theme _theme;
    private bool _disposed;

    public TrayTempraAppContext()
    {
        var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);
        _settingsStorage = new SettingsStorage(appDataPath);
        _startupManager = new StartupManager(AppName);
        var loadedIcon = ResourceIcons.TryLoadMainIcon();
        _mainIcon = loadedIcon ?? SystemIcons.Application;
        _ownsMainIcon = loadedIcon is not null;

        _settings = _settingsStorage.Load();
        _settings.StartWithWindows = _startupManager.IsEnabled();
        _theme = ThemeCatalog.Resolve(_settings.ThemeId);

        _popup = new ProgressPopupForm(_theme)
        {
            Icon = _mainIcon
        };
        _popup.UpdateTheme(_theme);

        var contextMenu = BuildContextMenu();
        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            ContextMenuStrip = contextMenu,
            Icon = _mainIcon
        };

        _notifyIcon.MouseClick += OnNotifyIconMouseClick;

        _timer = new System.Windows.Forms.Timer
        {
            Interval = 1000,
            Enabled = true
        };
        _timer.Tick += (_, _) => Refresh();

        Refresh();
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _timer.Stop();
            _timer.Dispose();

            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();

            _popup.Dispose();
            if (_ownsMainIcon)
            {
                _mainIcon.Dispose();
            }
        }

        _disposed = true;
        base.Dispose(disposing);
    }

    private ContextMenuStrip BuildContextMenu()
    {
        var menu = new ContextMenuStrip();
        var openItem = new ToolStripMenuItem("Open") { Name = "Open" };
        var settingsItem = new ToolStripMenuItem("Settings") { Name = "Settings" };
        var exitItem = new ToolStripMenuItem("Exit") { Name = "Exit" };

        openItem.Click += (_, _) => TogglePopup();
        settingsItem.Click += (_, _) => OpenSettings();
        exitItem.Click += (_, _) => ExitThread();

        menu.Items.Add(openItem);
        menu.Items.Add(settingsItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);

        return menu;
    }

    private void OnNotifyIconMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            TogglePopup();
        }
    }

    private void TogglePopup()
    {
        if (_popup.Visible)
        {
            _popup.Hide();
            return;
        }

        var popupLocation = GetPopupLocation();
        _popup.Location = popupLocation;
        _popup.Show();
        _popup.Activate();
    }

    private Point GetPopupLocation()
    {
        const int margin = 10;
        var screen = Screen.FromPoint(Cursor.Position);
        var working = screen.WorkingArea;
        var bounds = screen.Bounds;

        var taskbarAtBottom = working.Bottom < bounds.Bottom;
        var taskbarAtTop = working.Top > bounds.Top;
        var taskbarAtLeft = working.Left > bounds.Left;
        var taskbarAtRight = working.Right < bounds.Right;

        var x = working.Right - _popup.Width - margin;
        var y = working.Bottom - _popup.Height - margin;

        if (taskbarAtTop)
        {
            y = working.Top + margin;
        }
        else if (taskbarAtLeft)
        {
            x = working.Left + margin;
        }
        else if (taskbarAtRight)
        {
            x = working.Right - _popup.Width - margin;
        }
        else if (taskbarAtBottom)
        {
            y = working.Bottom - _popup.Height - margin;
        }

        x = Math.Clamp(x, working.Left + margin, working.Right - _popup.Width - margin);
        y = Math.Clamp(y, working.Top + margin, working.Bottom - _popup.Height - margin);
        return new Point(x, y);
    }

    private void OpenSettings()
    {
        using var form = new SettingsForm(_settings.Clone())
        {
            Icon = _mainIcon
        };
        if (form.ShowDialog() != DialogResult.OK || form.Result is null) return;

        var newSettings = form.Result.Normalize();
        _settingsStorage.Save(newSettings);
        _settings = newSettings;

        _startupManager.SetEnabled(_settings.StartWithWindows);
        ApplyTheme(_settings.ThemeId);
        Refresh();
    }

    private void ApplyTheme(string themeId)
    {
        _theme = ThemeCatalog.Resolve(themeId);
        _popup.UpdateTheme(_theme);
    }

    private void Refresh()
    {
        var now = DateTime.Now;
        var snapshot = ProgressCalculator.BuildSnapshot(now, _settings);
        var rows = BuildRows(snapshot);

        _popup.UpdateRows(rows);
        _notifyIcon.Icon = _mainIcon;
        _notifyIcon.Text = BuildTooltipText(snapshot);
    }

    private List<ProgressViewModel> BuildRows(ProgressSnapshot snapshot)
    {
        var list = new List<ProgressViewModel>(4);
        if (_settings.ShowDay) list.Add(ProgressViewModel.FromItem(snapshot.Day));
        if (_settings.ShowMonth) list.Add(ProgressViewModel.FromItem(snapshot.Month));
        if (_settings.ShowYear) list.Add(ProgressViewModel.FromItem(snapshot.Year));
        if (_settings.ShowLife && snapshot.Life is { } lifeItem) list.Add(ProgressViewModel.FromItem(lifeItem));

        if (list.Count == 0)
        {
            list.Add(new ProgressViewModel("Day", snapshot.Day.Percentage, snapshot.Day.ClampedValue));
        }

        return list;
    }

    private static string BuildTooltipText(ProgressSnapshot snapshot)
    {
        var segments = new List<string>(4)
        {
            $"D {snapshot.Day.Percentage}%",
            $"M {snapshot.Month.Percentage}%",
            $"Y {snapshot.Year.Percentage}%"
        };

        if (snapshot.Life is { } life)
        {
            segments.Add($"L {life.Percentage}%");
        }

        var text = string.Join(" | ", segments);
        return text.Length <= 63 ? text : text[..63];
    }
}
