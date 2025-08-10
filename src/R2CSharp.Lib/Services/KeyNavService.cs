using Avalonia.Input;
using R2CSharp.Lib.Models;

namespace R2CSharp.Lib.Services;

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
        e.Handled = true;
        
        switch (e.Key) {
            case Key.Up:
                HandleUpKey(currentPage, currentSelection, columns, rows, canGoPrevious);
                break;
            case Key.Down:
                HandleDownKey(currentPage, currentSelection, columns, rows, canGoNext);
                break;
            case Key.Left:
                HandleHorizontalKey(currentPage, currentSelection, -1);
                break;
            case Key.Right:
                HandleHorizontalKey(currentPage, currentSelection, 1, totalButtons);
                break;
            case Key.Enter:
                if (currentSelection >= 0 && currentSelection < totalButtons) {
                    ButtonPressed?.Invoke(currentSelection);
                }
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
            _previousPageIndex = _currentPageIndex;
            PageChangeRequested?.Invoke(-1);
        }
        else {
            var targetIndex = Math.Max(currentSelection - columns, 0);
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
            _previousPageIndex = _currentPageIndex;
            PageChangeRequested?.Invoke(1);
        }
        else {
            var targetIndex = Math.Min(currentSelection + columns, totalButtons - 1);
            SetCurrentSelection(currentPage, targetIndex);
        }
    }
    
    private void HandleHorizontalKey(PageConfiguration currentPage, int currentSelection, int direction, int? totalButtons = null)
    {
        if (currentSelection == -1) {
            SetCurrentSelection(currentPage, 0);
            return;
        }
        
        if (totalButtons.HasValue) {
            var targetIndex = currentSelection + direction;
            if (targetIndex >= 0 && targetIndex < totalButtons.Value) {
                SetCurrentSelection(currentPage, targetIndex);
            }
        } else {
            var targetIndex = currentSelection + direction;
            if (targetIndex >= 0) {
                SetCurrentSelection(currentPage, targetIndex);
            }
        }
    }
    
    private void SetCurrentSelection(PageConfiguration page, int index)
    {
        if (page.SelectedIndex == index) return;
        page.SelectedIndex = index;
        SelectionChanged?.Invoke(index);
    }
    
    public int GetLastSelectionColumn() => _lastSelectionColumn;
    public int GetPreviousPageIndex() => _previousPageIndex;
    
    public void SetPageIndex(int index)
    {
        _previousPageIndex = _currentPageIndex;
        _currentPageIndex = index;
    }
} 