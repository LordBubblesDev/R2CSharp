using Avalonia.Input;
using R2CSharp.Lib.Controls;
using R2CSharp.Lib.Models;
using R2CSharp.Lib.Views;

namespace R2CSharp.Lib.Helpers;

public class SelectionHelper(
    CarouselControl? carousel)
{
    private int _lastColumn = -1;
    private int _previousPageIndex = -1;
    
    public void HandleKeyNavigation(NavigationContext context, Action<int> navigateToPage, Action<PageConfiguration, int> setSelection)
    {
        var (keyEventArgs, currentPage, currentPageView, currentSelection, canGoPrevious, canGoNext) = context;
        var totalButtons = currentPage.Options.Count;
        var columns = currentPage.ActualColumns;
        var rows = currentPage.ActualRows;
        
        keyEventArgs.Handled = true;
        
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (keyEventArgs.Key) {
            case Key.Up:
                if (currentSelection == -1) setSelection(currentPage, 0);
                else if (currentSelection / columns == 0) {
                    if (canGoPrevious) { _lastColumn = currentSelection % columns; navigateToPage(-1); }
                } else {
                    setSelection(currentPage, Math.Max(currentSelection - columns, 0));
                }
                break;
            case Key.Down:
                if (currentSelection == -1) setSelection(currentPage, 0);
                else if (currentSelection / columns == rows - 1) {
                    if (canGoNext) { _lastColumn = currentSelection % columns; navigateToPage(1); }
                } else {
                    setSelection(currentPage, Math.Min(currentSelection + columns, totalButtons - 1));
                }
                break;
            case Key.Left when currentSelection > 0:
                setSelection(currentPage, currentSelection - 1);
                break;
            case Key.Right when currentSelection < totalButtons - 1:
                setSelection(currentPage, currentSelection + 1);
                break;
            case Key.Enter when currentSelection >= 0 && currentSelection < totalButtons:
                ButtonHelper.HandleButtonPress(currentPage, currentPageView, currentSelection);
                break;
            default:
                keyEventArgs.Handled = false;
                break;
        }
    }
    
    public void UpdateSelectionAfterPageChange(PageConfiguration currentPage, RebootOptionPageView currentPageView)
    {
        var currentPageIndex = carousel?.CurrentIndex ?? 0;
        currentPage.SelectedIndex = -1;
        ButtonHelper.ClearAllSelections(currentPageView);
        
        if (_lastColumn >= 0) {
            var targetIndex = CalculateTargetSelection(currentPage, _lastColumn, currentPageIndex, _previousPageIndex);
            if (targetIndex >= 0) {
                currentPage.SelectedIndex = targetIndex;
                ButtonHelper.UpdateVisualSelection(currentPage, currentPageView);
            }
        }
        
        _previousPageIndex = currentPageIndex;
    }
    
    private static int CalculateTargetSelection(PageConfiguration currentPage, int lastColumn, int currentPageIndex, int previousPageIndex)
    {
        var columns = currentPage.ActualColumns;
        var totalButtons = currentPage.Options.Count;
        
        if (columns == 0 || totalButtons == 0) return 0;
        
        if (currentPageIndex > previousPageIndex) {
            return Math.Min(lastColumn, columns - 1);
        } else {
            var lastRow = (totalButtons - 1) / columns;
            var targetColumn = Math.Min(lastColumn, columns - 1);
            return Math.Min(lastRow * columns + targetColumn, totalButtons - 1);
        }
    }
    
    public void UpdateLastColumn(int index, int columns)
    {
        if (columns > 0) _lastColumn = index % columns;
    }
}
