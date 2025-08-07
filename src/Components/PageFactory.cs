using R2CSharp.Models;

namespace R2CSharp.Components;

public class PageFactory(NyxIcons nyxIcons)
{
    public PageConfiguration CreateLaunchPage(List<RebootOption> options)
    {
        return new PageConfiguration("Launch", options, nyxIcons.UseFiveColumns)
        {
            SectionIcon = "fa-solid fa-rocket",
            EmptyMessage = "No launch options found",
            ThemeColor = nyxIcons.ThemeColor
        };
    }
    
    public PageConfiguration CreateConfigPage(List<RebootOption> options)
    {
        return new PageConfiguration("More Configurations", options, nyxIcons.UseFiveColumns)
        {
            SectionIcon = "fa-solid fa-cog",
            EmptyMessage = "No config options found",
            ThemeColor = nyxIcons.ThemeColor
        };
    }
    
    public PageConfiguration CreateUmsPage(List<RebootOption> options)
    {
        return new PageConfiguration("UMS (USB Mass Storage)", options)
        {
            SectionIcon = "fa-solid fa-hdd",
            EmptyMessage = "No UMS options available",
            ThemeColor = nyxIcons.ThemeColor
        };
    }
    
    public PageConfiguration CreateSystemPage(List<RebootOption> options)
    {
        return new PageConfiguration("System Options", options)
        {
            SectionIcon = "fa-solid fa-tools",
            EmptyMessage = "No system options available",
            ThemeColor = nyxIcons.ThemeColor
        };
    }
    
    public PageConfiguration CreateCustomPage(string sectionName, List<RebootOption> options, 
        string sectionIcon = "fa-solid fa-location-arrow", string emptyMessage = "No options found", 
        double iconFontSize = 35)
    {
        return new PageConfiguration(sectionName, options, nyxIcons.UseFiveColumns)
        {
            SectionIcon = sectionIcon,
            EmptyMessage = emptyMessage,
            ThemeColor = nyxIcons.ThemeColor
        };
    }
} 