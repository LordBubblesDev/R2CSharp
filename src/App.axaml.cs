using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using R2CSharp.Views;
#if COLORTEST
using R2CSharp.Tests;
#endif

namespace R2CSharp;

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
            desktop.MainWindow = new MainWindow { Content = new ColorTestView() };
#else
            desktop.MainWindow = new MainWindow();
#endif
        }

        base.OnFrameworkInitializationCompleted();
    }
}