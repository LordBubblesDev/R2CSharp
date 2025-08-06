using Avalonia.Controls;
using Avalonia.Interactivity;

namespace R2CSharp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
#if !DEBUG
        WindowState = WindowState.FullScreen;
#endif
    }
    
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
