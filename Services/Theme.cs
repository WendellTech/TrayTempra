using System.Drawing;

namespace TrayTempra.Services;

public sealed record Theme(
    string Id,
    string Name,
    Color Background,
    Color Foreground,
    Color SecondaryText,
    Color ProgressTrack,
    Color ProgressFill,
    Color Accent,
    Color Border);
