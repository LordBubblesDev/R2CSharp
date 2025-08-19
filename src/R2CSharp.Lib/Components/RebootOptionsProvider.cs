using R2CSharp.Lib.Models;
using R2CSharp.Lib.Readers;

namespace R2CSharp.Lib.Components;

public class RebootOptionsProvider(string bootDiskPath, NyxIcons nyxIcons)
{
    public List<RebootOption> LoadLaunchOptions() => LoadOptionsFromIni(
        Path.Combine(bootDiskPath, "bootloader", "hekate_ipl.ini"), 
        "fa-solid fa-rocket");

    public List<RebootOption> LoadConfigOptions()
    {
        var options = new List<RebootOption>();
        var iniDirectory = Path.Combine(bootDiskPath, "bootloader", "ini");

        if (!Directory.Exists(iniDirectory)) return options;
        
        var iniFiles = Directory.GetFiles(iniDirectory, "*.ini")
            .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase);

        var globalIndex = 1;
        options.AddRange(iniFiles.SelectMany(IniConfigReader.ParseIniSections,
            (_, section) => CreateRebootOption(section, globalIndex++, "fa-solid fa-gear")));

        return options;
    }

    public static List<RebootOption> LoadUmsOptions()
    {
        var umsOptions = new[] {
            "SD Card", "eMMC BOOT0", "eMMC BOOT1", "eMMC GPP",
            "emuMMC BOOT0", "emuMMC BOOT1", "emuMMC GPP"
        };

        return umsOptions.Select((name, i) => new RebootOption {
            Name = name,
            Index = i,
            FallbackIcon = GetIconForUmsOption(name)
        }).ToList();
    }
    
    private static string GetIconForUmsOption(string name) => name switch {
        "SD Card" => "fa-solid fa-sd-card",
        _ when name.StartsWith("eMMC") => "fa-solid fa-microchip",
        _  => "fa-solid fa-hard-drive"
    };
    
    private List<RebootOption> LoadOptionsFromIni(string iniPath, string fallbackIcon)
    {
        var options = new List<RebootOption>();
        
        if (!File.Exists(iniPath)) return options;
        
        var sections = IniConfigReader.ParseIniSections(iniPath);
        var index = 1;

        options.AddRange(sections.Select(section => CreateRebootOption(section, index++, fallbackIcon)));

        return options;
    }
    
    private RebootOption CreateRebootOption((string Name, string? IconPath) section, int index, string fallbackIcon)
    {
        var icon = nyxIcons.ConvertBmpToBitmap(section.IconPath) ?? nyxIcons.FallbackIcon;
        return new RebootOption {
            Name = section.Name,
            Index = index,
            Icon = icon,
            FallbackIcon = fallbackIcon
        };
    }
}