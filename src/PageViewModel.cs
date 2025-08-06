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
        Console.WriteLine("[PageViewModel] InitializeAsync started");
        try {
            Console.WriteLine("[PageViewModel] Initializing boot disk...");
            await _bootDiskService.InitializeBootDiskAsync();
            Console.WriteLine($"[PageViewModel] Boot disk initialized: {_bootDiskService.BootDiskPath}");
            
            Console.WriteLine("[PageViewModel] Creating services...");
            var iconService = new IconService(_bootDiskService.BootDiskPath);
            _rebootOptionsService = new RebootOptionsService(_bootDiskService.BootDiskPath, iconService);
            _pageFactoryService = new PageFactoryService(iconService);
            Console.WriteLine("[PageViewModel] Services created");
            
                    Console.WriteLine("[PageViewModel] Loading reboot options...");
        await LoadRebootOptionsAsync();
        Console.WriteLine($"[PageViewModel] Reboot options loaded, pages count: {Pages.Count}");
        
        // Force property change notification for Pages
        OnPropertyChanged(nameof(Pages));
        Console.WriteLine("[PageViewModel] Pages property change notification sent");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PageViewModel] Error during initialization: {ex.Message}");
        }
        finally {
            IsLoading = false;
            Console.WriteLine("[PageViewModel] InitializeAsync completed, IsLoading = false");
        }
    }

    private async Task LoadRebootOptionsAsync()
    {
        await Task.Run(LoadRebootOptions);
    }

    private void LoadRebootOptions()
    {
        Console.WriteLine("[PageViewModel] LoadRebootOptions started");
        Pages.Clear();

        // Create Launch page
        var launchOptions = _rebootOptionsService!.LoadLaunchOptions();
        Console.WriteLine($"[PageViewModel] Launch options loaded: {launchOptions.Count}");
        foreach (var option in launchOptions)
        {
            option.Command = SelectLaunchOptionCommand;
        }
        var launchPage = _pageFactoryService!.CreateLaunchPage(launchOptions);
        Pages.Add(launchPage);
        Console.WriteLine($"[PageViewModel] Launch page added, total pages: {Pages.Count}");

        // Create Config page
        var configOptions = _rebootOptionsService.LoadConfigOptions();
        Console.WriteLine($"[PageViewModel] Config options loaded: {configOptions.Count}");
        foreach (var option in configOptions)
        {
            option.Command = SelectConfigOptionCommand;
        }
        var configPage = _pageFactoryService.CreateConfigPage(configOptions);
        Pages.Add(configPage);
        Console.WriteLine($"[PageViewModel] Config page added, total pages: {Pages.Count}");

        // Create UMS page
        var umsOptions = RebootOptionsService.LoadUmsOptions();
        Console.WriteLine($"[PageViewModel] UMS options loaded: {umsOptions.Count}");
        foreach (var option in umsOptions)
        {
            option.Command = SelectUmsOptionCommand;
        }
        var umsPage = _pageFactoryService.CreateUmsPage(umsOptions);
        Pages.Add(umsPage);
        Console.WriteLine($"[PageViewModel] UMS page added, total pages: {Pages.Count}");

        // Create System Options page
        var systemOptions = new List<RebootOption>
        {
            new() { Name = "Hekate Bootloader", Command = RebootToBootloaderCommand, FallbackIcon = "fa-solid fa-boot" },
            new() { Name = "Reboot", Command = NormalRebootCommand, FallbackIcon = "fa-solid fa-rotate" },
            new() { Name = "Shutdown", Command = ShutdownCommand, FallbackIcon = "fa-solid fa-power-off" }
        };
        var systemPage = _pageFactoryService.CreateSystemPage(systemOptions);
        Pages.Add(systemPage);
        Console.WriteLine($"[PageViewModel] System page added, total pages: {Pages.Count}");
        Console.WriteLine($"[PageViewModel] LoadRebootOptions completed");
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