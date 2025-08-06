using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Windows.Input;

namespace R2CSharp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        WindowState = WindowState.FullScreen;
    }
    
    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
