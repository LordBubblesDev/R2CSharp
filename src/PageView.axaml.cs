using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.VisualTree;
using R2CSharp.Controls;
using R2CSharp.Services;
using R2CSharp.Helpers;
using R2CSharp.Models;
using R2CSharp.Views;

namespace R2CSharp;

public partial class PageView : UserControl
{
    private readonly EventService _eventService;
    private readonly PageStateHelper _pageStateHelper;
    private readonly KeyNavService _keyboardService;

    public PageView()
    {
        InitializeComponent();
        
        var viewModel = new PageViewModel();
        DataContext = viewModel;
        
        // Initialize services
        var scrollService = new ScrollDetectionService();
        var touchService = new TouchDetectionService();
        _keyboardService = new KeyNavService();
        var buttonSelectionService = new ButtonHelper();
        var pageTransitionService = new PageTransitionHelper();
        
        // Create state and event services
        _pageStateHelper = new PageStateHelper(viewModel, MainCarousel, buttonSelectionService, pageTransitionService, _keyboardService);
        _eventService = new EventService(scrollService, touchService, _keyboardService, viewModel, MainCarousel);
        
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
    
    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (this.GetVisualRoot() is Window window)
        {
            _eventService.SubscribeToEvents(window);
        }
    }
    
    private void OnViewModelPropertyChanged(PageViewModel viewModel)
    {
        if (MainCarousel == null) return;
        _pageStateHelper.InitializePages();
    }
    
    private void OnCarouselPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == CarouselControl.CurrentIndexProperty)
        {
            _pageStateHelper.HandlePageChange();
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (MainCarousel?.Children.FirstOrDefault() is not StandardPageView currentPageView) return;
        if (currentPageView.DataContext is not PageConfiguration currentPage) return;
        
        var currentSelection = GetCurrentSelection(currentPage);
        _keyboardService.HandleKeyDown(e, currentPage, currentSelection);
    }
    
    private void OnSelectionChanged(int index) => _pageStateHelper.HandleSelectionChange(index);
    private void OnKeyboardPageChangeRequested(int direction) => _pageStateHelper.HandleKeyboardPageChange(direction);
    private void OnButtonPressed(int buttonIndex) => _pageStateHelper.HandleButtonPress(buttonIndex);
    private void OnPageChangeRequested(int direction) => _pageStateHelper.HandleScrollPageChange(direction);
    
    private void OnPreviousButtonClick(object? sender, RoutedEventArgs e) => _pageStateHelper.NavigatePrevious();
    private void OnNextButtonClick(object? sender, RoutedEventArgs e) => _pageStateHelper.NavigateNext();
    
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