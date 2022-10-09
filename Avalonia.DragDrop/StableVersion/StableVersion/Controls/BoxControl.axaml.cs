using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace StableVersion.Controls;

public partial class BoxControl : UserControl
{
    public BoxControl()
    {
        InitializeComponent();
        InitDragDrop();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void InitDragDrop()
    {
        var backgroundBorder = this.FindControl<Border>("boxBackgroundBorder");
        backgroundBorder.PointerPressed += async (sender, e) =>
        {
            DataObject data = new();
            data.Set(DataFormats.Text, nameof(BoxControl));
            data.Set("Object", this);
            await DragDrop.DoDragDrop(e, data, DragDropEffects.Copy | DragDropEffects.Move);
            e.Handled = true;
        };
        backgroundBorder.PointerReleased += (sender, e) =>
        {
            e.Handled = true;
        };
    }
}