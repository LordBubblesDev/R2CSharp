namespace R2CSharp.Converters;

public static class ColorConverter
{
    /// <summary>
    /// Converts HSV color values to RGB hex string for Avalonia
    /// Based on the lv_color functions from Hekate's Nyx source code
    /// </summary>
    /// <param name="hue">Hue value [0-359]</param>
    /// <param name="saturation">Saturation value [0-100]</param>
    /// <param name="value">Value/Brightness [0-100]</param>
    /// <returns>Hex color string in format "#RRGGBB"</returns>
    public static string HsvToHex(int hue, int saturation, int value)
    {
        double h = hue;
        double s = saturation / 100.0;
        double v = value / 100.0;

        if (s == 0)
        {
            int gray = (int)(v * 255);
            return $"#{gray:X2}{gray:X2}{gray:X2}";
        }

        // Convert hue to 0-6 range
        h = h / 60.0;
        var i = (int)Math.Floor(h);
        var f = h - i;
        var p = v * (1 - s);
        var q = v * (1 - s * f);
        var t = v * (1 - s * (1 - f));

        double r, g, b;

        switch (i % 6) {
            case 0: 
                r = v; g = t; b = p;
                break;
            case 1:
                r = q; g = v; b = p;
                break;
            case 2:
                r = p; g = v; b = t;
                break;
            case 3:
                r = p; g = q; b = v;
                break;
            case 4:
                r = t; g = p; b = v;
                break;
            default:
                r = v; g = p; b = q;
                break;
        }

        // Convert to 0-255 range and create hex string
        var red = (int)Math.Round(r * 255);
        var green = (int)Math.Round(g * 255);
        var blue = (int)Math.Round(b * 255);

        return $"#{red:X2}{green:X2}{blue:X2}";
    }

    /// <summary>
    /// Converts theme color (Hue only) to hex color using default saturation and value
    /// </summary>
    /// <param name="hue">Hue value [0-359]</param>
    /// <returns>Hex color string in format "#RRGGBB"</returns>
    private static string ThemeColorToHex(int hue)
    {
        // Use default saturation and value
        return HsvToHex(hue, 100, 100);
    }

    /// <summary>
    /// Generates all hex colors from hue 0 to 359
    /// </summary>
    /// <returns>Array of hex color strings for all hue values</returns>
    public static string[] GetAllHueColors()
    {
        return Enumerable.Range(0, 360)
            .Select(ThemeColorToHex)
            .ToArray();
    }

    /// <summary>
    /// Logs all hue colors to console for debugging
    /// </summary>
    public static void LogAllColors()
    {
        var allColors = GetAllHueColors();
        Console.WriteLine("All HSV colors (0-359):");
        for (var i = 0; i < allColors.Length; i++)
        {
            Console.WriteLine($"Hue {i:D3}: {allColors[i]}");
        }
    }
}
