using Avalonia.Input;
using R2CSharp.Lib.Views;

namespace R2CSharp.Lib.Models;

public class NavigationContext(
    KeyEventArgs keyEventArgs,
    PageConfiguration currentPage,
    RebootOptionPageView currentPageView,
    int currentSelection,
    bool canGoPrevious,
    bool canGoNext)
{
    public KeyEventArgs KeyEventArgs { get; } = keyEventArgs;
    public PageConfiguration CurrentPage { get; } = currentPage;
    public RebootOptionPageView CurrentPageView { get; } = currentPageView;
    public int CurrentSelection { get; } = currentSelection;
    public bool CanGoPrevious { get; } = canGoPrevious;
    public bool CanGoNext { get; } = canGoNext;
    
    public void Deconstruct(
        out KeyEventArgs keyEventArgs,
        out PageConfiguration currentPage,
        out RebootOptionPageView currentPageView,
        out int currentSelection,
        out bool canGoPrevious,
        out bool canGoNext)
    {
        keyEventArgs = KeyEventArgs;
        currentPage = CurrentPage;
        currentPageView = CurrentPageView;
        currentSelection = CurrentSelection;
        canGoPrevious = CanGoPrevious;
        canGoNext = CanGoNext;
    }
}
