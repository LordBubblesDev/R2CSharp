using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using R2CSharp.Lib.Controls;
using R2CSharp.Lib.ViewModels;

namespace R2CSharp.Lib.Services;

public class EventService(
    ScrollDetectionService scrollService,
    TouchDetectionService touchService,
    KeyNavService keyboardService,
    CarouselPageViewModel? viewModel,
    CarouselControl? carousel)
{
    private readonly KeyNavService _keyboardService = keyboardService;
    private CarouselPageViewModel? _viewModel = viewModel;
    private CarouselControl? _carousel = carousel;

    public void SubscribeToEvents(Window window)
    {
        window.PointerWheelChanged += OnPointerWheelChanged;
        window.PointerPressed += OnPointerPressed;
        window.PointerReleased += OnPointerReleased;
        window.PointerMoved += OnPointerMoved;
        window.KeyDown += OnKeyDown;
        
        if (_viewModel != null) {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        
        if (_carousel != null) {
            _carousel.PropertyChanged += OnCarouselPropertyChanged;
        }
    }
    
    public void UnsubscribeFromEvents(Window window)
    {
        window.PointerWheelChanged -= OnPointerWheelChanged;
        window.PointerPressed -= OnPointerPressed;
        window.PointerReleased -= OnPointerReleased;
        window.PointerMoved -= OnPointerMoved;
        window.KeyDown -= OnKeyDown;
        
        if (_viewModel != null) {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        if (_carousel != null) {
            _carousel.PropertyChanged -= OnCarouselPropertyChanged;
        }
    }

    public event Action<CarouselPageViewModel>? ViewModelPropertyChanged;
    public event Action<AvaloniaPropertyChangedEventArgs>? CarouselPropertyChanged;
    public event Action<KeyEventArgs>? KeyDown;

    public void UpdateViewModel(CarouselPageViewModel newViewModel)
    {
        if (_viewModel != null) {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        _viewModel = newViewModel;
        
        if (_viewModel != null) {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }

    public void SubscribeToControlEvents(UserControl control)
    {
        control.KeyDown += OnKeyDown;
    }

    public void UnsubscribeFromControlEvents(UserControl control)
    {
        control.KeyDown -= OnKeyDown;
    }
    
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
        if (sender is CarouselPageViewModel model) {
            ViewModelPropertyChanged?.Invoke(model);
        }
    }
    
    private void OnCarouselPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        CarouselPropertyChanged?.Invoke(e);
    }
} 