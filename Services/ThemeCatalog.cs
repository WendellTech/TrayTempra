using System.Collections.ObjectModel;
using System.Drawing;

namespace TrayTempra.Services;

public static class ThemeCatalog
{
    private static readonly IReadOnlyList<Theme> Themes = new ReadOnlyCollection<Theme>(
        [
            new(
                "PlainLight",
                "Plain Light",
                ColorTranslator.FromHtml("#FAFAFA"),
                ColorTranslator.FromHtml("#222222"),
                ColorTranslator.FromHtml("#5A5A5A"),
                ColorTranslator.FromHtml("#E3E3E3"),
                ColorTranslator.FromHtml("#2E7D32"),
                ColorTranslator.FromHtml("#1B5E20"),
                ColorTranslator.FromHtml("#D6D6D6")),
            new(
                "PlainDark",
                "Plain Dark",
                ColorTranslator.FromHtml("#1E1F22"),
                ColorTranslator.FromHtml("#ECECEC"),
                ColorTranslator.FromHtml("#B2B2B2"),
                ColorTranslator.FromHtml("#34363A"),
                ColorTranslator.FromHtml("#66BB6A"),
                ColorTranslator.FromHtml("#A5D6A7"),
                ColorTranslator.FromHtml("#3D4147")),
            new(
                "CatppuccinLatte",
                "Catppuccin Latte",
                ColorTranslator.FromHtml("#EFF1F5"),
                ColorTranslator.FromHtml("#4C4F69"),
                ColorTranslator.FromHtml("#6C6F85"),
                ColorTranslator.FromHtml("#DCE0E8"),
                ColorTranslator.FromHtml("#40A02B"),
                ColorTranslator.FromHtml("#1E66F5"),
                ColorTranslator.FromHtml("#CCD0DA")),
            new(
                "CatppuccinMocha",
                "Catppuccin Mocha",
                ColorTranslator.FromHtml("#1E1E2E"),
                ColorTranslator.FromHtml("#CDD6F4"),
                ColorTranslator.FromHtml("#A6ADC8"),
                ColorTranslator.FromHtml("#313244"),
                ColorTranslator.FromHtml("#A6E3A1"),
                ColorTranslator.FromHtml("#89B4FA"),
                ColorTranslator.FromHtml("#45475A"))
        ]);

    public static IReadOnlyList<Theme> All => Themes;

    public static Theme Resolve(string? id)
    {
        var theme = Themes.FirstOrDefault(t =>
            string.Equals(t.Id, id, StringComparison.OrdinalIgnoreCase));
        return theme ?? Themes[1];
    }
}
