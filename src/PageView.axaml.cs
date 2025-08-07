using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.VisualTree;
using R2CSharp.Controls;
using R2CSharp.Services;
using Avalonia.Threading;
using R2CSharp.Models;
using R2CSharp.Views;

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
            window.KeyDown += OnKeyDown;
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

        if (MainCarousel.Children.FirstOrDefault() is not StandardPageView { DataContext: PageConfiguration firstPage } firstPageView) return;
        
        firstPage.SelectedIndex = -1;
        UpdateVisualSelection(firstPage);
            
        var buttons = new List<Button>();
        FindButtonsRecursively(firstPageView, buttons);
        foreach (var button in buttons) {
            button.Classes.Remove("selected");
        }
    }
    
    private void OnCarouselPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != CarouselControl.CurrentIndexProperty) return;
        UpdateNavigationState();
        
        var currentPageView = MainCarousel?.Children.FirstOrDefault() as StandardPageView;
        if (currentPageView?.DataContext is not PageConfiguration currentPage) return;
        
        currentPage.SelectedIndex = -1;
        UpdateVisualSelection(currentPage);
                    
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        foreach (var button in buttons) {
            button.Classes.Remove("selected");
        }

        if (_lastSelectionColumn < 0) return;
        var columns = currentPage.ActualColumns;
        var rows = currentPage.ActualRows;
        var totalButtons = currentPage.Options.Count;
                        
        int targetRow;
        if (MainCarousel != null && MainCarousel.CurrentIndex > _previousPageIndex) {
            targetRow = 0;
        }
        else {
            targetRow = rows - 1;
        }
                        
        var targetIndex = targetRow * columns + _lastSelectionColumn;
                        
        if (targetIndex >= 0 && targetIndex < totalButtons) {
            SetCurrentSelection(currentPage, targetIndex);
        }
        else {
            SetCurrentSelection(currentPage, 0);
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
    
    private int _lastSelectionColumn = -1;
    private int _previousPageIndex;
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (MainCarousel == null) return;
        var currentPageView = MainCarousel.Children.FirstOrDefault() as Views.StandardPageView;

        var currentPage = currentPageView?.DataContext as PageConfiguration;
        if (currentPage == null) return;
        
        var currentSelection = GetCurrentSelection(currentPage);
        var totalButtons = currentPage.Options.Count;
        var columns = currentPage.ActualColumns;
        var rows = currentPage.ActualRows;
        
        switch (e.Key) {
            case Key.Up:
                e.Handled = true;
                if (currentSelection == -1) {
                    SetCurrentSelection(currentPage, 0);
                }
                else
                {
                    var currentRow = currentSelection / columns;
                    if (currentRow == 0) {
                        if (_viewModel?.CanGoPrevious == true) {
                            _lastSelectionColumn = currentSelection % columns;
                            _previousPageIndex = MainCarousel.CurrentIndex;
                            MainCarousel.Previous();
                            UpdateNavigationState();
                        }
                    }
                    else {
                        SetCurrentSelection(currentPage, currentSelection - columns);
                    }
                }
                break;
                
            case Key.Down:
                e.Handled = true;
                if (currentSelection == -1) {
                    SetCurrentSelection(currentPage, 0);
                }
                else {
                    var currentRow = currentSelection / columns;
                    if (currentRow == rows - 1) {
                        if (_viewModel?.CanGoNext == true) {
                            _lastSelectionColumn = currentSelection % columns;
                            _previousPageIndex = MainCarousel.CurrentIndex;
                            MainCarousel.Next();
                            UpdateNavigationState();
                        }
                    }
                    else {
                        SetCurrentSelection(currentPage, currentSelection + columns);
                    }
                }
                break;
                
            case Key.Left:
                e.Handled = true;
                switch (currentSelection) {
                    case -1:
                        SetCurrentSelection(currentPage, 0);
                        break;
                    case > 0:
                        SetCurrentSelection(currentPage, currentSelection - 1);
                        break;
                }
                break;
                
            case Key.Right:
                e.Handled = true;
                if (currentSelection == -1) {
                    SetCurrentSelection(currentPage, 0);
                }
                else if (currentSelection < totalButtons - 1) {
                    SetCurrentSelection(currentPage, currentSelection + 1);
                }
                break;
                
            case Key.Enter:
                e.Handled = true;
                if (currentSelection >= 0 && currentSelection < totalButtons) {
                    var buttons = new List<Button>();
                    if (currentPageView != null) FindButtonsRecursively(currentPageView, buttons);

                    if (currentSelection < buttons.Count) {
                        var selectedButton = buttons[currentSelection];
                        
                        selectedButton.Classes.Add("pressed");
                        
                        var selectedOption = currentPage.Options[currentSelection];
                        if (selectedOption.Command != null && selectedOption.Command.CanExecute(selectedOption)) {
                            selectedOption.Command.Execute(selectedOption);
                        }
                        
                        Task.Delay(150).ContinueWith(_ => {
                            Dispatcher.UIThread.Post(() => {
                                selectedButton.Classes.Remove("pressed");
                            });
                        });
                    }
                }
                break;

            case Key.Space:
                e.Handled = true;
                break;
            default:
                return;
        }
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

    private static int GetCurrentSelection(PageConfiguration page)
    {
        return page.SelectedIndex;
    }
    
    private void SetCurrentSelection(PageConfiguration page, int index)
    {
        if (page.SelectedIndex == index) return;
        page.SelectedIndex = index;
        UpdateVisualSelection(page);
    }
    
    private void UpdateVisualSelection(PageConfiguration page)
    {
        if (MainCarousel?.Children.FirstOrDefault() is not StandardPageView currentPageView) return;
        
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        
        foreach (var button in buttons) {
            button.Classes.Remove("selected");
        }
        
        if (page.SelectedIndex >= 0 && page.SelectedIndex < buttons.Count) {
            buttons[page.SelectedIndex].Classes.Add("selected");
        }
    }
    
    private static void FindButtonsRecursively(Control control, List<Button> buttons)
    {
        if (control is Button button && button.Name == "OptionButton") {
            buttons.Add(button);
        }
        
        foreach (var child in control.GetVisualChildren()) {
            if (child is Control childControl) {
                FindButtonsRecursively(childControl, buttons);
            }
        }
    }
    
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        if (this.GetVisualRoot() is Window r2CWindow) {
            r2CWindow.PointerWheelChanged -= OnPointerWheelChanged;
            r2CWindow.PointerPressed -= OnPointerPressed;
            r2CWindow.PointerReleased -= OnPointerReleased;
            r2CWindow.PointerMoved -= OnPointerMoved;
            r2CWindow.KeyDown -= OnKeyDown;
        }
        
        if (DataContext is PageViewModel r2CModel) {
            r2CModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
        
        if (MainCarousel != null) {
            MainCarousel.PropertyChanged -= OnCarouselPropertyChanged;
        }
        
        _scrollService.PageChangeRequested -= OnPageChangeRequested;
        _touchService.PageChangeRequested -= OnPageChangeRequested;
        
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