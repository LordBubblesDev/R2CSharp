using Avalonia.Controls;

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
}
