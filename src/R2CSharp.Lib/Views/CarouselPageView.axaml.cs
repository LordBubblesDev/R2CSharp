using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using R2CSharp.Lib.Controls;
using R2CSharp.Lib.Helpers;
using R2CSharp.Lib.Models;
using R2CSharp.Lib.Services;
using R2CSharp.Lib.ViewModels;

namespace R2CSharp.Lib.Views;

public partial class CarouselPageView : UserControl
{
    private readonly EventService _eventService;
    private PageStateHelper? _pageStateHelper;
    private readonly KeyNavService _keyboardService;

    public CarouselPageView()
    {
        InitializeComponent();
        
        // Create a temporary ViewModel for initial binding
        var tempViewModel = new CarouselPageViewModel();
        DataContext = tempViewModel;
        
        // Initialize services
        var scrollService = new ScrollDetectionService();
        var touchService = new TouchDetectionService();
        _keyboardService = new KeyNavService();
        
        // Create event service (will be updated when ViewModel is set)
        _eventService = new EventService(scrollService, touchService, tempViewModel, MainCarousel);
        
        // Subscribe to events
        scrollService.PageChangeRequested += OnPageChangeRequested;
        touchService.PageChangeRequested += OnPageChangeRequested;
        _keyboardService.SelectionChanged += OnSelectionChanged;
        _keyboardService.PageChangeRequested += OnKeyboardPageChangeRequested;
        _keyboardService.ButtonPressed += OnButtonPressed;
        _eventService.ViewModelPropertyChanged += OnViewModelPropertyChanged;
        _eventService.CarouselPropertyChanged += OnCarouselPropertyChanged;
        _eventService.KeyDown += OnKeyDown;
        
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    /// <summary>
    /// Sets the ViewModel and initializes the carousel. Call this implicitly to load the app.
    /// </summary>
    /// <param name="viewModel">The ViewModel to use</param>
    public async Task SetViewModelAsync(CarouselPageViewModel viewModel)
    {
        _pageStateHelper = new PageStateHelper(viewModel, MainCarousel, _keyboardService);
        _eventService.UpdateViewModel(viewModel);
        DataContext = viewModel;
        await viewModel.LoadAsync();
    }
    
    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (this.GetVisualRoot() is Window window)
        {
            _eventService.SubscribeToEvents(window);
        }
    }
    
    private void OnViewModelPropertyChanged(CarouselPageViewModel viewModel)
    {
        if (MainCarousel == null || _pageStateHelper == null) return;
        _pageStateHelper.InitializePages();
    }
    
    private void OnCarouselPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == CarouselControl.CurrentIndexProperty && _pageStateHelper != null)
        {
            _pageStateHelper.HandlePageChange();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if ((MainCarousel?.Children!).FirstOrDefault() is not RebootOptionPageView currentPageView) return;
        if (currentPageView.DataContext is not PageConfiguration currentPage) return;
        if (DataContext is not CarouselPageViewModel viewModel) return;
        
        var currentSelection = GetCurrentSelection(currentPage);
        _keyboardService.HandleKeyPressed(e, currentPage, currentSelection, viewModel.CanGoPrevious, viewModel.CanGoNext);
    }
    
    private void OnSelectionChanged(int index) => _pageStateHelper?.HandleSelectionChange();
    private void OnKeyboardPageChangeRequested(int direction) => _pageStateHelper?.HandleKeyboardPageChange(direction);
    private void OnButtonPressed(int buttonIndex) => _pageStateHelper?.HandleButtonPress(buttonIndex);
    private void OnPageChangeRequested(int direction) => _pageStateHelper?.HandleScrollPageChange(direction);
    
    private void OnPreviousButtonClick(object? sender, RoutedEventArgs e) => _pageStateHelper?.NavigatePrevious();
    private void OnNextButtonClick(object? sender, RoutedEventArgs e) => _pageStateHelper?.NavigateNext();
    
    private static int GetCurrentSelection(PageConfiguration page) => page.SelectedIndex;
    
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        if (this.GetVisualRoot() is Window window)
        {
            _eventService.UnsubscribeFromEvents(window);
        }
        
        base.OnUnloaded(e);
    }
}