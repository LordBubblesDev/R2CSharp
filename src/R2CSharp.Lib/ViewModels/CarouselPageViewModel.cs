using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using R2CSharp.Lib.Components;
using R2CSharp.Lib.Helpers;
using R2CSharp.Lib.Models;
using R2CSharp.Lib.Services;

namespace R2CSharp.Lib.ViewModels;

public partial class CarouselPageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<PageConfiguration> _pages = [];
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _themeColor = "#00FFC8"; // hekate's default theme color
    [ObservableProperty] private bool _canGoPrevious;
    [ObservableProperty] private bool _canGoNext = true;

    private readonly BootDiskService _bootDiskService = new();
    private RebootOptionsProvider? _rebootOptionsService;
    private PageFactory? _pageFactory;
    private NyxIcons? _iconService;

    public async Task LoadAsync()
    {
        if (!IsLoading) { return; }
        
        try {
            await _bootDiskService.InitializeBootDiskAsync();
            _iconService = await Task.Run(() => new NyxIcons(_bootDiskService.BootDiskPath));
            await Dispatcher.UIThread.InvokeAsync(() => { ThemeColor = _iconService.ThemeColor; });
            
            _rebootOptionsService = new RebootOptionsProvider(_bootDiskService.BootDiskPath, _iconService);
            _pageFactory = new PageFactory(_iconService);
            
            await LoadRebootOptionsAsync();
            await Dispatcher.UIThread.InvokeAsync(() => { OnPropertyChanged(nameof(Pages)); });
        }
        catch (Exception ex) {
            Console.WriteLine($"[CarouselPageViewModel] Error during initialization: {ex.Message}");
        }
        finally {
            await Dispatcher.UIThread.InvokeAsync(() => { IsLoading = false; });
        }
    }

    private async Task LoadRebootOptionsAsync()
    {
        await Task.Run(LoadRebootOptions);
        await Dispatcher.UIThread.InvokeAsync(() => { OnPropertyChanged(nameof(Pages)); });
    }

    private void LoadRebootOptions()
    {
        Pages.Clear();

        var launchOptions = _rebootOptionsService!.LoadLaunchOptions();
        var configOptions = _rebootOptionsService.LoadConfigOptions();
        var umsOptions = RebootOptionsProvider.LoadUmsOptions();
        
        AssignCommands(launchOptions, SelectLaunchOptionCommand);
        AssignCommands(configOptions, SelectConfigOptionCommand);
        AssignCommands(umsOptions, SelectUmsOptionCommand);
        
        var systemOptions = new List<RebootOption>
        {
            new() { Name = "Hekate Bootloader", Command = RebootToBootloaderCommand, FallbackIcon = "fa-solid fa-boot" },
            new() { Name = "Reboot", Command = NormalRebootCommand, FallbackIcon = "fa-solid fa-rotate" },
            new() { Name = "Shutdown", Command = ShutdownCommand, FallbackIcon = "fa-solid fa-power-off" }
        };
        
        var launchPage = _pageFactory!.CreatePage("Launch", launchOptions);
        var configPage = _pageFactory.CreatePage("More Configurations", configOptions);
        var umsPage = _pageFactory.CreatePage("UMS (USB Mass Storage)", umsOptions);
        var systemPage = _pageFactory.CreatePage("System Options", systemOptions);
        
        Pages.Add(launchPage);
        Pages.Add(configPage);
        Pages.Add(umsPage);
        Pages.Add(systemPage);
    }
    
    private static void AssignCommands(List<RebootOption> options, ICommand command)
    {
        foreach (var option in options) {
            option.Command = command;
        }
    }

    private ICommand SelectLaunchOptionCommand { get; } = new RelayCommand<RebootOption>(option => 
        RebootHelper.ExecuteReboot("self", option!.Index.ToString(), "0"));

    private ICommand SelectConfigOptionCommand { get; } = new RelayCommand<RebootOption>(option => 
        RebootHelper.ExecuteReboot("self", option!.Index.ToString(), "1"));

    private ICommand SelectUmsOptionCommand { get; } = new RelayCommand<RebootOption>(option => 
        RebootHelper.ExecuteReboot("ums", option!.Index.ToString(), "0"));

    private ICommand RebootToBootloaderCommand { get; } = new RelayCommand(() => 
        RebootHelper.ExecuteReboot("bootloader", "0", "0"));

    private ICommand NormalRebootCommand { get; } = new RelayCommand(() => 
        RebootHelper.ExecuteReboot("normal", "0", "0"));

    private ICommand ShutdownCommand { get; } = new RelayCommand(RebootHelper.Shutdown);
}