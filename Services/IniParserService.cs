namespace R2CSharp.Services;

public class IniParserService
{
    public List<(string Name, string? IconPath)> ParseIniSections(string iniFilePath)
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
                else if (currentSection != null && currentSection != "config" && trimmedLine.StartsWith("icon=", StringComparison.OrdinalIgnoreCase)) {
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
} 