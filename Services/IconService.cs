using Avalonia.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace R2CSharp.Services;

public class IconService
{
    private readonly string _bootDiskPath;
    private Bitmap? _fallbackIcon;

    public IconService(string bootDiskPath)
    {
        _bootDiskPath = bootDiskPath;
        _fallbackIcon = ConvertBmpToBitmap("bootloader/res/icon_switch.bmp");
    }

    public Bitmap? FallbackIcon => _fallbackIcon;

    public Bitmap? ConvertBmpToBitmap(string? bmpPath)
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
} 