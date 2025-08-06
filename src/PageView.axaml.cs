using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.VisualTree;
using R2CSharp.Services;

namespace R2CSharp;

public partial class PageView : UserControl
{
    private readonly ScrollDetectionService _scrollService;
    
    public PageView()
    {
        InitializeComponent();
        
        DataContext = new PageViewModel();
        _scrollService = new ScrollDetectionService();
        _scrollService.PageChangeRequested += OnPageChangeRequested;
        AttachedToVisualTree += OnAttachedToVisualTree;
    }
    
    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (this.GetVisualRoot() is Window window) {
            window.PointerWheelChanged += OnPointerWheelChanged;
        }
        
        if (DataContext is PageViewModel viewModel) {
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
    
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(PageViewModel.Pages) || sender is not PageViewModel r2CModel) return;
        if (MainCarousel == null) return;
        foreach (var t in r2CModel.Pages) {
            var pageView = new Views.StandardPageView { DataContext = t };
            MainCarousel.AddPage(pageView);
        }
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        _scrollService.HandlePointerWheelChanged(e);
    }
    
    private void OnPageChangeRequested(int direction)
    {
        if (MainCarousel == null) return;
            
        MainCarousel.AllowNavigation = true;
            
        if (direction > 0) {
            MainCarousel.Previous();
        }
        else {
            MainCarousel.Next();
        }
            
        MainCarousel.AllowNavigation = false;
    }
    
    private void OnPreviousButtonClick(object? sender, RoutedEventArgs e)
    {
        if (MainCarousel == null) return;
        MainCarousel.AllowNavigation = true;
        MainCarousel.Previous();
        MainCarousel.AllowNavigation = false;
    }
    
    private void OnNextButtonClick(object? sender, RoutedEventArgs e)
    {
        if (MainCarousel == null) return;
        MainCarousel.AllowNavigation = true;
        MainCarousel.Next();
        MainCarousel.AllowNavigation = false;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (DataContext is PageViewModel viewModel) 
        {
            viewModel.Cleanup();
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        if (this.GetVisualRoot() is Window window)
        {
            window.PointerWheelChanged -= OnPointerWheelChanged;
        }
        _scrollService.PageChangeRequested -= OnPageChangeRequested;
    }
}