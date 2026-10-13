using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using R2CSharp.Lib.Converters;
using R2CSharp.Lib.Readers;

namespace R2CSharp.Lib.Components;

public class NyxIcons
{
    private readonly string _bootDiskPath;
    public readonly string ThemeColor;
    public readonly bool UseFiveColumns;

    public NyxIcons(string bootDiskPath)
    {
        _bootDiskPath = bootDiskPath;
        var nyxIni = Path.Combine(bootDiskPath, "bootloader", "nyx.ini");
        var theme = IniConfigReader.GetConfigProperty(nyxIni, "themecolor");
        
        ThemeColor = theme != null && int.TryParse(theme, out var hue) && hue is >= 0 and <= 359
            ? ColorConverter.HsvToHex(hue, 100, 100)
            : ColorConverter.HsvToHex(167, 100, 100); // default nyx theme color

        // Allow displaying 5 button columns if this setting is set to 1
        UseFiveColumns = IniConfigReader.GetConfigProperty(nyxIni, "entries5col") == "1";
    }

    public Bitmap? FallbackIcon => ConvertBmpToBitmap("bootloader/res/icon_switch.bmp");

    public Bitmap? ConvertBmpToBitmap(string? bmpPath)
    {
        if (string.IsNullOrEmpty(bmpPath)) return null;
        var path = Path.Combine(_bootDiskPath, bmpPath);
        if (!File.Exists(path)) return null;

        try {
            var data = File.ReadAllBytes(path);
            var o = BitConverter.ToInt32(data, 10);
            var w = BitConverter.ToInt32(data, 18);
            var h = Math.Abs(BitConverter.ToInt32(data, 22));
            var stride = w * 4;
            var pixels = new byte[stride * h];

            var tint = ThemeColor.Length == 7 && (
                bmpPath.EndsWith("icon_switch.bmp", StringComparison.OrdinalIgnoreCase)
                || bmpPath.EndsWith("icon_payload.bmp", StringComparison.OrdinalIgnoreCase)
                || bmpPath.EndsWith("_hue_nobox.bmp", StringComparison.OrdinalIgnoreCase)
                || bmpPath.EndsWith("_hue.bmp", StringComparison.OrdinalIgnoreCase));
            
            byte tr = 0, tg = 0, tb = 0;
            if (tint) {
                tr = Convert.ToByte(ThemeColor[1..3], 16);
                tg = Convert.ToByte(ThemeColor[3..5], 16);
                tb = Convert.ToByte(ThemeColor[5..7], 16);
            }

            for (var y = 0; y < h; y++) {
                var src = o + (h - 1 - y) * stride;
                var dst = y * stride;

                if (!tint) {
                    Buffer.BlockCopy(data, src, pixels, dst, stride);
                    continue;
                }

                for (var x = 0; x < w; x++, src += 4, dst += 4) {
                    pixels[dst] = tb;
                    pixels[dst + 1] = tg;
                    pixels[dst + 2] = tr;
                    pixels[dst + 3] = data[src + 3];
                }
            }

            var handle = GCHandle.Alloc(pixels, GCHandleType.Pinned);

            try {
                return new WriteableBitmap(PixelFormat.Bgra8888, AlphaFormat.Unpremul,
                    handle.AddrOfPinnedObject(), new PixelSize(w, h), new Vector(96, 96), stride);
            }
            finally {
                handle.Free();
            }
        }
        catch (Exception ex) {
            Console.WriteLine($"Failed to convert BMP {bmpPath}: {ex.Message}");
            return null;
        }
    }
}
