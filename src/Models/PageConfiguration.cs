namespace R2CSharp.Models;

public class PageConfiguration
{
    public string SectionName { get; set; }
    public string SectionIcon { get; set; } = "fa-solid fa-location-arrow";
    public List<RebootOption> Options { get; set; }
    public string EmptyMessage { get; set; } = "No options found";
    private bool UseFiveColumns { get; }
    public string ThemeColor { get; set; } = "#4EC9B0";
    
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
        var maxColumns = UseFiveColumns ? 5 : 4;
        var maxItems = maxColumns * 2;

        switch (itemCount) {
            case <= 3:
                ActualColumns = 3;
                ActualRows = 1;
                IsSingleRow = true;
                break;
            case <= 4:
                ActualColumns = 4;
                ActualRows = 1;
                IsSingleRow = true;
                break;
            case <= 8:
                ActualColumns = 4;
                ActualRows = 2;
                IsSingleRow = false;
                break;
            default:
                ActualColumns = maxColumns;
                ActualRows = 2;
                IsSingleRow = false;
                Options = Options.Take(maxItems).ToList();
                break;
        }
    }
} 