using R2CSharp.Models;

namespace R2CSharp.Services;

public class PageFactoryService(IconService iconService)
{
    public PageConfiguration CreateLaunchPage(List<RebootOption> options)
    {
        return new PageConfiguration("Launch", options, iconService.UseFiveColumns)
        {
            SectionIcon = "fa-solid fa-rocket",
            EmptyMessage = "No launch options found",
            ThemeColor = iconService.ThemeColor
        };
    }
    
    public PageConfiguration CreateConfigPage(List<RebootOption> options)
    {
        return new PageConfiguration("More Configurations", options, iconService.UseFiveColumns)
        {
            SectionIcon = "fa-solid fa-cog",
            EmptyMessage = "No config options found",
            ThemeColor = iconService.ThemeColor
        };
    }
    
    public PageConfiguration CreateUmsPage(List<RebootOption> options)
    {
        return new PageConfiguration("UMS (USB Mass Storage)", options, iconService.UseFiveColumns)
        {
            SectionIcon = "fa-solid fa-hdd",
            EmptyMessage = "No UMS options available",
            ThemeColor = iconService.ThemeColor
        };
    }
    
    public PageConfiguration CreateSystemPage(List<RebootOption> options)
    {
        return new PageConfiguration("System Options", options, iconService.UseFiveColumns)
        {
            SectionIcon = "fa-solid fa-tools",
            EmptyMessage = "No system options available",
            ThemeColor = iconService.ThemeColor
        };
    }
    
    public PageConfiguration CreateCustomPage(string sectionName, List<RebootOption> options, 
        string sectionIcon = "fa-solid fa-location-arrow", string emptyMessage = "No options found", 
        double iconFontSize = 35)
    {
        return new PageConfiguration(sectionName, options, iconService.UseFiveColumns)
        {
            SectionIcon = sectionIcon,
            EmptyMessage = emptyMessage,
            ThemeColor = iconService.ThemeColor
        };
    }
} 