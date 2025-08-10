using R2CSharp.Lib.Controls;
using R2CSharp.Lib.ViewModels;

namespace R2CSharp.Lib.Helpers;

public class NavigationHelper(
    CarouselPageViewModel? viewModel,
    CarouselControl? carousel)
{
    public void UpdateNavigationState()
    {
        if (viewModel == null || carousel == null) return;
        var currentIndex = carousel.CurrentIndex;
        var totalPages = viewModel.Pages.Count;
        viewModel.CanGoPrevious = currentIndex > 0;
        viewModel.CanGoNext = currentIndex < totalPages - 1;
    }
    
    public void NavigateToPage(int direction)
    {
        if (carousel == null || viewModel == null) return;
        switch (direction) {
            case > 0 when !viewModel.CanGoNext:
            case < 0 when !viewModel.CanGoPrevious:
                return;
            case > 0:
                carousel.Next();
                break;
            default:
                carousel.Previous();
                break;
        }
    }
    
    public void NavigatePrevious() => NavigateToPage(-1);
    public void NavigateNext() => NavigateToPage(1);
    public void HandleScrollPageChange(int direction) => NavigateToPage(direction > 0 ? -1 : 1);
}
