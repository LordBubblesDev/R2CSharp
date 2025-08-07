using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using R2CSharp.Lib.Components;

namespace R2CSharp.Lib;

/// <summary>
/// Extension methods to easily integrate R2CSharp.Lib into Avalonia applications
/// </summary>
public static class R2CSharpExtensions
{
    /// <summary>
    /// Adds R2CSharp theme resources and icon provider to the application
    /// </summary>
    /// <param name="appBuilder">The AppBuilder instance</param>
    /// <returns>The AppBuilder instance for method chaining</returns>
    public static AppBuilder UseR2CSharp(this AppBuilder appBuilder)
    {
        // Initialize FontAwesome icons (required for R2CSharp components)
        IconProvider.Current
            .Register(new FontAwesomeIconProvider(FontAwesomeJsonStreamProvider.Instance));

        return appBuilder.AfterSetup(_ =>
        {
            // Add R2CSharp theme resources to the application
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
    /// Adds only the R2CSharp theme resources without FontAwesome icons
    /// (Use this if you've already initialized FontAwesome elsewhere)
    /// </summary>
    /// <param name="appBuilder">The AppBuilder instance</param>
    /// <returns>The AppBuilder instance for method chaining</returns>
    public static AppBuilder UseR2CSharpTheme(this AppBuilder appBuilder)
    {
        return appBuilder.AfterSetup(_ =>
        {
            // Add R2CSharp theme resources to the application
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
}
