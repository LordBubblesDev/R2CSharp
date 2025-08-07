namespace R2CSharp.Models;

public class PageConfiguration
{
    public string SectionName { get; set; }
    public string SectionIcon { get; set; } = "fa-solid fa-location-arrow";
    public List<RebootOption> Options { get; set; }
    public string EmptyMessage { get; set; } = "No options found";
    private bool UseFiveColumns { get; }
    public string ThemeColor { get; set; } = "#4EC9B0";
    
    // Layout properties (calculated automatically)
    private int MaxColumns => UseFiveColumns ? 5 : 4;
    private static int MaxRows => 2;
    public int MaxItems => MaxColumns * MaxRows;
    public int ActualColumns { get; set; }
    public int ActualRows { get; set; }
    public bool IsSingleRow { get; set; }
    public int SelectedIndex { get; set; } = -1; // -1 means no selection
    
    public PageConfiguration(string sectionName, List<RebootOption> options, bool useFiveColumns = false)
    {
        SectionName = sectionName;
        Options = options;
        UseFiveColumns = useFiveColumns;

        CalculateLayout();
    }
    
    private void CalculateLayout()
    {
        var itemCount = Options.Count;

        switch (itemCount) {
            // Enforce specific grid layouts
            case <= 3:
                // 3 buttons: 3 columns, 1 row
                ActualColumns = 3;
                ActualRows = 1;
                IsSingleRow = true;
                break;
            case <= 8:
                // 4-8 buttons: 4 columns, 2 rows (show 8 slots)
                ActualColumns = 4;
                ActualRows = 2;
                IsSingleRow = false;
                break;
            default:
                // 9+ buttons: 5 columns, 2 rows (show 10 slots)
                ActualColumns = 5;
                ActualRows = 2;
                IsSingleRow = false;
                // Limit to max items
                Options = Options.Take(10).ToList();
                break;
        }
    }
} 