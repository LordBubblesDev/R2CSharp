using Avalonia.Controls;

namespace R2CSharp.Lib.Tests;

public partial class ColorTestView : UserControl
{
    public ColorTestView()
    {
        InitializeComponent();
        DataContext = new ColorTestViewModel();
    }
}