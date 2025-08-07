using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.VisualTree;
using R2CSharp.Controls;
using R2CSharp.Services;
using System.Linq;

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
    }
    
    private void OnCarouselPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == CarouselControl.CurrentIndexProperty)
        {
            UpdateNavigationState();
            
            // Clear selection when changing pages
            if (MainCarousel != null)
            {
                var currentPageView = MainCarousel.Children.FirstOrDefault() as Views.StandardPageView;
                if (currentPageView?.DataContext is Models.PageConfiguration currentPage)
                {
                    currentPage.SelectedIndex = -1;
                    UpdateVisualSelection(currentPage);
                }
            }
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
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (MainCarousel == null) return;
        
        // Get the current page from the carousel's children
        var currentPageView = MainCarousel.Children.FirstOrDefault() as Views.StandardPageView;
        if (currentPageView == null) return;
        
        var currentPage = currentPageView.DataContext as Models.PageConfiguration;
        if (currentPage == null) return;
        
        var currentSelection = GetCurrentSelection(currentPage);
        var totalButtons = currentPage.Options.Count;
        var columns = currentPage.ActualColumns;
        var rows = currentPage.ActualRows;
        
        switch (e.Key)
        {
            case Key.Up:
                e.Handled = true;
                if (currentSelection == -1)
                {
                    // First arrow key press - select the first button
                    SetCurrentSelection(currentPage, 0);
                }
                else
                {
                    var currentRow = currentSelection / columns;
                    if (currentRow == 0)
                    {
                        // At top row, go to previous page
                        if (_viewModel?.CanGoPrevious == true)
                        {
                            MainCarousel.Previous();
                            UpdateNavigationState();
                        }
                    }
                    else
                    {
                        // Move up within current page
                        SetCurrentSelection(currentPage, currentSelection - columns);
                    }
                }
                break;
                
            case Key.Down:
                e.Handled = true;
                if (currentSelection == -1)
                {
                    // First arrow key press - select the first button
                    SetCurrentSelection(currentPage, 0);
                }
                else
                {
                    var currentRow = currentSelection / columns;
                    if (currentRow == rows - 1)
                    {
                        // At bottom row, go to next page
                        if (_viewModel?.CanGoNext == true)
                        {
                            MainCarousel.Next();
                            UpdateNavigationState();
                        }
                    }
                    else
                    {
                        // Move down within current page
                        SetCurrentSelection(currentPage, currentSelection + columns);
                    }
                }
                break;
                
            case Key.Left:
                e.Handled = true;
                if (currentSelection == -1)
                {
                    SetCurrentSelection(currentPage, 0);
                }
                else if (currentSelection > 0)
                {
                    SetCurrentSelection(currentPage, currentSelection - 1);
                }
                break;
                
            case Key.Right:
                e.Handled = true;
                if (currentSelection == -1)
                {
                    SetCurrentSelection(currentPage, 0);
                }
                else if (currentSelection < totalButtons - 1)
                {
                    SetCurrentSelection(currentPage, currentSelection + 1);
                }
                break;
                
            case Key.Enter:
                e.Handled = true;
                if (currentSelection >= 0 && currentSelection < totalButtons)
                {
                    var selectedOption = currentPage.Options[currentSelection];
                    selectedOption.Command?.Execute(selectedOption);
                }
                break;
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

    private int GetCurrentSelection(Models.PageConfiguration page)
    {
        return page.SelectedIndex;
    }
    
    private void SetCurrentSelection(Models.PageConfiguration page, int index)
    {
        page.SelectedIndex = index;
        UpdateVisualSelection(page);
    }
    
    private void UpdateVisualSelection(Models.PageConfiguration page)
    {
        if (MainCarousel == null) return;
        
        // Get the current page from the carousel's children
        var currentPageView = MainCarousel.Children.FirstOrDefault() as Views.StandardPageView;
        if (currentPageView == null) return;
        
        // Find all buttons in the current page using visual tree traversal
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        
        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            if (i == page.SelectedIndex)
            {
                button.Classes.Add("selected");
            }
            else
            {
                button.Classes.Remove("selected");
            }
        }
    }
    
    private void FindButtonsRecursively(Control control, List<Button> buttons)
    {
        if (control is Button button && button.Name == "OptionButton")
        {
            buttons.Add(button);
        }
        
        foreach (var child in control.GetVisualChildren())
        {
            if (child is Control childControl)
            {
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