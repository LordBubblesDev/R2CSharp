using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using R2CSharp.Lib.Views;
#if COLORTEST
using R2CSharp.Tests;
#endif

namespace R2CSharp.Lib;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
#if COLORTEST
            var window = new MainWindow { Content = new ColorTestView() };
#else
            var window = new MainWindow();
#endif
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}