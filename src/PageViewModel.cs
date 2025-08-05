using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using R2CSharp.Models;
using R2CSharp.Services;

namespace R2CSharp;

public partial class PageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<RebootOption> _launchOptions = new();

    [ObservableProperty] private ObservableCollection<RebootOption> _configOptions = new();

    [ObservableProperty] private ObservableCollection<RebootOption> _umsOptions = new();

    private readonly BootDiskService _bootDiskService;
    private readonly RebootOptionsService _rebootOptionsService;
    private readonly RebootService _rebootService;

    public PageViewModel()
    {
        _bootDiskService = new BootDiskService();
        _bootDiskService.InitializeBootDisk();

        var iniParser = new IniParserService();
        var iconService = new IconService(_bootDiskService.BootDiskPath);
        _rebootOptionsService = new RebootOptionsService(_bootDiskService.BootDiskPath, iniParser, iconService);
        _rebootService = new RebootService();

        LoadRebootOptions();
    }


    private void LoadRebootOptions()
    {
        LaunchOptions.Clear();
        ConfigOptions.Clear();
        UmsOptions.Clear();

        var launchOptions = _rebootOptionsService.LoadLaunchOptions();
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