using Avalonia.Media;

namespace Manager.GUI.Services.Nginx;

public static class NginxIcons
{
    // A Blue "Keyword" Box Icon
    public static IImage Directive { get; } = new DrawingImage
    {
        Drawing = new GeometryDrawing
        {
            Brush = SolidColorBrush.Parse("#4ec9b0"), // VS Code Cyan/Blue
            Pen = new Pen(SolidColorBrush.Parse("#4ec9b0"), 1),
            Geometry = Geometry.Parse("M2,5 L2,11 L14,11 L14,5 Z M2,5 L14,5 M5,8 L11,8")
            // Simple box representation
        }
    };

    // A Purple "Variable" Icon (Cube-ish shape)
    public static IImage Variable { get; } = new DrawingImage
    {
        Drawing = new GeometryDrawing
        {
            Brush = SolidColorBrush.Parse("#c586c0"), // VS Code Purple
            Pen = new Pen(SolidColorBrush.Parse("#c586c0"), 1),
            Geometry = Geometry.Parse("M8,2 L2,5 L2,11 L8,14 L14,11 L14,5 Z M2,5 L8,8 L14,5 M8,14 L8,8")
            // Isometric cube representation
        }
    };
}