using Avalonia.Input;
using R2CSharp.Models;

namespace R2CSharp.Services;

public class KeyNavService
{
    private int _lastSelectionColumn = -1;
    private int _previousPageIndex;
    private int _currentPageIndex;
    
    public event Action<int>? SelectionChanged;
    public event Action<int>? PageChangeRequested;
    public event Action<int>? ButtonPressed;
    
    public void HandleKeyPressed(KeyEventArgs e, PageConfiguration currentPage, int currentSelection, bool canGoPrevious, bool canGoNext)
    {
        var totalButtons = currentPage.Options.Count;
        var columns = currentPage.ActualColumns;
        var rows = currentPage.ActualRows;
        
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (e.Key) {
            case Key.Up:
                e.Handled = true;
                HandleUpKey(currentPage, currentSelection, columns, rows, canGoPrevious);
                break;
                
            case Key.Down:
                e.Handled = true;
                HandleDownKey(currentPage, currentSelection, columns, rows, canGoNext);
                break;
                
            case Key.Left:
                e.Handled = true;
                HandleLeftKey(currentPage, currentSelection);
                break;
                
            case Key.Right:
                e.Handled = true;
                HandleRightKey(currentPage, currentSelection, totalButtons);
                break;
                
            case Key.Enter:
                e.Handled = true;
                HandleEnterKey(currentSelection, totalButtons);
                break;
        }
    }
    
    private void HandleUpKey(PageConfiguration currentPage, int currentSelection, int columns, int rows, bool canGoPrevious)
    {
        if (currentSelection == -1) {
            SetCurrentSelection(currentPage, 0);
            return;
        }

        var currentRow = currentSelection / columns;
        var currentColumn = currentSelection % columns;

        if (currentRow == 0) {
            if (!canGoPrevious) return;
            _lastSelectionColumn = currentColumn;
            _previousPageIndex = GetCurrentPageIndex();
            PageChangeRequested?.Invoke(-1);
        }
        else {
            var targetIndex = currentSelection - columns;
            var previousRowStart = (currentRow - 1) * columns;
            var previousRowEnd = Math.Min(previousRowStart + columns - 1, currentPage.Options.Count - 1);
            targetIndex = Math.Min(previousRowEnd, targetIndex);

            SetCurrentSelection(currentPage, targetIndex);
        }
    }
    
    private void HandleDownKey(PageConfiguration currentPage, int currentSelection, int columns, int rows, bool canGoNext)
    {
        if (currentSelection == -1) {
            SetCurrentSelection(currentPage, 0);
            return;
        }

        var totalButtons = currentPage.Options.Count;
        var currentRow = currentSelection / columns;
        var currentColumn = currentSelection % columns;

        if (currentRow == rows - 1) {
            if (!canGoNext) return;
            _lastSelectionColumn = currentColumn;
            _previousPageIndex = GetCurrentPageIndex();
            PageChangeRequested?.Invoke(1);
        }
        else {
            var targetIndex = currentSelection + columns;

            if (targetIndex >= totalButtons) {
                var lastIndexInRow = totalButtons - 1;
                SetCurrentSelection(currentPage, lastIndexInRow);
            }
            else {
                SetCurrentSelection(currentPage, targetIndex);
            }
        }
    }
    
    private void HandleLeftKey(PageConfiguration currentPage, int currentSelection)
    {
        switch (currentSelection) {
            case -1:
                SetCurrentSelection(currentPage, 0);
                break;
            case > 0:
                SetCurrentSelection(currentPage, currentSelection - 1);
                break;
        }
    }
    
    private void HandleRightKey(PageConfiguration currentPage, int currentSelection, int totalButtons)
    {
        if (currentSelection == -1) {
            SetCurrentSelection(currentPage, 0);
        }
        else if (currentSelection < totalButtons - 1) {
            SetCurrentSelection(currentPage, currentSelection + 1);
        }
    }
    
    private void HandleEnterKey(int currentSelection, int totalButtons)
    {
        if (currentSelection >= 0 && currentSelection < totalButtons) {
            ButtonPressed?.Invoke(currentSelection);
        }
    }
    
    private void SetCurrentSelection(PageConfiguration page, int index)
    {
        if (page.SelectedIndex == index) return;
        page.SelectedIndex = index;
        SelectionChanged?.Invoke(index);
    }
    
    private int GetCurrentPageIndex() => _currentPageIndex;
    public int GetLastSelectionColumn() => _lastSelectionColumn;
    public int GetPreviousPageIndex() => _previousPageIndex;
    
    public void SetPageIndex(int index)
    {
        _previousPageIndex = _currentPageIndex;
        _currentPageIndex = index;
    }
} 