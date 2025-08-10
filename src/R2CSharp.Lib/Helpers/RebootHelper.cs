using System.Diagnostics;

namespace R2CSharp.Lib.Helpers;

public static class RebootHelper
{
    private static readonly bool IsTkmm = File.Exists("/usr/bin/tkmm-reboot");
    private static readonly bool HasReboot = File.Exists("/sbin/reboot") || File.Exists("/usr/sbin/reboot");
    private static readonly bool HasShutdown = File.Exists("/sbin/shutdown") || File.Exists("/usr/sbin/shutdown");

    public static void ExecuteReboot(string action, string param1, string param2)
    {
        try {
            // Write to sysfs files like r2c does
            File.WriteAllText("/sys/devices/r2p/action", action);
            File.WriteAllText("/sys/devices/r2p/param1", param1);
            File.WriteAllText("/sys/devices/r2p/param2", param2);

            Console.WriteLine($"Reboot command: action={action}, param1={param1}, param2={param2}");

            if (IsTkmm) {
                ExecuteProcess("/usr/bin/tkmm-reboot", null);
            } else if (HasReboot) {
                ExecuteProcess("/sbin/reboot", null);
            } else {
                ExecuteProcess("sh", "-c \"reboot\"");
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to execute reboot command: {ex.Message}");
        }
    }

    public static void Shutdown()
    {
        try {
            if (IsTkmm) {
                ExecuteProcess("/usr/bin/tkmm-shutdown", null);
            } else if (HasShutdown) {
                ExecuteProcess("/sbin/shutdown", "-P now");
            } else {
                ExecuteProcess("sh", "-c \"shutdown -P now\"");
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to execute shutdown command: {ex.Message}");
        }
    }
    
    private static Process? ExecuteProcess(string fileName, string? arguments)
    {
        if (!File.Exists(fileName)) return null;
        
        var startInfo = new ProcessStartInfo {
            FileName = fileName,
            Arguments = arguments ?? string.Empty,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        
        return Process.Start(startInfo);
    }
}