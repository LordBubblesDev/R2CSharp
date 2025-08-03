using System.Diagnostics;
using System.Runtime.InteropServices;

namespace R2CSharp.Services;

public class BootDiskService
{
    private string _bootDiskPath = string.Empty;
    private bool _isDiskMounted = false;

    public string BootDiskPath => _bootDiskPath;
    public bool IsDiskMounted => _isDiskMounted;

    public void InitializeBootDisk()
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