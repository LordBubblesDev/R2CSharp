using R2CSharp.Controls;
using R2CSharp.Models;
using R2CSharp.Services;
using R2CSharp.Views;

namespace R2CSharp.Helpers;

public class PageStateHelper(
    PageViewModel? viewModel,
    CarouselControl? carousel,
    ButtonHelper buttonHelper,
    PageTransitionHelper pageTransitionHelper,
    KeyNavService keyboardService)
{
    public void InitializePages()
    {
        if (viewModel == null || carousel == null) return;
        
        foreach (var page in viewModel.Pages)
        {
            var pageView = new StandardPageView { DataContext = page };
            carousel.AddPage(pageView);
        }
        
        UpdateNavigationState();
        
        if (carousel.Children.FirstOrDefault() is StandardPageView firstPageView &&
            firstPageView.DataContext is PageConfiguration firstPage)
        {
            firstPage.SelectedIndex = -1;
            buttonHelper.ClearAllSelections(firstPageView);
        }
    }
    
    public void HandlePageChange()
    {
        UpdateNavigationState();
        
        var currentPageView = carousel?.Children.FirstOrDefault() as StandardPageView;
        if (currentPageView?.DataContext is not PageConfiguration currentPage) return;
        
        currentPage.SelectedIndex = -1;
        buttonHelper.ClearAllSelections(currentPageView);
        
        var lastColumn = keyboardService.GetLastSelectionColumn();
        if (pageTransitionHelper.ShouldPreserveSelection(lastColumn))
        {
            var targetIndex = pageTransitionHelper.CalculateTargetSelection(
                currentPage, lastColumn, carousel?.CurrentIndex ?? 0, keyboardService.GetPreviousPageIndex());
            
            if (targetIndex >= 0)
            {
                SetCurrentSelection(currentPage, targetIndex);
            }
        }
    }
    
    public void HandleSelectionChange(int index)
    {
        if (carousel?.Children.FirstOrDefault() is not StandardPageView currentPageView) return;
        if (currentPageView.DataContext is not PageConfiguration currentPage) return;
        
        buttonHelper.UpdateVisualSelection(currentPage, currentPageView);
    }
    
    public void HandleButtonPress(int buttonIndex)
    {
        if (carousel?.Children.FirstOrDefault() is not StandardPageView currentPageView) return;
        if (currentPageView.DataContext is not PageConfiguration currentPage) return;
        
        var buttons = buttonHelper.FindButtonsInPage(currentPageView);
        
        if (buttonIndex < buttons.Count)
        {
            var selectedButton = buttons[buttonIndex];
            buttonHelper.ApplyPressedState(selectedButton);
            var selectedOption = currentPage.Options[buttonIndex];
            
            if (selectedOption.Command?.CanExecute(selectedOption) == true) {
                selectedOption.Command.Execute(selectedOption);
            }
            
            Task.Delay(150).ContinueWith(_ => {
                Avalonia.Threading.Dispatcher.UIThread.Post(() => {
                    buttonHelper.RemovePressedState(selectedButton);
                });
            });
        }
    }
    
    public void HandleKeyboardPageChange(int direction)
    {
        if (carousel == null) return;
        
        keyboardService.SetPageIndex(carousel.CurrentIndex);
        
        if (direction > 0)
        {
            carousel.Next();
        }
        else
        {
            carousel.Previous();
        }
        
        UpdateNavigationState();
    }
    
    public void HandleScrollPageChange(int direction)
    {
        if (carousel == null) return;
        
        if (direction > 0)
        {
            carousel.Previous();
        }
        else
        {
            carousel.Next();
        }
        
        UpdateNavigationState();
    }
    
    public void NavigatePrevious()
    {
        if (carousel == null) return;
        carousel.Previous();
        UpdateNavigationState();
    }
    
    public void NavigateNext()
    {
        if (carousel == null) return;
        carousel.Next();
        UpdateNavigationState();
    }
    
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
        
        if (carousel?.Children.FirstOrDefault() is StandardPageView currentPageView)
        {
            buttonHelper.UpdateVisualSelection(page, currentPageView);
        }
    }
} 