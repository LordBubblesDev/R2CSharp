using Avalonia.Controls;

namespace R2CSharp.Lib.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
#if !DEBUG && !COLORTEST
        WindowState = WindowState.FullScreen;
#endif
    }
}
