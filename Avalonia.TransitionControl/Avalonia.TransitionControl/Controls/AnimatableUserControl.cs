using Avalonia.Controls;

namespace Avalonia.TransitionControl.Controls;

public class AnimatableUserControl : UserControl
{
    private double _boundsY;
    private double _boundsX;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _boundsX = Bounds.X;
        _boundsY = Bounds.Y;
        base.OnAttachedToVisualTree(e);
    }

    public static readonly DirectProperty<AnimatableUserControl, double> BoundsXProperty =
    AvaloniaProperty.RegisterDirect<AnimatableUserControl, double>(
        nameof(BoundsX),
        o => o.BoundsX,
        (o, v) => o.BoundsX = v);
    public double BoundsX
    {
        get => _boundsX;
        set
        {
            SetAndRaise(BoundsXProperty, ref _boundsX, value);
            Bounds = new Rect(_boundsX, Bounds.Y, Bounds.Width, Bounds.Height);
        }
    }

    public static readonly DirectProperty<AnimatableUserControl, double> BoundsYProperty =
    AvaloniaProperty.RegisterDirect<AnimatableUserControl, double>(
        nameof(BoundsY),
        o => o.BoundsY,
        (o, v) => o.BoundsY = v);
    public double BoundsY
    {
        get => _boundsY;
        set
        {
            SetAndRaise(BoundsYProperty, ref _boundsY, value);
            Bounds = new Rect(Bounds.X, _boundsY, Bounds.Width, Bounds.Height);
        }
    }
}