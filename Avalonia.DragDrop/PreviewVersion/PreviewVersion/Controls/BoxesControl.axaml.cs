using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using PreviewVersion.Models;
using PreviewVersion.ViewModels;

namespace PreviewVersion.Controls;

public partial class BoxesControl : UserControl
{
    private const string CustomFormat = "application/xxx-avalonia-controlcatalog-custom";

    public BoxesControl()
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
        void DragOver(object sender, DragEventArgs e)
        {
            if (e.Source is Control c && e.Data.GetText() == nameof(BoxControl))
            {
                e.DragEffects = e.DragEffects & (DragDropEffects.Move);
                if (DragIndicator.Instance.DragItemIndicator == null)
                {
                    BoxControl draggedControl = e.Data.Get("Object") as BoxControl;
                    var indicatorControl = new BoxControl() { IsHitTestVisible = false, Opacity = 0.8, Height = draggedControl.Bounds.Height, Width = draggedControl.Bounds.Width, DataContext = ((Avalonia.StyledElement)e.Data.Get("Object")).DataContext as Box };
                    var viewboxControl = new Viewbox() { IsHitTestVisible = false, Stretch = Stretch.Uniform, StretchDirection = StretchDirection.Both, Width = draggedControl.Bounds.Width / 2 };
                    viewboxControl.Child = indicatorControl;
                    DragIndicator.Instance.DragItemIndicator = viewboxControl;
                }
                DragIndicator.Instance.Open();
            }

            if (DragIndicator.Instance.IsOpen)
            {
                var mousePosition = e.GetPosition(App.MainWin);
                DragIndicator.Instance.HorizontalOffset = mousePosition.X + DragIndicator.Instance.Child.Bounds.Height / 2;
                DragIndicator.Instance.VerticalOffset = mousePosition.Y + DragIndicator.Instance.Child.Bounds.Width / 2;
            }

            // Only allow if the dragged data contains text or filenames.
            if (!e.Data.Contains(DataFormats.Text)
                && !e.Data.Contains(DataFormats.FileNames)
                && !e.Data.Contains(CustomFormat))
            {
                e.DragEffects = DragDropEffects.None;
                ResetDragDropIndicator();
            }
        }

        void Drop(object sender, DragEventArgs e)
        {
            if (e.Source is Control c && e.Data.GetText() == nameof(BoxControl))
            {
                e.DragEffects = e.DragEffects & (DragDropEffects.Move);
                Box dragSource = ((Avalonia.StyledElement)e.Data.Get("Object")).DataContext as Box;
                Box dropTarget = ((Avalonia.StyledElement)e.Source).DataContext as Box;
                if (dragSource != null && dropTarget != null)
                {
                    var sourceItemsControl = this.FindControl<ItemsControl>("boxItemsControl");
                    var viewModel = sourceItemsControl.DataContext as MainViewModel;
                    var index = viewModel.Boxes.IndexOf(dropTarget);
                    if (index == -1)
                        index = 0;
                    viewModel.Boxes.Remove(dragSource);
                    viewModel.Boxes.Insert(index, dragSource);
                    e.Handled = true;
                }
            }
            ResetDragDropIndicator();
        }

        void ResetDragDrop(object sender, RoutedEventArgs e)
        {
            ResetDragDropIndicator();
        }

        void ResetDragDropIndicator()
        {
            DragIndicator.Instance.Close();
            DragIndicator.Instance.Child = null;
            DragIndicator.Instance.DragItemIndicator = null;
        }

        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
        AddHandler(DragDrop.DragLeaveEvent, ResetDragDrop);
    }
}