using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace R2CSharp.Lib.Views;

public partial class CloseButtonOverlay : UserControl
{
    public CloseButtonOverlay()
    {
        InitializeComponent();
    }
    
    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.GetVisualRoot() is Window window)
        {
            window.Close();
        }
    }
} 