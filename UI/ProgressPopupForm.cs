using TrayTempra.Services;

namespace TrayTempra.UI;

public sealed class ProgressPopupForm : Form
{
    private const int MinPopupHeight = 112;
    private const int ExtraVerticalPadding = 12;

    private readonly ProgressPanelControl _panel;

    public ProgressPopupForm(Theme theme)
    {
        ShowInTaskbar = false;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;
        Width = 300;
        Height = 164;
        Padding = Padding.Empty;
        BackColor = theme.Background;

        _panel = new ProgressPanelControl(theme)
        {
            Dock = DockStyle.Fill
        };

        Controls.Add(_panel);

        Deactivate += (_, _) => Hide();
        ApplyRoundedRegion();
    }

    public void UpdateTheme(Theme theme)
    {
        BackColor = theme.Background;
        _panel.SetTheme(theme);
        ApplyRoundedRegion();
    }

    public void UpdateRows(IReadOnlyList<ProgressViewModel> rows)
    {
        _panel.SetRows(rows);
        var targetHeight = Math.Max(MinPopupHeight, _panel.RequiredHeight + ExtraVerticalPadding);
        if (Height != targetHeight)
        {
            Height = targetHeight;
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        Activate();
        ApplyRoundedRegion();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        ApplyRoundedRegion();
    }

    private void ApplyRoundedRegion()
    {
        const int radius = 16;
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        var rect = new Rectangle(0, 0, Width, Height);
        var diameter = radius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        Region?.Dispose();
        Region = new Region(path);
    }
}
