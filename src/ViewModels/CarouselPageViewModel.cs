using System.Collections.ObjectModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using R2CSharp.Components;
using R2CSharp.Helpers;
using R2CSharp.Models;
using R2CSharp.Services;

namespace R2CSharp.ViewModels;

public partial class CarouselPageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<PageConfiguration> _pages = [];
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _themeColor = "#00FFC8"; // hekate's default theme color
    [ObservableProperty] private bool _canGoPrevious;
    [ObservableProperty] private bool _canGoNext = true;

    private readonly BootDiskService _bootDiskService;
    private RebootOptionsProvider? _rebootOptionsService;
    private PageFactory? _pageFactoryService;
    private NyxIcons? _iconService;

    public CarouselPageViewModel()
    {
        _bootDiskService = new BootDiskService();
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try {
            await _bootDiskService.InitializeBootDiskAsync();
            _iconService = await Task.Run(() => new NyxIcons(_bootDiskService.BootDiskPath));
            await Dispatcher.UIThread.InvokeAsync(() => { ThemeColor = _iconService.ThemeColor; });
            
            _rebootOptionsService = new RebootOptionsProvider(_bootDiskService.BootDiskPath, _iconService);
            _pageFactoryService = new PageFactory(_iconService);
            
            await LoadRebootOptionsAsync();
            
            await Dispatcher.UIThread.InvokeAsync(() => { OnPropertyChanged(nameof(Pages)); });
        }
        catch (Exception ex)
        {
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

        // Create Launch page
        var launchOptions = _rebootOptionsService!.LoadLaunchOptions();
        
        foreach (var option in launchOptions) {
            option.Command = SelectLaunchOptionCommand;
        }
        
        var launchPage = _pageFactoryService!.CreateLaunchPage(launchOptions);
        Pages.Add(launchPage);

        // Create Config page
        var configOptions = _rebootOptionsService.LoadConfigOptions();
        
        foreach (var option in configOptions) {
            option.Command = SelectConfigOptionCommand;
        }
        
        var configPage = _pageFactoryService.CreateConfigPage(configOptions);
        Pages.Add(configPage);

        // Create UMS page
        var umsOptions = RebootOptionsProvider.LoadUmsOptions();
        
        foreach (var option in umsOptions) {
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
        RebootHelper.ExecuteReboot("self", option.Index.ToString(), "0");
    }

    [RelayCommand]
    private void SelectConfigOption(RebootOption option)
    {
        RebootHelper.ExecuteReboot("self", option.Index.ToString(), "1");
    }

    [RelayCommand]
    private void SelectUmsOption(RebootOption option)
    {
        RebootHelper.ExecuteReboot("ums", option.Index.ToString(), "0");
    }

    [RelayCommand]
    private void RebootToBootloader()
    {
        RebootHelper.ExecuteReboot("bootloader", "0", "0");
    }

    [RelayCommand]
    private void NormalReboot()
    {
        RebootHelper.ExecuteReboot("normal", "0", "0");
    }

    [RelayCommand]
    private void Shutdown()
    {
        RebootHelper.Shutdown();
    }

    public void Cleanup()
    {
        _bootDiskService.Cleanup();
    }
}