using R2CSharp.Lib.Controls;
using R2CSharp.Lib.Models;
using R2CSharp.Lib.Services;
using R2CSharp.Lib.ViewModels;
using R2CSharp.Lib.Views;

namespace R2CSharp.Lib.Helpers;

public class PageStateHelper(
    CarouselPageViewModel? viewModel,
    CarouselControl? carousel,
    KeyNavService keyboardService)
{
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
        ButtonHelper.ClearAllSelections(firstPageView);
    }
    
    public void HandlePageChange()
    {
        UpdateNavigationState();
        
        var currentPageView = carousel?.Children.FirstOrDefault() as RebootOptionPageView;
        if (currentPageView?.DataContext is not PageConfiguration currentPage) return;
        
        currentPage.SelectedIndex = -1;
        ButtonHelper.ClearAllSelections(currentPageView);
        
        var lastColumn = keyboardService.GetLastSelectionColumn();
        if (lastColumn < 0) return;
        
        var targetIndex = CalculateTargetSelection(
            currentPage, lastColumn, carousel?.CurrentIndex ?? 0, keyboardService.GetPreviousPageIndex());
            
        if (targetIndex >= 0) {
            SetCurrentSelection(currentPage, targetIndex);
        }
    }
    
    public void HandleSelectionChange()
    {
        if (carousel?.Children.FirstOrDefault() is not RebootOptionPageView currentPageView) return;
        if (currentPageView.DataContext is not PageConfiguration currentPage) return;
        
        ButtonHelper.UpdateVisualSelection(currentPage, currentPageView);
    }
    
    public void HandleButtonPress(int buttonIndex)
    {
        if (carousel?.Children.FirstOrDefault() is not RebootOptionPageView currentPageView) return;
        if (currentPageView.DataContext is not PageConfiguration currentPage) return;
        
        var buttons = ButtonHelper.FindButtonsInPage(currentPageView);

        if (buttonIndex >= buttons.Count) return;
        var selectedButton = buttons[buttonIndex];
        ButtonHelper.ApplyPressedState(selectedButton);
        var selectedOption = currentPage.Options[buttonIndex];
            
        if (selectedOption.Command?.CanExecute(selectedOption) == true) {
            selectedOption.Command.Execute(selectedOption);
        }
            
        Task.Delay(150).ContinueWith(_ => {
            Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                ButtonHelper.RemovePressedState(selectedButton);
            });
        });
    }

    private void HandlePageChange(int direction)
    {
        if (carousel == null || viewModel == null) return;
        
        keyboardService.SetPageIndex(carousel.CurrentIndex);
        
        var canNavigate = direction > 0 ? viewModel.CanGoNext : viewModel.CanGoPrevious;
        if (!canNavigate) return;
        
        if (direction > 0) {
            carousel.Next();
        } else {
            carousel.Previous();
        }
        
        UpdateNavigationState();
    }
    
    public void HandleScrollPageChange(int direction) => HandlePageChange(-direction);
    public void HandleKeyboardPageChange(int direction) => HandlePageChange(direction);
    
    public void NavigatePrevious() => HandlePageChange(-1);
    public void NavigateNext() => HandlePageChange(1);
    
    private void UpdateNavigationState()
    {
        if (viewModel == null || carousel == null) return;
        
        var currentIndex = carousel.CurrentIndex;
        var totalPages = viewModel.Pages.Count;
        
        viewModel.CanGoPrevious = currentIndex > 0;
        viewModel.CanGoNext = currentIndex < totalPages - 1;
    }
    
    private void SetCurrentSelection(PageConfiguration page, int index)
    {
        if (page.SelectedIndex == index) return;
        page.SelectedIndex = index;
        
        if (carousel?.Children.FirstOrDefault() is RebootOptionPageView currentPageView)
        {
            ButtonHelper.UpdateVisualSelection(page, currentPageView);
        }
    }
    
    private static int CalculateTargetSelection(PageConfiguration currentPage, int lastSelectionColumn, int currentPageIndex, int previousPageIndex)
    {
        if (lastSelectionColumn < 0) return -1;
        
        var columns = currentPage.ActualColumns;
        var totalButtons = currentPage.Options.Count;
        
        if (columns == 0 || totalButtons == 0) return 0;

        var targetRow = currentPageIndex > previousPageIndex ? 0 : (totalButtons - 1) / columns;
        var startOfRow = targetRow * columns;
        var endOfRow = Math.Min(startOfRow + columns, totalButtons);

        var clampedColumn = Math.Min(lastSelectionColumn, endOfRow - startOfRow - 1);
        return startOfRow + clampedColumn;
    }
} 