using Avalonia.Controls;
using R2CSharp.Lib.ViewModels;

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
        
        LoadCarouselAsync();
    }
    
    private async void LoadCarouselAsync()
    {
        try {
            if (this.FindControl<CarouselPageView>("CarouselView") is not { } carousel) {
                return;
            }
            var viewModel = new CarouselPageViewModel();
            await carousel.SetViewModelAsync(viewModel);
        }
        catch (Exception e) {
            Console.WriteLine(e);
        }
    }
}
