using TrayTempra.Models;
using TrayTempra.Services;

namespace TrayTempra.UI;

public sealed class SettingsForm : Form
{
    private readonly TableLayoutPanel _table;
    private readonly ComboBox _themeCombo = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly CheckBox _showDay = new() { Text = "Show Day" };
    private readonly CheckBox _showMonth = new() { Text = "Show Month" };
    private readonly CheckBox _showYear = new() { Text = "Show Year" };
    private readonly CheckBox _showLife = new() { Text = "Show Life" };
    private readonly DateTimePicker _birthDate = new() { Format = DateTimePickerFormat.Short };
    private readonly NumericUpDown _lifespanYears = new() { Minimum = 1, Maximum = 150, Value = 85 };
    private readonly NumericUpDown _dayStart = new() { Minimum = 0, Maximum = 23 };
    private readonly NumericUpDown _monthStartDay = new() { Minimum = 1, Maximum = 28, Value = 1 };
    private readonly NumericUpDown _yearStartMonth = new() { Minimum = 1, Maximum = 12, Value = 1 };
    private readonly NumericUpDown _yearStartDay = new() { Minimum = 1, Maximum = 28, Value = 1 };
    private readonly CheckBox _startWithWindows = new() { Text = "Start with Windows" };
    private readonly Button _saveButton = new() { Text = "Save", Width = 92, Height = 30 };
    private readonly Button _cancelButton = new() { Text = "Cancel", Width = 92, Height = 30 };

    public AppSettings? Result { get; private set; }

    public SettingsForm(AppSettings current)
    {
        Text = "TrayTempra Settings";
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(396, 470);
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);

        foreach (var theme in ThemeCatalog.All)
        {
            _themeCombo.Items.Add(new ThemeItem(theme.Id, theme.Name));
        }

        _table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(14, 14, 14, 10),
            ColumnCount = 2,
            RowCount = 13,
            AutoSize = false
        };

        _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));
        _table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));

        AddRow(_table, 0, "Theme", _themeCombo);
        AddRow(_table, 1, "", _showDay);
        AddRow(_table, 2, "", _showMonth);
        AddRow(_table, 3, "", _showYear);
        AddRow(_table, 4, "", _showLife);
        AddRow(_table, 5, "Birth Date", _birthDate);
        AddRow(_table, 6, "Lifespan Years", _lifespanYears);
        AddRow(_table, 7, "Day Start Hour", _dayStart);
        AddRow(_table, 8, "Month Start Day", _monthStartDay);
        AddRow(_table, 9, "Year Start Month", _yearStartMonth);
        AddRow(_table, 10, "Year Start Day", _yearStartDay);
        AddRow(_table, 11, "", _startWithWindows);
        _table.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));

        var buttonsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            Margin = new Padding(0, 12, 0, 0)
        };

        _saveButton.FlatStyle = FlatStyle.Flat;
        _cancelButton.FlatStyle = FlatStyle.Flat;
        _saveButton.Click += (_, _) => SaveAndClose();
        _cancelButton.Click += (_, _) => DialogResult = DialogResult.Cancel;

        buttonsPanel.Controls.Add(_saveButton);
        buttonsPanel.Controls.Add(_cancelButton);

        _table.Controls.Add(buttonsPanel, 0, 12);
        _table.SetColumnSpan(buttonsPanel, 2);

        Controls.Add(_table);

        _showLife.CheckedChanged += (_, _) => ToggleLifeControls();
        _themeCombo.SelectedIndexChanged += (_, _) => ApplyThemeFromSelection();

        LoadFrom(current);
        ApplyTheme(ThemeCatalog.Resolve(current.ThemeId));
    }

    private static void AddRow(TableLayoutPanel table, int row, string label, Control control)
    {
        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 31));
        var title = new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0)
        };
        control.Margin = new Padding(0);
        control.Dock = DockStyle.Fill;

        table.Controls.Add(title, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private void LoadFrom(AppSettings settings)
    {
        _themeCombo.SelectedItem = _themeCombo.Items.Cast<ThemeItem>()
            .FirstOrDefault(i => string.Equals(i.Id, settings.ThemeId, StringComparison.OrdinalIgnoreCase))
            ?? _themeCombo.Items[0];

        _showDay.Checked = settings.ShowDay;
        _showMonth.Checked = settings.ShowMonth;
        _showYear.Checked = settings.ShowYear;
        _showLife.Checked = settings.ShowLife;
        _birthDate.Value = settings.BirthDate.ToDateTime(TimeOnly.MinValue);
        _lifespanYears.Value = settings.LifespanYears;
        _dayStart.Value = settings.DayStartHour;
        _monthStartDay.Value = settings.MonthStartDay;
        _yearStartMonth.Value = settings.YearStartMonth;
        _yearStartDay.Value = settings.YearStartDay;
        _startWithWindows.Checked = settings.StartWithWindows;
        ToggleLifeControls();
    }

    private void ToggleLifeControls()
    {
        var lifeEnabled = _showLife.Checked;
        _birthDate.Enabled = lifeEnabled;
        _lifespanYears.Enabled = lifeEnabled;
    }

    private void SaveAndClose()
    {
        if (_themeCombo.SelectedItem is not ThemeItem theme) return;

        Result = new AppSettings
        {
            ThemeId = theme.Id,
            ShowDay = _showDay.Checked,
            ShowMonth = _showMonth.Checked,
            ShowYear = _showYear.Checked,
            ShowLife = _showLife.Checked,
            BirthDate = DateOnly.FromDateTime(_birthDate.Value.Date),
            LifespanYears = (int)_lifespanYears.Value,
            DayStartHour = (int)_dayStart.Value,
            MonthStartDay = (int)_monthStartDay.Value,
            YearStartMonth = (int)_yearStartMonth.Value,
            YearStartDay = (int)_yearStartDay.Value,
            StartWithWindows = _startWithWindows.Checked
        }.Normalize();

        DialogResult = DialogResult.OK;
    }

    private void ApplyThemeFromSelection()
    {
        if (_themeCombo.SelectedItem is not ThemeItem item) return;
        ApplyTheme(ThemeCatalog.Resolve(item.Id));
    }

    private void ApplyTheme(Theme theme)
    {
        BackColor = theme.Background;
        ForeColor = theme.Foreground;
        _table.BackColor = theme.Background;

        var controls = Controls.Cast<Control>().ToArray();
        foreach (var control in controls)
        {
            ApplyThemeRecursive(control, theme);
        }

        _saveButton.BackColor = theme.ProgressFill;
        _saveButton.ForeColor = theme.Background;
        _saveButton.FlatAppearance.BorderColor = theme.Border;

        _cancelButton.BackColor = theme.ProgressTrack;
        _cancelButton.ForeColor = theme.Foreground;
        _cancelButton.FlatAppearance.BorderColor = theme.Border;
    }

    private static void ApplyThemeRecursive(Control control, Theme theme)
    {
        control.ForeColor = theme.Foreground;

        switch (control)
        {
            case Label:
                control.BackColor = theme.Background;
                control.ForeColor = theme.SecondaryText;
                break;
            case Button:
                break;
            case TextBox:
            case ComboBox:
            case NumericUpDown:
            case DateTimePicker:
                control.BackColor = theme.ProgressTrack;
                break;
            case CheckBox:
                control.BackColor = theme.Background;
                break;
            default:
                control.BackColor = theme.Background;
                break;
        }

        foreach (Control child in control.Controls)
        {
            ApplyThemeRecursive(child, theme);
        }
    }

    private sealed record ThemeItem(string Id, string Name)
    {
        public override string ToString() => Name;
    }
}
