using Avalonia.Media.Imaging;
using System.Windows.Input;
using Avalonia;

namespace R2CSharp.Models;

public class PageConfiguration
{
    public string SectionName { get; set; } = string.Empty;
    public string SectionIcon { get; set; } = "fa-solid fa-location-arrow";
    public List<RebootOption> Options { get; set; } = new();
    public string EmptyMessage { get; set; } = "No options found";
    public double IconFontSize { get; set; } = 35;
    public bool UseFiveColumns { get; set; } = false;
    public string ThemeColor { get; set; } = "#4EC9B0";
    
    // Layout properties (calculated automatically)
    public int MaxColumns => UseFiveColumns ? 5 : 4;
    public int MaxRows => 2;
    public int MaxItems => MaxColumns * MaxRows;
    public int ActualColumns { get; set; }
    public int ActualRows { get; set; }
    
    public PageConfiguration(string sectionName, List<RebootOption> options, bool useFiveColumns = false, double iconFontSize = 35)
    {
        SectionName = sectionName;
        Options = options;
        UseFiveColumns = useFiveColumns;
        IconFontSize = iconFontSize;
        
        CalculateLayout();
    }
    
    private void CalculateLayout()
    {
        var itemCount = Options.Count;
        
        // Enforce specific grid layouts
        if (itemCount <= 3)
        {
            // 3 buttons: 3 columns, 1 row
            ActualColumns = 3;
            ActualRows = 1;
        }
        else if (itemCount <= 8)
        {
            // 4-8 buttons: 4 columns, 2 rows (show 8 slots)
            ActualColumns = 4;
            ActualRows = 2;
        }
        else
        {
            // 9+ buttons: 5 columns, 2 rows (show 10 slots)
            ActualColumns = 5;
            ActualRows = 2;
            // Limit to max items
            Options = Options.Take(10).ToList();
        }
    }
} 