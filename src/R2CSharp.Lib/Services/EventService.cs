using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using R2CSharp.Lib.Controls;
using R2CSharp.Lib.ViewModels;

namespace R2CSharp.Lib.Services;

public class EventService(
    InputDetectionService inputService,
    CarouselPageViewModel? viewModel,
    CarouselControl? carousel)
{
    public event Action<CarouselPageViewModel>? ViewModelPropertyChanged;
    public event Action<AvaloniaPropertyChangedEventArgs>? CarouselPropertyChanged;
    public event Action<KeyEventArgs>? KeyDown;

    public void SubscribeToEvents(Window window)
    {
        window.KeyDown += OnKeyDown;
        
        if (carousel != null) {
            carousel.PointerWheelChanged += OnPointerWheelChanged;
            carousel.PointerPressed += OnPointerPressed;
            carousel.PointerReleased += OnPointerReleased;
            carousel.PropertyChanged += OnCarouselPropertyChanged;
        }
        
        if (viewModel != null) {
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
    
    public void UnsubscribeFromEvents()
    {   
        if (carousel != null) {
            carousel.PointerWheelChanged -= OnPointerWheelChanged;
            carousel.PointerPressed -= OnPointerPressed;
            carousel.PointerReleased -= OnPointerReleased;
            carousel.PropertyChanged -= OnCarouselPropertyChanged;
        }
        
        if (viewModel != null) {
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }

    public void UpdateViewModel(CarouselPageViewModel? newViewModel)
    {
        if (viewModel != null) {
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        if (newViewModel != null) {
            newViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
    }
    
    private bool IsInputAllowed()
    {
        if (carousel == null) return false;
        return carousel.IsEnabled && carousel.IsHitTestVisible;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!IsInputAllowed()) return;
        KeyDown?.Invoke(e);
    }
    
    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!IsInputAllowed()) return;
        inputService.HandlePointerWheelChanged(e);
    }
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!IsInputAllowed()) return;
        inputService.HandlePointerPressed(e);
    }
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!IsInputAllowed()) return;
        inputService.HandlePointerReleased(e);
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