using R2CSharp.Lib.Controls;
using R2CSharp.Lib.Models;
using R2CSharp.Lib.ViewModels;
using R2CSharp.Lib.Views;

namespace R2CSharp.Lib.Helpers;

public class PageStateHelper(
    CarouselPageViewModel? viewModel,
    CarouselControl? carousel)
{
    private readonly NavigationHelper _navigationHelper = new(viewModel, carousel);
    private readonly SelectionHelper _selectionHelper = new(carousel);
    
    public void InitializePages()
    {
        if (viewModel == null || carousel == null) return;
        
        foreach (var page in viewModel.Pages) {
            carousel.AddPage(new RebootOptionPageView { DataContext = page });
        }
        
        _navigationHelper.UpdateNavigationState();

        if (carousel.Children.FirstOrDefault() is not RebootOptionPageView {
                DataContext: PageConfiguration firstPage
            } firstPageView) return;
        firstPage.SelectedIndex = -1;
        ButtonHelper.ClearAllSelections(firstPageView);
    }
    
    public void HandleCarouselChange()
    {
        _navigationHelper.UpdateNavigationState();
        if (carousel?.Children.FirstOrDefault() is RebootOptionPageView currentPageView && 
            currentPageView.DataContext is PageConfiguration currentPage) {
            _selectionHelper.UpdateSelectionAfterPageChange(currentPage, currentPageView);
        }
    }
    
    public void HandleKeyNavigation(NavigationContext context)
    {
        _selectionHelper.HandleKeyNavigation(context, _navigationHelper.NavigateToPage, SetSelection);
    }
    
    private void SetSelection(PageConfiguration page, int index)
    {
        if (page.SelectedIndex == index) return;
        page.SelectedIndex = index;
        _selectionHelper.UpdateLastColumn(index, page.ActualColumns);
        
        if (carousel?.Children.FirstOrDefault() is RebootOptionPageView currentPageView) {
            ButtonHelper.UpdateVisualSelection(page, currentPageView);
        }
    }
    
    public void NavigatePrevious() => _navigationHelper.NavigatePrevious();
    public void NavigateNext() => _navigationHelper.NavigateNext();
    public void HandleScrollPageChange(int direction) => _navigationHelper.HandleScrollPageChange(direction);
} 