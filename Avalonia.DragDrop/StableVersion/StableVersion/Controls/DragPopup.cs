using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace StableVersion.Controls;

public class DragPopup : Popup
{
    public DragPopup()
    {
        PlacementMode = Avalonia.Controls.PlacementMode.Top;
        PlacementTarget = App.MainWin;
        HorizontalOffset = VerticalOffset = 0;
        IsHitTestVisible = false;
        IsLightDismissEnabled = false;
    }

    public Control DragItemIndicator
    {
        get { return Child; }
        set { Child = value; }
    }
}
