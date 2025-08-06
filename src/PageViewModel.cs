using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using R2CSharp.Models;
using R2CSharp.Services;
using Avalonia;

namespace R2CSharp;

public partial class PageViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<SectionData> _sections = [];
    [ObservableProperty] private bool _isLoading = true;
    [ObservableProperty] private string _themeColor = "#00FF6E";
    [ObservableProperty] private Thickness _buttonsMargin = new(27, 4, 27, 4);

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
            ThemeColor = iconService.ThemeColor;
            ButtonsMargin = iconService.ButtonsMargin;
            
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
        Sections.Clear();

        var sectionConfigs = new[]
        {
            new
            {
                Title = "Launch",
                EmptyMessage = "No launch options found",
                Options = _rebootOptionsService!.LoadLaunchOptions(),
                Command = SelectLaunchOptionCommand
            },
            new
            {
                Title = "More Configurations",
                EmptyMessage = "No config options found",
                Options = _rebootOptionsService.LoadConfigOptions(),
                Command = SelectConfigOptionCommand
            },
            new
            {
                Title = "UMS (USB Mass Storage)",
                EmptyMessage = "",
                Options = RebootOptionsService.LoadUmsOptions(),
                Command = SelectUmsOptionCommand
            }
        };

        foreach (var config in sectionConfigs)
        {
            var section = new SectionData
            {
                Title = config.Title,
                EmptyMessage = config.EmptyMessage
            };

            foreach (var option in config.Options)
            {
                option.Command = config.Command;
                section.Items.Add(option);
            }

            Sections.Add(section);
        }
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