using R2CSharp.Models;

namespace R2CSharp.Components;

public class PageFactory(NyxIcons nyxIcons)
{
    private const string EmptyMessage = "No options found";

    public PageConfiguration CreateLaunchPage(List<RebootOption> options)
    {
        return new PageConfiguration("Launch", options, nyxIcons.UseFiveColumns)
        {
            EmptyMessage = "No launch options found",
            ThemeColor = nyxIcons.ThemeColor
        };
    }
    
    public PageConfiguration CreateConfigPage(List<RebootOption> options)
    {
        return new PageConfiguration("More Configurations", options, nyxIcons.UseFiveColumns)
        {
            EmptyMessage = "No config options found",
            ThemeColor = nyxIcons.ThemeColor
        };
    }
    
    public PageConfiguration CreateUmsPage(List<RebootOption> options)
    {
        return new PageConfiguration("UMS (USB Mass Storage)", options)
        {
            EmptyMessage = EmptyMessage,
            ThemeColor = nyxIcons.ThemeColor
        };
    }
    
    public PageConfiguration CreateSystemPage(List<RebootOption> options)
    {
        return new PageConfiguration("System Options", options)
        {
            EmptyMessage = EmptyMessage,
            ThemeColor = nyxIcons.ThemeColor
        };
    }
    
    public PageConfiguration CreateCustomPage(string sectionName, List<RebootOption> options, 
        string sectionIcon = "fa-solid fa-location-arrow", string emptyMessage = "No options found", 
        double iconFontSize = 35)
    {
        return new PageConfiguration(sectionName, options, nyxIcons.UseFiveColumns)
        {
            EmptyMessage = emptyMessage
        };
    }
} 