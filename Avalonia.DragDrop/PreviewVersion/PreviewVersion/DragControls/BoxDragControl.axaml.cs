using Avalonia.Markup.Xaml;

namespace PreviewVersion.DragControls;

public partial class BoxDragControl : DragEnabledPanel
{
    public BoxDragControl()
    {
        InitializeComponent();     
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
