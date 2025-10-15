# R2CSharp

A reimplementation of the R2C GUI script (Reboot2Config) for Switchroot Linux distros using C# and Avalonia UI, made to mimic and resemble [Hekate](https://github.com/CTCaer/hekate).

<p align="center">
  <video src="https://github.com/user-attachments/assets/bcb291d8-cfc1-4ffa-a124-128e8d1c454c"></video>
</p>

## Overview

R2CSharp is a modern, responsive and touch-friendly interface that provides an intuitive way to select reboot options when using Linux on the Nintendo Switch.

### Requirements

None, all runtimes and libraries are embedded inside the binary.

### Features

- **Touch & Keyboard Support**: Touch gesture and keyboard navigation (set the joy-cons to mouse mode to navigate)
- **Theme Support**: Customizable theming (uses color from Nyx settings in Hekate)
- **Adaptive Scaling**: Adapts to different resolutions (only 16:9 since that's the aspect ratio of the Switch)

## Building

Note: **building requires [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)**

```bash
git clone https://github.com/LordBubblesDev/r2csharp
cd R2CSharp
dotnet build
```

To run the app after building, use the following command:

```bash
dotnet run --project src/R2CSharp.App
```

## Configuration

The application reads configuration from:

- `bootloader/hekate_ipl.ini` - Launch options
- `bootloader/ini/*.ini` - Additional configuration files
- `bootloader/nyx.ini` - Theme color and column settings

## Navigation

Note: *it's possible to navigate using the D-Pad and the A button when setting the joy-cons to mouse mode. The right joystick will function the same as a scroll wheel and can be used to switch pages.*

### Mouse Controls

- **Scroll Wheel**: Navigate between pages
- **Click**: Select option

### Keyboard Controls

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
- Icons from [Font Awesome](https://fontawesome.com/)
- Image processing with [SixLabors.ImageSharp](https://sixlabors.com/products/imagesharp/)
