using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using R2CSharp.Models;
using R2CSharp.Services;

namespace R2CSharp;

public partial class PageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<RebootOption> _launchOptions = [];

    [ObservableProperty] private ObservableCollection<RebootOption> _configOptions = [];

    [ObservableProperty] private ObservableCollection<RebootOption> _umsOptions = [];
    [ObservableProperty] private bool _isLoading = true;

    private readonly BootDiskService _bootDiskService;
    private RebootOptionsService? _rebootOptionsService;

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
        LaunchOptions.Clear();
        ConfigOptions.Clear();
        UmsOptions.Clear();

        var launchOptions = _rebootOptionsService!.LoadLaunchOptions();
        var configOptions = _rebootOptionsService.LoadConfigOptions();
        var umsOptions = RebootOptionsService.LoadUmsOptions();

        foreach (var option in launchOptions)
            LaunchOptions.Add(option);

        foreach (var option in configOptions)
            ConfigOptions.Add(option);

        foreach (var option in umsOptions)
            UmsOptions.Add(option);
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