using Avalonia;
using Avalonia.Media;
using System;

namespace ReleaseVersion.DragControls;

public class PointerInfo
{
    public Point Point { get; set; }
    public Color Color { get => GetRandomColor(); }

    private static Color GetRandomColor()
    {
        var colors = AllColors;
        var color = colors[new Random().Next(0, colors.Length - 1)];
        return color;
    }

    private static Color[] AllColors = new[]
    {
        Colors.Aqua,
        Colors.Beige,
        Colors.Chartreuse,
        Colors.Coral,
        Colors.Fuchsia,
        Colors.Crimson,
        Colors.Lavender,
        Colors.Orange,
        Colors.Orchid,
        Colors.ForestGreen,
        Colors.SteelBlue,
        Colors.PapayaWhip,
        Colors.PaleVioletRed,
        Colors.Goldenrod,
        Colors.Maroon,
        Colors.Moccasin,
        Colors.Navy,
        Colors.Wheat,
        Colors.Violet,
        Colors.Sienna,
        Colors.Indigo,
        Colors.Honeydew
    };
}

