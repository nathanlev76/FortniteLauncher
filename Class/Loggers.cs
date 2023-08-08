using System.Drawing;
using Pastel;

namespace FortniteLauncher.Class;

public class Loggers
{
    # region colors 
    private Color paths = Color.FromArgb(16, 176, 125);
    private Color types = Color.FromArgb(168, 16, 176);
    private Color errors = Color.FromArgb(224, 16, 16);
    private Color warnings = Color.FromArgb(235, 211, 0);
    private Color aqua = Color.FromArgb(38, 196, 236);
    private Color pink = Color.FromArgb(177, 74, 200);

    #endregion

    public string Green(string path) => path.Pastel(paths);
    public string Purple(string type) => type.Pastel(types);
    public string Red(string error) => error.Pastel(errors);
    public string Yellow(string text) => text.Pastel(warnings);
    public string Aqua(string text) => text.Pastel(aqua);
    public string Pink(string text) => text.Pastel(pink);


}
