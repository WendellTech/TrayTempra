using System.Drawing.Drawing2D;
using TrayTempra.Services;

namespace TrayTempra.UI;

public sealed class ProgressPanelControl : Control
{
    private const int RowHeight = 36;
    private const int HorizontalPadding = 14;
    private const int TopPadding = 14;

    private readonly Font _labelFont = new("Segoe UI", 8.8f, FontStyle.Regular, GraphicsUnit.Point);
    private readonly Font _percentFont = new("Segoe UI Semibold", 9f, FontStyle.Regular, GraphicsUnit.Point);

    private Theme _theme;
    private IReadOnlyList<ProgressViewModel> _rows = Array.Empty<ProgressViewModel>();

    public int RequiredHeight { get; private set; } = CalculateRequiredHeight(0);

    public ProgressPanelControl(Theme theme)
    {
        _theme = theme;
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    public void SetTheme(Theme theme)
    {
        _theme = theme;
        BackColor = theme.Background;
        ForeColor = theme.Foreground;
        Invalidate();
    }

    public void SetRows(IReadOnlyList<ProgressViewModel> rows)
    {
        _rows = rows;
        RequiredHeight = CalculateRequiredHeight(rows.Count);
        Invalidate();
    }

    public static int CalculateRequiredHeight(int rowCount)
    {
        return Math.Max(TopPadding * 2 + rowCount * RowHeight, 90);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _labelFont.Dispose();
            _percentFont.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        e.Graphics.Clear(_theme.Background);

        if (_rows.Count == 0) return;

        for (var i = 0; i < _rows.Count; i++)
        {
            DrawRow(e.Graphics, _rows[i], i);
        }
    }

    private void DrawRow(Graphics g, ProgressViewModel item, int index)
    {
        var y = TopPadding + index * RowHeight;
        var titleRect = new Rectangle(HorizontalPadding, y, Width - HorizontalPadding * 2, 16);
        var barRect = new Rectangle(HorizontalPadding, y + 18, Width - HorizontalPadding * 2, 11);

        using var trackBrush = new SolidBrush(_theme.ProgressTrack);
        using var fillBrush = new SolidBrush(_theme.ProgressFill);
        using var textBrush = new SolidBrush(_theme.Foreground);
        using var secondaryTextBrush = new SolidBrush(_theme.SecondaryText);

        g.DrawString(item.Label, _labelFont, secondaryTextBrush, titleRect.Location);

        var percent = $"{item.Percentage}%";
        var percentSize = g.MeasureString(percent, _percentFont);
        g.DrawString(percent, _percentFont, textBrush, Width - HorizontalPadding - percentSize.Width, y - 1);

        FillRoundedRect(g, trackBrush, barRect, 5);

        var fillWidth = (int)Math.Round(barRect.Width * item.Value, MidpointRounding.AwayFromZero);
        if (fillWidth > 0)
        {
            FillRoundedRect(g, fillBrush, new Rectangle(barRect.X, barRect.Y, fillWidth, barRect.Height), 5);
        }
    }

    private static void FillRoundedRect(Graphics g, Brush brush, Rectangle rect, int radius)
    {
        using var path = BuildRoundedRectPath(rect, radius);
        g.FillPath(brush, path);
    }

    private static GraphicsPath BuildRoundedRectPath(Rectangle rect, int radius)
    {
        var diameter = radius * 2;
        var path = new GraphicsPath();

        if (rect.Width <= diameter || rect.Height <= diameter)
        {
            path.AddRectangle(rect);
            path.CloseFigure();
            return path;
        }

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }
}
