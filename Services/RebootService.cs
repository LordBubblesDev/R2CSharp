using System.Diagnostics;

namespace R2CSharp.Services;

public class RebootService
{
    public void ExecuteReboot(string action, string param1, string param2)
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

    public void Shutdown()
    {
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "sh",
                Arguments = "-c \"shutdown -P now\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            // Don't wait for exit since we're shutting down
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to execute shutdown command: {ex.Message}");
        }
    }
} 