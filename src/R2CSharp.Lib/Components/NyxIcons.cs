using Avalonia.Media.Imaging;
using R2CSharp.Lib.Converters;
using R2CSharp.Lib.Readers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Point = SixLabors.ImageSharp.Point;

namespace R2CSharp.Lib.Components;

public class NyxIcons
{
    private readonly string _bootDiskPath;
    public readonly string ThemeColor;
    public readonly bool UseFiveColumns;

    public NyxIcons(string bootDiskPath)
    {
        _bootDiskPath = bootDiskPath;
        
        var nyxIniPath = Path.Combine(_bootDiskPath, "bootloader", "nyx.ini");
        var themeColorValue = IniConfigReader.GetConfigProperty(nyxIniPath, "themecolor");

        ThemeColor = themeColorValue != null && int.TryParse(themeColorValue, out var hue) && hue is >= 0 and <= 359
            ? ColorConverter.HsvToHex(hue, 100, 100)
            : ColorConverter.HsvToHex(167, 100, 100); // default nyx theme color

        // Allow displaying 5 button columns if this setting is set to 1
        var entries5Col = IniConfigReader.GetConfigProperty(nyxIniPath, "entries5col") == "1";
        UseFiveColumns = entries5Col;
    }

    public Bitmap? FallbackIcon => ConvertBmpToBitmap("bootloader/res/icon_switch.bmp");

    private static bool ShouldApplyColor(string bmpPath) =>
        bmpPath.EndsWith("icon_switch.bmp", StringComparison.OrdinalIgnoreCase)
        || bmpPath.EndsWith("icon_payload.bmp", StringComparison.OrdinalIgnoreCase)
        || bmpPath.EndsWith("_hue_nobox.bmp", StringComparison.OrdinalIgnoreCase)
        || bmpPath.EndsWith("_hue.bmp", StringComparison.OrdinalIgnoreCase);

    public Bitmap? ConvertBmpToBitmap(string? bmpPath)
    {
        if (string.IsNullOrEmpty(bmpPath)) return null;

        var fullBmpPath = Path.Combine(_bootDiskPath, bmpPath);
        if (!File.Exists(fullBmpPath)) return null;

        try {
            using var image = Image.Load(fullBmpPath);
            
            if (ShouldApplyColor(bmpPath) && ThemeColor.StartsWith("#") && ThemeColor.Length == 7) {
                var (r, g, b) = ParseHexColor(ThemeColor);
                
                using var coloredImage = new Image<Rgba32>(image.Width, image.Height);
                coloredImage.Mutate(x => x.Fill(new Color(new Rgba32(r, g, b))));
                
                coloredImage.Mutate(x => x.DrawImage(image, new Point(0, 0), new GraphicsOptions
                {
                    ColorBlendingMode = PixelColorBlendingMode.Normal,
                    AlphaCompositionMode = PixelAlphaCompositionMode.DestIn
                }));
                
                image.Mutate(x => x
                    .Fill(Color.Transparent)
                    .DrawImage(coloredImage, new Point(0, 0), new GraphicsOptions
                    {
                        ColorBlendingMode = PixelColorBlendingMode.Normal,
                        AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver
                    })
                );
            }
            
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
    
    private static (byte r, byte g, byte b) ParseHexColor(string hexColor)
    {
        var r = Convert.ToByte(hexColor.Substring(1, 2), 16);
        var g = Convert.ToByte(hexColor.Substring(3, 2), 16);
        var b = Convert.ToByte(hexColor.Substring(5, 2), 16);
        return (r, g, b);
    }
}