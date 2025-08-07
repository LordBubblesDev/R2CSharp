using System.Diagnostics;

namespace R2CSharp.Helpers;

public static class RebootHelper
{
    public static void ExecuteReboot(string action, string param1, string param2)
    {
        try {
            // Write to sysfs files like r2c does
            File.WriteAllText("/sys/devices/r2p/action", action);
            File.WriteAllText("/sys/devices/r2p/param1", param1);
            File.WriteAllText("/sys/devices/r2p/param2", param2);

            Console.WriteLine($"Reboot command: action={action}, param1={param1}, param2={param2}");

            var startInfo = new ProcessStartInfo {
                FileName = "sh",
                Arguments = "-c \"reboot\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to execute reboot command: {ex.Message}");
        }
    }

    public static void Shutdown()
    {
        try {
            var startInfo = new ProcessStartInfo {
                FileName = "sh",
                Arguments = "-c \"shutdown -P now\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to execute shutdown command: {ex.Message}");
        }
    }
}