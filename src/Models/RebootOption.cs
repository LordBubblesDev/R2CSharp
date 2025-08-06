using Avalonia.Media.Imaging;
using System.Windows.Input;
using Avalonia;

namespace R2CSharp.Models;

public class RebootOption
{
    public string Name { get; set; } = string.Empty;
    public int Index { get; init; }
    public Bitmap? Icon { get; set; }
    public string FallbackIcon { get; set; } = "fa-solid fa-rocket";
    public ICommand? Command { get; set; }
    public bool IsUmsOption { get; set; }
    public Thickness? CustomMargin { get; set; }
    public Thickness EffectiveMargin => CustomMargin ?? new Thickness(27, 4, 27, 4);
}
