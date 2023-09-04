using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace ReleaseVersion.Controls;

public partial class SampleDragAndDropControl : UserControl
{
    private Grid _draggable;

    public SampleDragAndDropControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        _draggable = this.FindControl<Grid>("draggable");
        _draggable.PointerPressed += DoDrag;
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    private void DragOver(object sender, DragEventArgs e)
    {
        e.DragEffects &= DragDropEffects.Move;
    }

    private void Drop(object sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Text))
        {
            var droppedControlName = e.Data.GetText();
        }
    }

    async void DoDrag(object sender, PointerPressedEventArgs e)
    {
        if (_draggable.DataContext == null)
            return;
        DataObject dragData = new();
        dragData.Set(DataFormats.Text, _draggable.DataContext.GetType().Name);
        dragData.Set("Object", _draggable);

        var result = await DragDrop.DoDragDrop(e, dragData, DragDropEffects.Move);
        switch (result)
        {
            case DragDropEffects.Move:
                
                break;
            case DragDropEffects.Copy:
                
                break;
            case DragDropEffects.Link:
                
                break;
            case DragDropEffects.None:
                
                break;
            default:
                
                break;
        }
    }


}
