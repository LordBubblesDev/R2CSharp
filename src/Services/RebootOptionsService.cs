using R2CSharp.Models;

namespace R2CSharp.Services;

public class RebootOptionsService(string bootDiskPath, IniParserService iniParser, IconService iconService)
{
    public List<RebootOption> LoadLaunchOptions()
    {
        var options = new List<RebootOption>();
        var hekateIplPath = Path.Combine(bootDiskPath, "bootloader", "hekate_ipl.ini");

        if (File.Exists(hekateIplPath)) {
            var sections = IniParserService.ParseIniSections(hekateIplPath);
            var index = 1;
            foreach (var section in sections) {
                var icon = iconService.ConvertBmpToBitmap(section.IconPath) ?? iconService.FallbackIcon;
                options.Add(new RebootOption {
                    Name = section.Name,
                    Type = RebootType.Launch,
                    Index = index,
                    Icon = icon
                });
                index++;
            }
        }

        return options;
    }

    public List<RebootOption> LoadConfigOptions()
    {
        var options = new List<RebootOption>();
        var iniDirectory = Path.Combine(bootDiskPath, "bootloader", "ini");

        if (!Directory.Exists(iniDirectory)) return options;
        var iniFiles = Directory.GetFiles(iniDirectory, "*.ini")
            .OrderBy(Path.GetFileName, StringComparer.Ordinal)
            .ToArray();

        var allSections = (
            from iniFile in iniFiles let sections = 
                IniParserService.ParseIniSections(iniFile) from section in sections select (
                    section.Name,
                    section.IconPath,
                    Path.GetFileName(iniFile))).ToList();

        var globalIndex = 1;
        foreach (var section in allSections) {
            var icon = iconService.ConvertBmpToBitmap(section.IconPath) ?? iconService.FallbackIcon;
            options.Add(new RebootOption {
                Name = section.Name,
                Type = RebootType.Config,
                Index = globalIndex,
                Icon = icon
            });
            globalIndex++;
        }

        return options;
    }

    public static List<RebootOption> LoadUmsOptions()
    {
        var umsOptions = new[] {
            "SD Card",
            "eMMC BOOT0",
            "eMMC BOOT1",
            "eMMC GPP",
            "emuMMC BOOT0",
            "emuMMC BOOT1",
            "emuMMC GPP"
        };

        return umsOptions.Select((t, i) => new RebootOption {
            Name = t,
            Type = RebootType.Ums,
            Index = i
        }).ToList();
    }
}