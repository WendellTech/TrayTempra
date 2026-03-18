using System.Windows.Forms;

namespace TrayTempra;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new TrayTempraAppContext());
    }
}
