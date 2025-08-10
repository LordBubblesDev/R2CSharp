using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using R2CSharp.Lib.Controls;
using R2CSharp.Lib.Models;
using R2CSharp.Lib.ViewModels;
using R2CSharp.Lib.Views;

namespace R2CSharp.Lib.Helpers;

public class PageStateHelper(
    CarouselPageViewModel? viewModel,
    CarouselControl? carousel)
{
    private int _lastColumn = -1;
    private int _previousPageIndex = -1;
    
    public void InitializePages()
    {
        if (viewModel == null || carousel == null) return;
        
        foreach (var page in viewModel.Pages) {
            var pageView = new RebootOptionPageView { DataContext = page };
            carousel.AddPage(pageView);
        }
        
        UpdateNavigationState();
        
        if (carousel.Children.FirstOrDefault() is not RebootOptionPageView { DataContext: PageConfiguration firstPage } firstPageView) return;
        
        firstPage.SelectedIndex = -1;
        ClearAllSelections(firstPageView);
    }
    
    public void HandleCarouselChange()
    {
        UpdateNavigationState();
        UpdateSelectionAfterPageChange();
    }
    
    public void HandleKeyNavigation(KeyEventArgs e, PageConfiguration currentPage, int currentSelection, bool canGoPrevious, bool canGoNext)
    {
        var totalButtons = currentPage.Options.Count;
        var columns = currentPage.ActualColumns;
        var rows = currentPage.ActualRows;
        
        e.Handled = true;
        
        switch (e.Key) {
            case Key.Up:
                HandleUpKey(currentPage, currentSelection, columns, rows, canGoPrevious);
                break;
            case Key.Down:
                HandleDownKey(currentPage, currentSelection, columns, rows, canGoNext);
                break;
            case Key.Left:
                HandleHorizontalKey(currentPage, currentSelection, -1, totalButtons);
                break;
            case Key.Right:
                HandleHorizontalKey(currentPage, currentSelection, 1, totalButtons);
                break;
            case Key.Enter:
                if (currentSelection >= 0 && currentSelection < totalButtons) {
                    HandleButtonPress(currentSelection);
                }
                break;
            default:
                e.Handled = false;
                break;
        }
    }
    
    private void HandleUpKey(PageConfiguration currentPage, int currentSelection, int columns, int rows, bool canGoPrevious)
    {
        if (currentSelection == -1) {
            SetSelection(currentPage, 0);
            return;
        }

        var currentRow = currentSelection / columns;
        var currentColumn = currentSelection % columns;

        if (currentRow == 0) {
            if (!canGoPrevious) return;
            _lastColumn = currentColumn;
            NavigateToPage(-1);
        }
        else {
            var targetIndex = Math.Max(currentSelection - columns, 0);
            SetSelection(currentPage, targetIndex);
        }
    }
    
    private void HandleDownKey(PageConfiguration currentPage, int currentSelection, int columns, int rows, bool canGoNext)
    {
        if (currentSelection == -1) {
            SetSelection(currentPage, 0);
            return;
        }

        var totalButtons = currentPage.Options.Count;
        var currentRow = currentSelection / columns;
        var currentColumn = currentSelection % columns;

        if (currentRow == rows - 1) {
            if (!canGoNext) return;
            _lastColumn = currentColumn;
            NavigateToPage(1);
        }
        else {
            var targetIndex = Math.Min(currentSelection + columns, totalButtons - 1);
            SetSelection(currentPage, targetIndex);
        }
    }
    
    private void HandleHorizontalKey(PageConfiguration currentPage, int currentSelection, int direction, int totalButtons)
    {
        if (currentSelection == -1) {
            SetSelection(currentPage, 0);
            return;
        }
        
        var targetIndex = currentSelection + direction;
        if (targetIndex >= 0 && targetIndex < totalButtons) {
            SetSelection(currentPage, targetIndex);
        }
    }
    
    private void SetSelection(PageConfiguration page, int index)
    {
        if (page.SelectedIndex == index) return;
        
        page.SelectedIndex = index;
        
        var columns = page.ActualColumns;
        if (columns > 0) {
            _lastColumn = index % columns;
        }
        
        if (carousel?.Children.FirstOrDefault() is RebootOptionPageView currentPageView) {
            UpdateVisualSelection(page, currentPageView);
        }
    }
    
    private void NavigateToPage(int direction)
    {
        if (carousel == null || viewModel == null) return;
        
        var canNavigate = direction > 0 ? viewModel.CanGoNext : viewModel.CanGoPrevious;
        if (!canNavigate) return;
        
        if (direction > 0) {
            carousel.Next();
        } else {
            carousel.Previous();
        }
    }
    
    private void UpdateSelectionAfterPageChange()
    {
        if (carousel?.Children.FirstOrDefault() is not RebootOptionPageView currentPageView) return;
        if (currentPageView.DataContext is not PageConfiguration currentPage) return;
        
        var currentPageIndex = carousel?.CurrentIndex ?? 0;
        
        currentPage.SelectedIndex = -1;
        ClearAllSelections(currentPageView);
        
        if (_lastColumn >= 0) {
            var targetIndex = CalculateTargetSelection(currentPage, _lastColumn, currentPageIndex, _previousPageIndex);
            if (targetIndex >= 0) {
                currentPage.SelectedIndex = targetIndex;
                UpdateVisualSelection(currentPage, currentPageView);
            }
        }
        
        _previousPageIndex = currentPageIndex;
    }
    
    private static int CalculateTargetSelection(PageConfiguration currentPage, int lastColumn, int currentPageIndex, int previousPageIndex)
    {
        var columns = currentPage.ActualColumns;
        var totalButtons = currentPage.Options.Count;
        
        if (columns == 0 || totalButtons == 0) return 0;
        
        int targetIndex;
        
        if (currentPageIndex > previousPageIndex) {
            targetIndex = Math.Min(lastColumn, totalButtons - 1);
        } else {
            targetIndex = lastColumn + columns;
            
            if (targetIndex >= totalButtons) {
                targetIndex = totalButtons - 1;
            }
        }
        
        return targetIndex;
    }
    
    private static void UpdateVisualSelection(PageConfiguration page, RebootOptionPageView currentPageView)
    {
        var buttons = FindButtonsInPage(currentPageView);
        
        foreach (var button in buttons) {
            button.Classes.Remove("selected");
        }
        
        if (page.SelectedIndex >= 0 && page.SelectedIndex < buttons.Count) {
            buttons[page.SelectedIndex].Classes.Add("selected");
        }
    }
    
    private static void ClearAllSelections(RebootOptionPageView currentPageView)
    {
        var buttons = FindButtonsInPage(currentPageView);
        foreach (var button in buttons) {
            button.Classes.Remove("selected");
        }
    }

    private void HandleButtonPress(int buttonIndex)
    {
        if (carousel?.Children.FirstOrDefault() is not RebootOptionPageView currentPageView) return;
        if (currentPageView.DataContext is not PageConfiguration currentPage) return;
        
        var buttons = FindButtonsInPage(currentPageView);
        if (buttonIndex >= buttons.Count) return;
        
        var selectedButton = buttons[buttonIndex];
        var selectedOption = currentPage.Options[buttonIndex];
        
        selectedButton.Classes.Add("pressed");
        
        if (selectedOption.Command?.CanExecute(selectedOption) == true) {
            selectedOption.Command.Execute(selectedOption);
        }
        
        Task.Delay(150).ContinueWith(_ => {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                selectedButton.Classes.Remove("pressed");
            });
        });
    }
    
    private static List<Button> FindButtonsInPage(RebootOptionPageView currentPageView)
    {
        var buttons = new List<Button>();
        FindButtonsRecursively(currentPageView, buttons);
        return buttons;
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
    
    private void UpdateNavigationState()
    {
        if (viewModel == null || carousel == null) return;
        
        var currentIndex = carousel.CurrentIndex;
        var totalPages = viewModel.Pages.Count;
        
        viewModel.CanGoPrevious = currentIndex > 0;
        viewModel.CanGoNext = currentIndex < totalPages - 1;
    }
    
    public void NavigatePrevious()
    {
        if (carousel == null || viewModel == null) return;
        if (!viewModel.CanGoPrevious) return;
        
        carousel.Previous();
    }
    
    public void NavigateNext()
    {
        if (carousel == null || viewModel == null) return;
        if (!viewModel.CanGoNext) return;
        
        carousel.Next();
    }
    
    public void HandleScrollPageChange(int direction)
    {
        if (direction > 0) NavigatePrevious();
        else NavigateNext();
    }
} 