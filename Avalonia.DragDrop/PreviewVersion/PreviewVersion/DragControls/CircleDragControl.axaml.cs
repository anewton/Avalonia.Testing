using Avalonia.Markup.Xaml;

namespace PreviewVersion.DragControls;

public partial class CircleDragControl : DragEnabledPanel
{
    public CircleDragControl()
    {
        InitializeComponent();     
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
