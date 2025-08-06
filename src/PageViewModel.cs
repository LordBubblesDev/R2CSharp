using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using R2CSharp.Models;
using R2CSharp.Services;
using Avalonia;

namespace R2CSharp;

public partial class PageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<PageConfiguration> _pages = [];
    [ObservableProperty] private bool _isLoading = true;

    private readonly BootDiskService _bootDiskService;
    private RebootOptionsService? _rebootOptionsService;
    private PageFactoryService? _pageFactoryService;

    public PageViewModel()
    {
        _bootDiskService = new BootDiskService();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try {
            await _bootDiskService.InitializeBootDiskAsync();
            
            var iconService = new IconService(_bootDiskService.BootDiskPath);
            _rebootOptionsService = new RebootOptionsService(_bootDiskService.BootDiskPath, iconService);
            _pageFactoryService = new PageFactoryService(iconService);
            
            await LoadRebootOptionsAsync();
        }
        finally {
            IsLoading = false;
        }
    }

    private async Task LoadRebootOptionsAsync()
    {
        await Task.Run(LoadRebootOptions);
    }

    private void LoadRebootOptions()
    {
        Pages.Clear();

        // Create Launch page
        var launchOptions = _rebootOptionsService!.LoadLaunchOptions();
        foreach (var option in launchOptions)
        {
            option.Command = SelectLaunchOptionCommand;
        }
        var launchPage = _pageFactoryService!.CreateLaunchPage(launchOptions);
        Pages.Add(launchPage);

        // Create Config page
        var configOptions = _rebootOptionsService.LoadConfigOptions();
        foreach (var option in configOptions)
        {
            option.Command = SelectConfigOptionCommand;
        }
        var configPage = _pageFactoryService.CreateConfigPage(configOptions);
        Pages.Add(configPage);

        // Create UMS page
        var umsOptions = RebootOptionsService.LoadUmsOptions();
        foreach (var option in umsOptions)
        {
            option.Command = SelectUmsOptionCommand;
        }
        var umsPage = _pageFactoryService.CreateUmsPage(umsOptions);
        Pages.Add(umsPage);

        // Create System Options page
        var systemOptions = new List<RebootOption>
        {
            new() { Name = "Hekate Bootloader", Command = RebootToBootloaderCommand, FallbackIcon = "fa-solid fa-boot" },
            new() { Name = "Reboot", Command = NormalRebootCommand, FallbackIcon = "fa-solid fa-rotate" },
            new() { Name = "Shutdown", Command = ShutdownCommand, FallbackIcon = "fa-solid fa-power-off" }
        };
        var systemPage = _pageFactoryService.CreateSystemPage(systemOptions);
        Pages.Add(systemPage);
    }


    [RelayCommand]
    private void SelectLaunchOption(RebootOption option)
    {
        RebootService.ExecuteReboot("self", option.Index.ToString(), "0");
    }

    [RelayCommand]
    private void SelectConfigOption(RebootOption option)
    {
        RebootService.ExecuteReboot("self", option.Index.ToString(), "1");
    }

    [RelayCommand]
    private void SelectUmsOption(RebootOption option)
    {
        RebootService.ExecuteReboot("ums", option.Index.ToString(), "0");
    }

    [RelayCommand]
    private void RebootToBootloader()
    {
        RebootService.ExecuteReboot("bootloader", "0", "0");
    }

    [RelayCommand]
    private void NormalReboot()
    {
        RebootService.ExecuteReboot("normal", "0", "0");
    }

    [RelayCommand]
    private void Shutdown()
    {
        RebootService.Shutdown();
    }

    public void Cleanup()
    {
        _bootDiskService.Cleanup();
    }
}