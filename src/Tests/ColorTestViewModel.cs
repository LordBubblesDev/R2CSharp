using System.Collections.ObjectModel;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using R2CSharp.Converters;

namespace R2CSharp.Tests;

public partial class ColorTestViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<ColorItem> _colorItems = new();

    public ColorTestViewModel()
    {
        LoadAllColors();
        
    }

    private void LoadAllColors()
    {
        var allColors = ColorConverter.GetAllHueColors();
        
        for (int i = 0; i < allColors.Length; i++)
        {
            ColorItems.Add(new ColorItem
            {
                Hue = i,
                HexColor = allColors[i],
                ColorBrush = new SolidColorBrush(Color.Parse(allColors[i]))
            });
        }
    }
}

public class ColorItem
{
    public int Hue { get; set; }
    public string HexColor { get; set; } = string.Empty;
    public IBrush ColorBrush { get; set; } = Brushes.Transparent;
}
