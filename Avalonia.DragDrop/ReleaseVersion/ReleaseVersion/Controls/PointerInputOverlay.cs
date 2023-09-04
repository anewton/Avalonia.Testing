using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using ReleaseVersion.DragControls;
using System.IO;

namespace ReleaseVersion.Controls;

public class PointerInputOverlay : Panel
{
    private static DragPopup _dragPopup;
    private static bool _isPointerTracked = false;
    private static PointerPressedEventArgs _pointerPressedEventArgs;
    private double _dragItemScaleFactor = 0.8;
    private static Control _source = null;

    public PointerInputOverlay() { }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {

        if (e.Pointer.Type == PointerType.Pen)
        {
            _pointerPressedEventArgs = e;

            if (_source == null && e.Source.GetType().IsAssignableTo(typeof(DraggableControl)))
                _source = (Control)e.Source;

            if (_source?.DataContext == null)
                return;

            if (_isPointerTracked && _source != null) // 2nd time pointer was pressed after a pick
            {
                e.Handled = true;
                DataObject data = new();
                data.Set(DataFormats.Text, _source.DataContext.GetType().Name);
                data.Set("Object", _source);

                DragDrop.DoDragDrop(_pointerPressedEventArgs, data, DragDropEffects.Move);
                ResetDragDropIndicator();
            }
            else if (!_isPointerTracked && _source != null)
            {
                e.Handled = true;
                _isPointerTracked = true;
                var clickPosition = e.GetCurrentPoint(_source).Position;
                if (_dragPopup == null)
                {
                    _dragPopup = new DragPopup()
                    {
                        IsVisible = false
                    };
                    if (!Children.Contains(_dragPopup))
                        Children.Add(_dragPopup);
                }

                if (_dragPopup.DragItemIndicator == null)
                {
                    var imageControl = RenderToBitmap(_source, _dragItemScaleFactor);
                    _dragPopup.DragItemIndicator = imageControl;
                }
                if (!_dragPopup.IsOpen)
                {
                    var mousePosition = _pointerPressedEventArgs.GetPosition(_source);
                    _dragPopup.PlacementTarget = (Control)_source;
                    _dragPopup.HorizontalOffset = mousePosition.X - clickPosition.X * _dragItemScaleFactor;
                    _dragPopup.VerticalOffset = _source.Bounds.Height - clickPosition.Y * _dragItemScaleFactor + mousePosition.Y;
                    _dragPopup.Open();
                }
            }
        }
        base.OnPointerPressed(e);
    }

    private static void ResetDragDropIndicator()
    {
        _source = null;
        _isPointerTracked = false;
        _dragPopup.DragItemIndicator = null;
        _dragPopup.Close();
    }

    private static Avalonia.Controls.Image RenderToBitmap(Control target, double imageScaleFactor)
    {
        var targetLastPosition = target.Bounds.Position;
        var targetWidth = target.Bounds.Width;
        var targetHeight = target.Bounds.Height;
        var pixelSize = new PixelSize((int)targetWidth, (int)targetHeight);
        var size = new Size(targetWidth, targetHeight);
        using RenderTargetBitmap bitmap = new(pixelSize, new Vector(96, 96));
        target.Measure(size);
        target.Arrange(new Rect(size));
        bitmap.Render(target);
        using var bitmapStream = new MemoryStream();
        bitmap.Save(bitmapStream);
        bitmapStream.Position = 0;
        var image = new Avalonia.Media.Imaging.Bitmap(bitmapStream);
        target.Arrange(new Rect(targetLastPosition, size));
        var scaledWidth = target.Bounds.Width * imageScaleFactor;
        var scaledHeight = target.Bounds.Height * imageScaleFactor;
        var imageControl = new Avalonia.Controls.Image() { Source = image, Width = scaledWidth, Height = scaledHeight };
        return imageControl;
    }
}
