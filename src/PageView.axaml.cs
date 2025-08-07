using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.VisualTree;
using R2CSharp.Controls;
using R2CSharp.Services;

namespace R2CSharp;

public partial class PageView : UserControl
{
    private readonly ScrollDetectionService _scrollService;
    private readonly TouchDetectionService _touchService;
    private readonly PageViewModel? _viewModel;
    
    public PageView()
    {
        InitializeComponent();
        
        _viewModel = new PageViewModel();
        DataContext = _viewModel;
        _scrollService = new ScrollDetectionService();
        _touchService = new TouchDetectionService();
        _scrollService.PageChangeRequested += OnPageChangeRequested;
        _touchService.PageChangeRequested += OnPageChangeRequested;
        AttachedToVisualTree += OnAttachedToVisualTree;
    }
    
    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (this.GetVisualRoot() is Window window) {
            window.PointerWheelChanged += OnPointerWheelChanged;
            window.PointerPressed += OnPointerPressed;
            window.PointerReleased += OnPointerReleased;
            window.PointerMoved += OnPointerMoved;
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
        UpdateNavigationState();
        MainCarousel.PropertyChanged += OnCarouselPropertyChanged;
    }
    
    private void OnCarouselPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == CarouselControl.CurrentIndexProperty)
        {
            UpdateNavigationState();
        }
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        _scrollService.HandlePointerWheelChanged(e);
    }
    
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _touchService.HandlePointerPressed(e);
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _touchService.HandlePointerReleased(e);
    }
    
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        _touchService.HandlePointerMoved(e);
    }
    
    private void OnPageChangeRequested(int direction)
    {
        if (MainCarousel == null) return;
            
        if (direction > 0) {
            MainCarousel.Previous();
        }
        else {
            MainCarousel.Next();
        }
            
        UpdateNavigationState();
    }
    
    private void OnPreviousButtonClick(object? sender, RoutedEventArgs e)
    {
        if (MainCarousel == null) return;
        MainCarousel.Previous();
        UpdateNavigationState();
    }
    
    private void OnNextButtonClick(object? sender, RoutedEventArgs e)
    {
        if (MainCarousel == null) return;
        MainCarousel.Next();
        UpdateNavigationState();
    }

    private void UpdateNavigationState()
    {
        if (_viewModel == null || MainCarousel == null) return;
        
        var currentIndex = MainCarousel.CurrentIndex;
        var totalPages = _viewModel.Pages.Count;
        
        _viewModel.CanGoPrevious = currentIndex > 0;
        _viewModel.CanGoNext = currentIndex < totalPages - 1;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (DataContext is PageViewModel viewModel) 
        {
            viewModel.Cleanup();
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        if (MainCarousel != null)
        {
            MainCarousel.PropertyChanged -= OnCarouselPropertyChanged;
        }
        
        if (this.GetVisualRoot() is Window window)
        {
            window.PointerWheelChanged -= OnPointerWheelChanged;
            window.PointerPressed -= OnPointerPressed;
            window.PointerReleased -= OnPointerReleased;
            window.PointerMoved -= OnPointerMoved;
        }
        _scrollService.PageChangeRequested -= OnPageChangeRequested;
        _touchService.PageChangeRequested -= OnPageChangeRequested;
    }
}