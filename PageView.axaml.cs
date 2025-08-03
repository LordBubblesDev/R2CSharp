using Avalonia.Controls;

namespace R2CSharp;

public partial class PageView : UserControl
{
    public PageView()
    {
        InitializeComponent();
        DataContext = new PageViewModel();
    }
} 