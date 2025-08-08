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
        // Note: This won't override existing FontAwesome registration
        try
        {
            IconProvider.Current
                .Register(new FontAwesomeIconProvider(FontAwesomeJsonStreamProvider.Instance));
        }
        catch (InvalidOperationException)
        {
            // FontAwesome already registered, ignore
        }
        catch (ArgumentException)
        {
            // FontAwesome already registered with same prefix, ignore
        }

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
    
    /// <summary>
    /// Manually load R2CSharp theme resources into an application
    /// Use this if the automatic loading doesn't work in your setup
    /// </summary>
    /// <param name="app">The Avalonia Application instance</param>
    public static void LoadR2CSharpResources(this Application app)
    {
        if (app.Resources is not null)
        {
            var themeUri = new Uri("avares://R2CSharp.Lib/Themes/R2CSharpTheme.axaml");
            var themeResource = new ResourceInclude(themeUri)
            {
                Source = themeUri
            };
            
            app.Resources.MergedDictionaries.Add(themeResource);
        }
    }
    
    /// <summary>
    /// Initialize FontAwesome icons for R2CSharp components
    /// Use this if you need to initialize icons separately
    /// </summary>
    public static void InitializeR2CSharpIcons()
    {
        try
        {
            IconProvider.Current
                .Register(new FontAwesomeIconProvider(FontAwesomeJsonStreamProvider.Instance));
        }
        catch (InvalidOperationException)
        {
            // FontAwesome already registered, ignore
        }
        catch (ArgumentException)
        {
            // FontAwesome already registered with same prefix, ignore
        }
    }
}
