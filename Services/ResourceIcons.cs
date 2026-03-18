using System.Reflection;

namespace TrayTempra.Services;

public static class ResourceIcons
{
    public static Icon? TryLoadMainIcon()
    {
        var assembly = Assembly.GetExecutingAssembly();
        const string resourceName = "TrayTempra.assets.logo_sandglass_ico.ico";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        return stream is null ? null : new Icon(stream);
    }
}
