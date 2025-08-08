using R2CSharp.Lib.Models;

namespace R2CSharp.Lib.Components;

public class PageFactory(NyxIcons nyxIcons)
{
    /// <summary>
    /// Creates a standardized page configuration
    /// </summary>
    /// <param name="pageTitle">The title of the page</param>
    /// <param name="options">List of reboot options</param>
    /// <returns>A configured PageConfiguration</returns>
    public PageConfiguration CreatePage(string pageTitle, List<RebootOption> options)
    {
        return new PageConfiguration(pageTitle, options, nyxIcons.UseFiveColumns)
        {
            EmptyMessage = "No entries found",
            ThemeColor = nyxIcons.ThemeColor
        };
    }
}