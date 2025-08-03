using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace R2CSharp;

public partial class PageViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<RebootOption> _launchOptions = new();

    [ObservableProperty]
    private ObservableCollection<RebootOption> _configOptions = new();

    [ObservableProperty]
    private ObservableCollection<RebootOption> _umsOptions = new();

    private string _bootDiskPath = string.Empty;
    private bool _isDiskMounted = false;
    
    private Bitmap? _fallbackIcon;

    public PageViewModel()
    {
        InitializeBootDisk();
        _fallbackIcon = ConvertBmpToBitmap("bootloader/res/icon_switch.bmp");
        LoadRebootOptions();
    }

    private void InitializeBootDisk()
    {
#if DEBUG
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("Debug mode on Windows - using test path");
            _bootDiskPath = "V:\\";
            _isDiskMounted = false;
            return;
        }
#endif

        // Check if /flash already exists
        if (Directory.Exists("/flash/bootloader"))
        {
            Console.WriteLine("Bootloader files already accessible at /flash/bootloader");
            _bootDiskPath = "/flash";
            _isDiskMounted = false;
            return;
        }

        Console.WriteLine("Mounting boot disk...");
        if (MountBootDisk())
        {
            // _bootDiskPath is already set in MountBootDisk() method
            _isDiskMounted = true;
        }
        else
        {
            Console.WriteLine("Failed to mount boot disk, using fallback path");
            _bootDiskPath = "/flash";
            _isDiskMounted = false;
        }
    }

    private bool MountBootDisk()
    {
        try
        {
            var cmdline = File.ReadAllText("/proc/cmdline");
            var swrDir = ParseCmdlineParameter(cmdline, "swr_dir") ?? "switchroot/ubuntu";
            var mmcBlk = ParseCmdlineParameter(cmdline, "boot_m") ?? "0";
            var mmcPart = ParseCmdlineParameter(cmdline, "boot_p") ?? "1";

            var devMmc = "mmcblk0";
            if (File.Exists("/dev/mmcblk1") && mmcBlk == "1")
            {
                devMmc = "mmcblk1";
            }

            var devicePath = $"/dev/{devMmc}p{mmcPart}";
            Console.WriteLine($"Mounting boot disk {devicePath}");

            if (!File.Exists(devicePath))
            {
                Console.WriteLine($"Device {devicePath} not found");
                return false;
            }

            // Check if the device is already mounted
            var mountInfo = ExecuteCommandWithOutput("mount");
            if (mountInfo.Contains(devicePath))
            {
                // Device is already mounted, find the mount point
                var lines = mountInfo.Split('\n');
                foreach (var line in lines)
                {
                    if (line.Contains(devicePath))
                    {
                        var parts = line.Split(new[] { " on ", " type " }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            var existingMountPoint = parts[1].Trim();
                            Console.WriteLine($"Boot disk already mounted at {existingMountPoint}");
                            _bootDiskPath = existingMountPoint;
                            return true;
                        }
                    }
                }
            }

            // Create mount point
            var mountPoint = "/opt/switchroot/boot_disk";
            Directory.CreateDirectory(mountPoint);

            var result = ExecuteCommand("mount", $"{devicePath} {mountPoint}");
            if (result == 0)
            {
                Console.WriteLine("Boot disk mounted successfully");
                _bootDiskPath = mountPoint;
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to mount boot disk, exit code: {result}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error mounting boot disk: {ex.Message}");
            return false;
        }
    }

    private string? ParseCmdlineParameter(string cmdline, string parameter)
    {
        var pattern = $"{parameter}=([^\\s]+)";
        var match = System.Text.RegularExpressions.Regex.Match(cmdline, pattern);
        return match.Success ? match.Groups[1].Value : null;
    }

    private int ExecuteCommand(string command, string arguments)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "sh",
                Arguments = $"-c \"{command} {arguments}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = Process.Start(startInfo);
            if (process != null)
            {
                process.WaitForExit();
                return process.ExitCode;
            }
            return -1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing command {command}: {ex.Message}");
            return -1;
        }
    }

    private string ExecuteCommandWithOutput(string command, string? arguments = null)
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "sh",
                Arguments = arguments != null ? $"-c \"{command} {arguments}\"" : $"-c \"{command}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using var process = Process.Start(startInfo);
            if (process != null)
            {
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
            return string.Empty;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error executing command {command}: {ex.Message}");
            return string.Empty;
        }
    }

    private void LoadRebootOptions()
    {
        LoadLaunchOptions();
        LoadConfigOptions();
        LoadUmsOptions();
    }

    private void LoadLaunchOptions()
    {
        LaunchOptions.Clear();
        var hekateIplPath = Path.Combine(_bootDiskPath, "bootloader", "hekate_ipl.ini");
        
        if (File.Exists(hekateIplPath)) {
            var sections = ParseIniSections(hekateIplPath);
            int index = 1;
            foreach (var section in sections) {
                var icon = ConvertBmpToBitmap(section.IconPath) ?? _fallbackIcon;
                LaunchOptions.Add(new RebootOption {
                    Name = section.Name,
                    Type = RebootType.Launch,
                    Index = index,
                    Icon = icon
                });
                index++;
            }
        }
    }

    private void LoadConfigOptions()
    {
        ConfigOptions.Clear();
        var iniDirectory = Path.Combine(_bootDiskPath, "bootloader", "ini");
        
        if (Directory.Exists(iniDirectory)) {
            var iniFiles = Directory.GetFiles(iniDirectory, "*.ini");
            int globalIndex = 1;
            
            foreach (var iniFile in iniFiles) {
                var sections = ParseIniSections(iniFile);
                foreach (var section in sections) {
                    var icon = ConvertBmpToBitmap(section.IconPath) ?? _fallbackIcon;
                    ConfigOptions.Add(new RebootOption {
                        Name = section.Name,
                        Type = RebootType.Config,
                        Index = globalIndex,
                        Icon = icon
                    });
                    globalIndex++;
                }
            }
        }
    }

    private void LoadUmsOptions()
    {
        UmsOptions.Clear();
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
            UmsOptions.Add(new RebootOption {
                Name = umsOptions[i],
                Type = RebootType.Ums,
                Index = i
            });
        }
    }

    private List<(string Name, string? IconPath)> ParseIniSections(string iniFilePath)
    {
        var sections = new List<(string Name, string? IconPath)>();
        
        try {
            var lines = File.ReadAllLines(iniFilePath);
            string? currentSection = null;
            string? currentIcon = null;
            
            foreach (var line in lines) {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']')) {
                    if (currentSection != null && currentSection != "config") {
                        sections.Add((currentSection, currentIcon));
                    }
                    
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    currentIcon = null;
                }
                else if (currentSection != null && trimmedLine.StartsWith("icon=", StringComparison.OrdinalIgnoreCase)) {
                    currentIcon = trimmedLine.Substring(5).Trim();
                }
            }
            
            if (currentSection != null && currentSection != "config") {
                sections.Add((currentSection, currentIcon));
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to parse Hekate INI files: {ex.Message}");
        }
        
        return sections;
    }

    private Bitmap? ConvertBmpToBitmap(string? bmpPath)
    {
        if (string.IsNullOrEmpty(bmpPath)) {
            return null;
        }
        
        var fullBmpPath = Path.Combine(_bootDiskPath, bmpPath);
        
        if (!File.Exists(fullBmpPath)) {
            return null;
        }

        try {
            using var image = Image.Load(fullBmpPath);
            using var stream = new MemoryStream();
            image.Save(stream, new PngEncoder());
            stream.Seek(0, SeekOrigin.Begin);
            return new Bitmap(stream);
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to convert BMP {bmpPath}: {ex.Message}");
            return null;
        }
    }
    
    [RelayCommand]
    private void SelectLaunchOption(RebootOption option)
    {
        ExecuteReboot("self", option.Index.ToString(), "0");
    }

    [RelayCommand]
    private void SelectConfigOption(RebootOption option)
    {
        ExecuteReboot("self", option.Index.ToString(), "1");
    }

    [RelayCommand]
    private void SelectUmsOption(RebootOption option)
    {
        ExecuteReboot("ums", option.Index.ToString(), "0");
    }

    [RelayCommand]
    private void RebootToBootloader()
    {
        ExecuteReboot("bootloader", "0", "0");
    }

    [RelayCommand]
    private void NormalReboot()
    {
        ExecuteReboot("normal", "0", "0");
    }

    [RelayCommand]
    private void Shutdown()
    {
        // In a real implementation, you would call the appropriate shutdown command
        Console.WriteLine("Shutdown command executed");
    }

    private void ExecuteReboot(string action, string param1, string param2)
    {
        try
        {
            // Write to sysfs files like r2c does
            File.WriteAllText("/sys/devices/r2p/action", action);
            File.WriteAllText("/sys/devices/r2p/param1", param1);
            File.WriteAllText("/sys/devices/r2p/param2", param2);
            
            Console.WriteLine($"Reboot command: action={action}, param1={param1}, param2={param2}");
            
            // Execute reboot command
            var startInfo = new ProcessStartInfo
            {
                FileName = "reboot",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            // Don't wait for exit since we're rebooting
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to execute reboot command: {ex.Message}");
        }
    }

    public void Cleanup()
    {
        if (_isDiskMounted)
        {
            try
            {
                Console.WriteLine("Cleaning up mounted disk...");
                var result = ExecuteCommand("umount", "/opt/switchroot/boot_disk");
                if (result == 0)
                {
                    Console.WriteLine("Boot disk unmounted successfully");
                }
                else
                {
                    Console.WriteLine($"Failed to unmount boot disk, exit code: {result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during cleanup: {ex.Message}");
            }
        }
    }
}

public class RebootOption
{
    public string Name { get; set; } = string.Empty;
    public RebootType Type { get; set; }
    public int Index { get; set; }
    public Bitmap? Icon { get; set; }
}

public enum RebootType
{
    Launch,
    Config,
    Ums
} 