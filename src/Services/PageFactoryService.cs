using R2CSharp.Models;
using System.Windows.Input;

namespace R2CSharp.Services;

public class PageFactoryService
{
    private readonly IconService _iconService;
    
    public PageFactoryService(IconService iconService)
    {
        _iconService = iconService;
    }
    
    public PageConfiguration CreateLaunchPage(List<RebootOption> options)
    {
        return new PageConfiguration("Launch", options, _iconService.UseFiveColumns, 35)
        {
            SectionIcon = "fa-solid fa-rocket",
            EmptyMessage = "No launch options found",
            ThemeColor = _iconService.ThemeColor
        };
    }
    
    public PageConfiguration CreateConfigPage(List<RebootOption> options)
    {
        return new PageConfiguration("More Configurations", options, _iconService.UseFiveColumns, 35)
        {
            SectionIcon = "fa-solid fa-cog",
            EmptyMessage = "No config options found",
            ThemeColor = _iconService.ThemeColor
        };
    }
    
    public PageConfiguration CreateUmsPage(List<RebootOption> options)
    {
        return new PageConfiguration("UMS (USB Mass Storage)", options, _iconService.UseFiveColumns, 35)
        {
            SectionIcon = "fa-solid fa-hdd",
            EmptyMessage = "No UMS options available",
            ThemeColor = _iconService.ThemeColor
        };
    }
    
    public PageConfiguration CreateSystemPage(List<RebootOption> options)
    {
        return new PageConfiguration("System Options", options, _iconService.UseFiveColumns, 35)
        {
            SectionIcon = "fa-solid fa-tools",
            EmptyMessage = "No system options available",
            ThemeColor = _iconService.ThemeColor
        };
    }
    
    public PageConfiguration CreateCustomPage(string sectionName, List<RebootOption> options, 
        string sectionIcon = "fa-solid fa-location-arrow", string emptyMessage = "No options found", 
        double iconFontSize = 35)
    {
        return new PageConfiguration(sectionName, options, _iconService.UseFiveColumns, iconFontSize)
        {
            SectionIcon = sectionIcon,
            EmptyMessage = emptyMessage,
            ThemeColor = _iconService.ThemeColor
        };
    }
} 