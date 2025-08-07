using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using R2CSharp.Controls;

namespace R2CSharp.Services;

public class EventService(
    ScrollDetectionService scrollService,
    TouchDetectionService touchService,
    KeyNavService keyboardService,
    ViewModels.CarouselPageViewModel? viewModel,
    CarouselControl? carousel)
{
    private readonly KeyNavService _keyboardService = keyboardService;

    public void SubscribeToEvents(Window window)
    {
        window.PointerWheelChanged += OnPointerWheelChanged;
        window.PointerPressed += OnPointerPressed;
        window.PointerReleased += OnPointerReleased;
        window.PointerMoved += OnPointerMoved;
        window.KeyDown += OnKeyDown;
        
        if (viewModel != null) {
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        
        if (carousel != null) {
            carousel.PropertyChanged += OnCarouselPropertyChanged;
        }
    }
    
    public void UnsubscribeFromEvents(Window window)
    {
        window.PointerWheelChanged -= OnPointerWheelChanged;
        window.PointerPressed -= OnPointerPressed;
        window.PointerReleased -= OnPointerReleased;
        window.PointerMoved -= OnPointerMoved;
        window.KeyDown -= OnKeyDown;
        
        if (viewModel != null) {
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        if (carousel != null) {
            carousel.PropertyChanged -= OnCarouselPropertyChanged;
        }
    }

    public event Action<ViewModels.CarouselPageViewModel>? ViewModelPropertyChanged;
    public event Action<AvaloniaPropertyChangedEventArgs>? CarouselPropertyChanged;
    public event Action<KeyEventArgs>? KeyDown;
    
    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        scrollService.HandlePointerWheelChanged(e);
    }
    
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        touchService.HandlePointerPressed(e);
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        touchService.HandlePointerReleased(e);
    }
    
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        touchService.HandlePointerMoved(e);
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        KeyDown?.Invoke(e);
    }
    
    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (sender is ViewModels.CarouselPageViewModel model) {
            ViewModelPropertyChanged?.Invoke(model);
        }
    }
    
    private void OnCarouselPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        CarouselPropertyChanged?.Invoke(e);
    }
} 