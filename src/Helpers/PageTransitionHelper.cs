using R2CSharp.Models;

namespace R2CSharp.Helpers;

public class PageTransitionHelper
{
    public int CalculateTargetSelection(PageConfiguration currentPage, int lastSelectionColumn, 
        int currentPageIndex, int previousPageIndex)
    {
        if (lastSelectionColumn < 0) return -1;
        
        var columns = currentPage.ActualColumns;
        var rows = currentPage.ActualRows;
        var totalButtons = currentPage.Options.Count;
        
        int targetRow;
        if (currentPageIndex > previousPageIndex) {
            targetRow = 0;
        }
        else {
            targetRow = rows - 1;
        }
        
        var targetIndex = targetRow * columns + lastSelectionColumn;
        
        if (targetIndex >= 0 && targetIndex < totalButtons) {
            return targetIndex;
        }

        return 0;
    }
    
    public bool ShouldPreserveSelection(int lastSelectionColumn)
    {
        return lastSelectionColumn >= 0;
    }
} 