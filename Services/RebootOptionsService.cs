using R2CSharp.Models;

namespace R2CSharp.Services;

public class RebootOptionsService
{
    private readonly string _bootDiskPath;
    private readonly IniParserService _iniParser;
    private readonly IconService _iconService;

    public RebootOptionsService(string bootDiskPath, IniParserService iniParser, IconService iconService)
    {
        _bootDiskPath = bootDiskPath;
        _iniParser = iniParser;
        _iconService = iconService;
    }

    public List<RebootOption> LoadLaunchOptions()
    {
        var options = new List<RebootOption>();
        var hekateIplPath = Path.Combine(_bootDiskPath, "bootloader", "hekate_ipl.ini");
        
        if (File.Exists(hekateIplPath)) {
            var sections = _iniParser.ParseIniSections(hekateIplPath);
            int index = 1;
            foreach (var section in sections) {
                var icon = _iconService.ConvertBmpToBitmap(section.IconPath) ?? _iconService.FallbackIcon;
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
        var iniDirectory = Path.Combine(_bootDiskPath, "bootloader", "ini");
        
        if (Directory.Exists(iniDirectory)) {
            var iniFiles = Directory.GetFiles(iniDirectory, "*.ini");
            int globalIndex = 1;
            
            foreach (var iniFile in iniFiles) {
                var sections = _iniParser.ParseIniSections(iniFile);
                foreach (var section in sections) {
                    var icon = _iconService.ConvertBmpToBitmap(section.IconPath) ?? _iconService.FallbackIcon;
                    options.Add(new RebootOption {
                        Name = section.Name,
                        Type = RebootType.Config,
                        Index = globalIndex,
                        Icon = icon
                    });
                    globalIndex++;
                }
            }
        }
        
        return options;
    }

    public List<RebootOption> LoadUmsOptions()
    {
        var options = new List<RebootOption>();
        var umsOptions = new[] {
            "SD Card",
            "eMMC BOOT0",
            "eMMC BOOT1", 
            "eMMC GPP",
            "emuMMC BOOT0",
            "emuMMC BOOT1",
            "emuMMC GPP"
        };

        for (int i = 0; i < umsOptions.Length; i++) {
            options.Add(new RebootOption {
                Name = umsOptions[i],
                Type = RebootType.Ums,
                Index = i
            });
        }
        
        return options;
    }
} 