using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace ReleaseVersion.DragControls;

public class DragPopup : Popup
{
    public DragPopup()
    {
        Placement = PlacementMode.Top;
        HorizontalOffset = VerticalOffset = 0;
        IsHitTestVisible = false;
        IsLightDismissEnabled = false;
        OverlayDismissEventPassThrough = false;
        Topmost = true;
    }

    public Control DragItemIndicator
    {
        get { return Child; }
        set { Child = value; }
    }
}