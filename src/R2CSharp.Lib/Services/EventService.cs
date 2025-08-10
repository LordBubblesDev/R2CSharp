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
        window.PointerWheelChanged += OnPointerWheelChanged;
        window.PointerPressed += OnPointerPressed;
        window.PointerReleased += OnPointerReleased;
        window.KeyDown += OnKeyDown;
        
        if (viewModel != null) {
            viewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        
        if (carousel != null) {
            carousel.PropertyChanged += OnCarouselPropertyChanged;
        }
    }
    
    public void UnsubscribeFromEvents()
    {   
        if (viewModel != null) {
            viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        if (carousel != null) {
            carousel.PropertyChanged -= OnCarouselPropertyChanged;
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
    
    private void OnKeyDown(object? sender, KeyEventArgs e) => KeyDown?.Invoke(e);
    
    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e) => inputService.HandlePointerWheelChanged(e);
    private void OnPointerPressed(object? sender, PointerPressedEventArgs e) => inputService.HandlePointerPressed(e);
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e) => inputService.HandlePointerReleased(e);
    
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