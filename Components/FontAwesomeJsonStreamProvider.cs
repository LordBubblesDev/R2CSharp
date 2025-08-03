using Avalonia.Platform;
using Projektanker.Icons.Avalonia.FontAwesome;

namespace R2CSharp.Components;

public sealed class FontAwesomeJsonStreamProvider : IFontAwesomeUtf8JsonStreamProvider
{
    public static readonly FontAwesomeJsonStreamProvider Instance = new();

    public Stream GetUtf8JsonStream()
    {
        const string iconsResourcePath = "avares://R2CSharp/Assets/fa-icons.json";
        Uri iconsResourceUri = new(iconsResourcePath);
        return AssetLoader.Open(iconsResourceUri);
    }
} 