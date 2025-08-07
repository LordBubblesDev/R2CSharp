using System.Windows.Input;
using Avalonia.Media.Imaging;

namespace R2CSharp.Lib.Models;

public class RebootOption
{
    public string Name { get; set; } = string.Empty;
    public int Index { get; init; }
    public Bitmap? Icon { get; set; }
    public string FallbackIcon { get; set; } = "fa-solid fa-rocket";
    public ICommand? Command { get; set; }
}
