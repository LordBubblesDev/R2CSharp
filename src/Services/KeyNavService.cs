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
    
    public void HandleKeyDown(KeyEventArgs e, PageConfiguration currentPage, int currentSelection)
    {
        var totalButtons = currentPage.Options.Count;
        var columns = currentPage.ActualColumns;
        var rows = currentPage.ActualRows;
        
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (e.Key) {
            case Key.Up:
                e.Handled = true;
                HandleUpKey(currentPage, currentSelection, columns, rows);
                break;
                
            case Key.Down:
                e.Handled = true;
                HandleDownKey(currentPage, currentSelection, columns, rows);
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
    
    private void HandleUpKey(PageConfiguration currentPage, int currentSelection, int columns, int rows)
    {
        if (currentSelection == -1) {
            SetCurrentSelection(currentPage, 0);
        }
        else {
            var currentRow = currentSelection / columns;
            if (currentRow == 0) {
                _lastSelectionColumn = currentSelection % columns;
                _previousPageIndex = GetCurrentPageIndex();
                PageChangeRequested?.Invoke(-1);
            }
            else {
                SetCurrentSelection(currentPage, currentSelection - columns);
            }
        }
    }
    
    private void HandleDownKey(PageConfiguration currentPage, int currentSelection, int columns, int rows)
    {
        if (currentSelection == -1) {
            SetCurrentSelection(currentPage, 0);
        }
        else {
            var currentRow = currentSelection / columns;
            if (currentRow == rows - 1) {
                _lastSelectionColumn = currentSelection % columns;
                _previousPageIndex = GetCurrentPageIndex();
                PageChangeRequested?.Invoke(1);
            }
            else {
                SetCurrentSelection(currentPage, currentSelection + columns);
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