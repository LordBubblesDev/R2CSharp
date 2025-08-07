namespace R2CSharp.Readers;

public static class IniConfigReader
{
    /// <summary>
    /// Gets a property value from the config section of an INI file
    /// </summary>
    /// <param name="iniFilePath">Path to the INI file</param>
    /// <param name="propertyName">Name of the property to get</param>
    /// <returns>The property value or null if not found</returns>
    public static string? GetConfigProperty(string iniFilePath, string propertyName)
    {
        try {
            if (!File.Exists(iniFilePath)) {
                Console.WriteLine($"INI file not found at: {iniFilePath}");
                return null;
            }

            var lines = File.ReadAllLines(iniFilePath);
            string? currentSection = null;
            string? propertyValue = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']'))
                {
                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                }
                else if (currentSection == "config" && 
                         trimmedLine.StartsWith($"{propertyName}=", StringComparison.OrdinalIgnoreCase))
                {
                    propertyValue = trimmedLine[(propertyName.Length + 1)..].Trim();
                }
            }

            return propertyValue;
        }
        catch (Exception ex) {
            Console.WriteLine($"Error reading property {propertyName} from {iniFilePath}: {ex.Message}");
            return null;
        }
    }
    
    public static List<(string Name, string? IconPath)> ParseIniSections(string iniFilePath)
    {
        var sections = new List<(string Name, string? IconPath)>();

        try {
            var lines = File.ReadAllLines(iniFilePath);
            string? currentSection = null;
            string? currentIcon = null;

            foreach (var line in lines) {
                var trimmedLine = line.Trim();

                if (trimmedLine.StartsWith('[') && trimmedLine.EndsWith(']')) {
                    if (currentSection != null && currentSection != "config")
                        sections.Add((currentSection, currentIcon));

                    currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    currentIcon = null;
                }
                else if (currentSection != null && currentSection != "config" &&
                         trimmedLine.StartsWith("icon=", StringComparison.OrdinalIgnoreCase)) {
                    currentIcon = trimmedLine.Substring(5).Trim();
                }
            }

            if (currentSection != null && currentSection != "config") sections.Add((currentSection, currentIcon));
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to parse Hekate INI files: {ex.Message}");
        }

        return sections;
    }
}