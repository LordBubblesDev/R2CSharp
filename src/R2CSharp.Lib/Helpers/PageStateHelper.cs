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
            carousel.AddPage(new RebootOptionPageView { DataContext = page });
        }
        
        UpdateNavigationState();
        
        if (carousel.Children.FirstOrDefault() is RebootOptionPageView { DataContext: PageConfiguration firstPage } firstPageView) {
            firstPage.SelectedIndex = -1;
            ClearAllSelections(firstPageView);
        }
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
                if (currentSelection == -1) SetSelection(currentPage, 0);
                else if (currentSelection / columns == 0) {
                    if (canGoPrevious) { _lastColumn = currentSelection % columns; NavigateToPage(-1); }
                } else {
                    SetSelection(currentPage, Math.Max(currentSelection - columns, 0));
                }
                break;
            case Key.Down:
                if (currentSelection == -1) SetSelection(currentPage, 0);
                else if (currentSelection / columns == rows - 1) {
                    if (canGoNext) { _lastColumn = currentSelection % columns; NavigateToPage(1); }
                } else {
                    SetSelection(currentPage, Math.Min(currentSelection + columns, totalButtons - 1));
                }
                break;
            case Key.Left:
                if (currentSelection > 0) SetSelection(currentPage, currentSelection - 1);
                break;
            case Key.Right:
                if (currentSelection < totalButtons - 1) SetSelection(currentPage, currentSelection + 1);
                break;
            case Key.Enter:
                if (currentSelection >= 0 && currentSelection < totalButtons) HandleButtonPress(currentSelection);
                break;
            default:
                e.Handled = false;
                break;
        }
    }
    
    private void SetSelection(PageConfiguration page, int index)
    {
        if (page.SelectedIndex == index) return;
        page.SelectedIndex = index;
        if (page.ActualColumns > 0) _lastColumn = index % page.ActualColumns;
        
        if (carousel?.Children.FirstOrDefault() is RebootOptionPageView currentPageView) {
            UpdateVisualSelection(page, currentPageView);
        }
    }
    
    private void NavigateToPage(int direction)
    {
        if (carousel == null || viewModel == null) return;
        if ((direction > 0 && !viewModel.CanGoNext) || (direction < 0 && !viewModel.CanGoPrevious)) return;
        
        if (direction > 0) carousel.Next();
        else carousel.Previous();
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
        
        var targetIndex = currentPageIndex > previousPageIndex 
            ? Math.Min(lastColumn, totalButtons - 1)
            : Math.Min(lastColumn + columns, totalButtons - 1);
        
        return targetIndex;
    }
    
    private static void UpdateVisualSelection(PageConfiguration page, RebootOptionPageView currentPageView)
    {
        var buttons = FindButtonsInPage(currentPageView);
        foreach (var button in buttons) button.Classes.Remove("selected");
        if (page.SelectedIndex >= 0 && page.SelectedIndex < buttons.Count) {
            buttons[page.SelectedIndex].Classes.Add("selected");
        }
    }
    
    private static void ClearAllSelections(RebootOptionPageView currentPageView)
    {
        var buttons = FindButtonsInPage(currentPageView);
        foreach (var button in buttons) button.Classes.Remove("selected");
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
            Avalonia.Threading.Dispatcher.UIThread.Post(() => selectedButton.Classes.Remove("pressed"));
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
        if (control is Button button && button.Name == "OptionButton") buttons.Add(button);
        foreach (var child in control.GetVisualChildren()) {
            if (child is Control childControl) FindButtonsRecursively(childControl, buttons);
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
    
    public void NavigatePrevious() => NavigateToPage(-1);
    public void NavigateNext() => NavigateToPage(1);
    public void HandleScrollPageChange(int direction) => NavigateToPage(direction > 0 ? -1 : 1);
} 