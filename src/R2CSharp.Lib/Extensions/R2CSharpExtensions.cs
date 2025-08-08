using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using R2CSharp.Lib.Components;

namespace R2CSharp.Lib.Extensions;

/// <summary>
/// Extension methods to easily integrate R2CSharp.Lib into Avalonia applications
/// </summary>
public static class R2CSharpExtensions
{
    /// <summary>
    /// Adds R2CSharp theme resources to the application
    /// </summary>
    /// <param name="appBuilder">The AppBuilder instance</param>
    /// <returns>The AppBuilder instance for method chaining</returns>
    public static AppBuilder UseR2CSharp(this AppBuilder appBuilder)
    {
        return appBuilder.AfterSetup(_ =>
        {
            // Ensure resources are loaded before any views are created
            if (Application.Current?.Resources is not null)
            {
                var themeUri = new Uri("avares://R2CSharp.Lib/Themes/R2CSharpTheme.axaml");
                var themeResource = new ResourceInclude(themeUri)
                {
                    Source = themeUri
                };
                
                Application.Current.Resources.MergedDictionaries.Add(themeResource);
            }
        });
    }
    
    /// <summary>
    /// Adds R2CSharp theme resources and FontAwesome icons to the application
    /// </summary>
    /// <param name="appBuilder">The AppBuilder instance</param>
    /// <returns>The AppBuilder instance for method chaining</returns>
    public static AppBuilder UseR2CSharpWithIcons(this AppBuilder appBuilder)
    {
        // Initialize FontAwesome icons
        try
        {
            IconProvider.Current
                .Register(new FontAwesomeIconProvider(FontAwesomeJsonStreamProvider.Instance));
        }
        catch (Exception)
        {
            // FontAwesome already registered, ignore
        }
        
        // Then load theme resources
        return appBuilder.UseR2CSharp();
    }
}
