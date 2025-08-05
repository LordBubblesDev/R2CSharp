using Avalonia.Media.Imaging;

namespace R2CSharp.Models;

public class RebootOption
{
    public string Name { get; set; } = string.Empty;
    public RebootType Type { get; set; }
    public int Index { get; set; }
    public Bitmap? Icon { get; set; }
}

public enum RebootType
{
    Launch,
    Config,
    Ums
}