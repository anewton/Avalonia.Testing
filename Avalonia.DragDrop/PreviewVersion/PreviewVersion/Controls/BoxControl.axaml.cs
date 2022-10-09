using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace PreviewVersion.Controls;

public partial class BoxControl : UserControl
{
    private Border _dragTarget;

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
        _dragTarget = this.FindControl<Border>("boxBackgroundBorder");
        _dragTarget.PointerPressed += async (sender, e) =>
        {
            if (e.Pointer.Type == PointerType.Mouse || e.Pointer.Type == PointerType.Pen || e.Pointer.Type == PointerType.Touch)
            {
                DataObject data = new();
                data.Set(DataFormats.Text, nameof(BoxControl));
                data.Set("Object", this);
                await DragDrop.DoDragDrop(e, data, DragDropEffects.Copy | DragDropEffects.Move);
                e.Handled = true;
            }
        };
        _dragTarget.PointerReleased += (sender, e) =>
        {
            e.Handled = true;
        };
    }
}