using Avalonia.Controls;
using Avalonia.Interactivity;

namespace R2CSharp;

public partial class PageView : UserControl
{
    public PageView()
    {
        InitializeComponent();
        DataContext = new PageViewModel();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (DataContext is PageViewModel viewModel) viewModel.Cleanup();
    }
}