# R2CSharp

A reimplementation of the R2C GUI script (Reboot 2 Config) for Switchroot Linux distros using C# and Avalonia UI, made to mimic and resemble [Hekate](https://github.com/CTCaer/hekate).

<p align="center">
  <video src="img/uidemo.mkv" controls width="650">
    Your browser does not support the video tag.
  </video>
</p>


## Overview

R2CSharp is a modern, responsive and touch-friendly interface that provides an intuitive way to select reboot options on Switchroot Linux distros, built with C# and Avalonia UI.

### Requirements

None, all required runtimes and libraries are embedded inside the binary.

### Features

- **Touch & Keyboard Support**: Touch gesture and keyboard navigation (set the joy-cons to mouse mode to navigate)
- **Theme Support**: Customizable theming (uses color from Nyx settings in Hekate)
- **Adaptive Scaling**: Adapts to different resolutions (only 16:9 since that's the aspect ratio of the Switch)

### Building

```bash
git clone https://github.com/LordBubblesDev/r2csharp
cd R2CSharp
dotnet build
```

### Running

```bash
dotnet run --project src/R2CSharp.App
```

## Configuration

The application reads configuration from:

- `bootloader/hekate_ipl.ini` - Launch options
- `bootloader/ini/*.ini` - Additional configuration files
- `bootloader/nyx.ini` - Theme color and column settings

## Navigation

### Keyboard/Mouse Controls

- **Scroll Wheel**: Navigate between pages
- **Click**: Select option
- **Page Up/Down Keys**: Navigate between pages
- **Arrow Keys**: Navigate between options
- **Enter Key**: Select option

### Touch Gestures

- **Swipe Up/Down**: Navigate between pages
- **Tap**: Select option

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- Help and encouragements from [CTCaer](https://github.com/CTCaer)
- Original R2C script from [theofficialgman](https://github.com/theofficialgman)
- Built with [Avalonia UI](https://avaloniaui.net/)
- Image processing with [SixLabors.ImageSharp](https://sixlabors.com/products/imagesharp/)
- Icons from [Font Awesome](https://fontawesome.com/)
