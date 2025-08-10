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
        foreach (var iniFile in iniFiles) {
            var sections = IniConfigReader.ParseIniSections(iniFile);
            foreach (var section in sections) {
                var icon = nyxIcons.ConvertBmpToBitmap(section.IconPath) ?? nyxIcons.FallbackIcon;
                options.Add(new RebootOption {
                    Name = section.Name,
                    Index = globalIndex++,
                    Icon = icon,
                    FallbackIcon = "fa-solid fa-cog"
                });
            }
        }

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
            FallbackIcon = "fa-solid fa-hdd"
        }).ToList();
    }
    
    private List<RebootOption> LoadOptionsFromIni(string iniPath, string fallbackIcon)
    {
        var options = new List<RebootOption>();
        
        if (!File.Exists(iniPath)) return options;
        
        var sections = IniConfigReader.ParseIniSections(iniPath);
        var index = 1;
        
        foreach (var section in sections) {
            var icon = nyxIcons.ConvertBmpToBitmap(section.IconPath) ?? nyxIcons.FallbackIcon;
            options.Add(new RebootOption {
                Name = section.Name,
                Index = index++,
                Icon = icon,
                FallbackIcon = fallbackIcon
            });
        }

        return options;
    }
}