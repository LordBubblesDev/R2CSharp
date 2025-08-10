using System.Diagnostics;

namespace R2CSharp.Lib.Helpers;

public static class RebootHelper
{
    private static readonly bool IsTkmm = File.Exists("/usr/bin/tkmm-reboot");

    public static void ExecuteReboot(string action, string param1, string param2)
    {
        try {
            // Write to sysfs files like r2c does
            File.WriteAllText("/sys/devices/r2p/action", action);
            File.WriteAllText("/sys/devices/r2p/param1", param1);
            File.WriteAllText("/sys/devices/r2p/param2", param2);

            Console.WriteLine($"Reboot command: action={action}, param1={param1}, param2={param2}");

            ExecuteProcess(IsTkmm ? "/usr/bin/tkmm-reboot" : "sh", "-c \"reboot\"");
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to execute reboot command: {ex.Message}");
        }
    }

    public static void Shutdown()
    {
        try {
            ExecuteProcess(IsTkmm ? "/usr/bin/tkmm-shutdown" : "sh", "-c \"shutdown -P now\"");
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