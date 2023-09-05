using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using ReleaseVersion.Controls;
using System.Collections.Generic;
using System.IO;

namespace ReleaseVersion.DragControls;

public class DraggableControl : Panel
{
    protected readonly Dictionary<IPointer, PointerInfo> _pointers = new();
    protected Point _dragItemClickPosition = default;
    protected DragPopup _dragPopup;
    protected bool _isPointerTracked = false;
    protected PointerPressedEventArgs _pointerPressedEventArgs;
    protected double _dragItemScaleFactor = 0.8;
    private Point _touchInputPosition;
    private readonly DragGestureRecognizer _touchDragRecognizer;

    public DraggableControl()
    {
        _touchDragRecognizer = new DragGestureRecognizer();
        _touchDragRecognizer.OnPointerMoved += OnTouchPointerMoved;
        _touchDragRecognizer.OnPointerPressed += OnTouchPointerPressed;
        _touchDragRecognizer.OnPointerReleased += OnTouchPointerReleased;
        _touchDragRecognizer.OnPointerCaptureLost += OnTouchPointerCaptureLost;

        GestureRecognizers.Add(_touchDragRecognizer);
    }

    protected void OnTouchPointerMoved(PointerEventArgs e)
    {
        if (!_isPointerTracked)
            return;
        UpdatePointer(e);
        e.Handled = true;
        foreach (var pt in _pointers.Values)
        {
            if (_dragPopup.IsOpen)
            {
                _touchInputPosition = pt.Point;
                _dragPopup.HorizontalOffset = _touchInputPosition.X - _dragItemClickPosition.X * _dragItemScaleFactor;
                _dragPopup.VerticalOffset = _dragPopup.DragItemIndicator.Bounds.Height - _dragItemClickPosition.Y * _dragItemScaleFactor + _touchInputPosition.Y;
                _dragPopup.UpdateLayout();
            }
        }
    }

    protected void OnTouchPointerPressed(PointerPressedEventArgs e)
    {
        if (_isPointerTracked)
            return;
        UpdatePointer(e);
        e.Handled = true;

        _pointerPressedEventArgs = e;
        _isPointerTracked = true;
        _dragItemClickPosition = e.GetCurrentPoint(this).Position;

        if (_dragPopup.DragItemIndicator == null)
        {
            var imageControl = RenderToBitmap(this, _dragItemScaleFactor);
            _dragPopup.DragItemIndicator = imageControl;
        }
        if (!_dragPopup.IsOpen)
        {
            var mousePosition = _pointerPressedEventArgs.GetPosition(this);
            _dragPopup.HorizontalOffset = mousePosition.X - _dragItemClickPosition.X * _dragItemScaleFactor;
            _dragPopup.VerticalOffset = (Bounds.Height * _dragItemScaleFactor) - _dragItemClickPosition.Y * _dragItemScaleFactor + mousePosition.Y;
            _dragPopup.Open();
        }
    }

    protected void OnTouchPointerReleased(PointerReleasedEventArgs e)
    {
        if (DataContext == null)
            return;

        if (!_isPointerTracked)
            return;
        _pointers.Remove(e.Pointer);

        DataObject data = new();
        data.Set(DataFormats.Text, DataContext.GetType().Name);
        data.Set("Object", this);

        ResetDragDropIndicator();
        DragDrop.DoDragDrop(e, data, DragDropEffects.Move); // TODO: Fix for PointerType == Pen

        InvalidateVisual();
    }

    protected void OnTouchPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (!_isPointerTracked)
            return;
        _pointers.Remove(e.Pointer);
        e.Handled = true;

        ResetDragDropIndicator();
        InvalidateVisual();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_dragPopup == null)
        {
            _dragPopup = new DragPopup()
            {
                PlacementTarget = (Control)TemplatedParent,
                IsVisible = false
            };
            if (!Children.Contains(_dragPopup))
                Children.Add(_dragPopup);
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (e.Pointer.Type != PointerType.Mouse)
            return;

        if (!_isPointerTracked)
            return;
        UpdatePointer(e);
        e.Handled = true;
        foreach (var pt in _pointers.Values)
        {
            if (_dragPopup.IsOpen)
            {
                var mousePosition = pt.Point;
                _dragPopup.HorizontalOffset = mousePosition.X - _dragItemClickPosition.X * _dragItemScaleFactor;
                _dragPopup.VerticalOffset = _dragPopup.DragItemIndicator.Bounds.Height - _dragItemClickPosition.Y * _dragItemScaleFactor + mousePosition.Y;
                _dragPopup.UpdateLayout();
            }
        }
        base.OnPointerMoved(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.Pointer.Type != PointerType.Mouse)
        {
            e.Source = this;
            base.OnPointerPressed(e);
            return;
        }

        if (_isPointerTracked)
            return;
        UpdatePointer(e);
        e.Pointer.Capture(this);
        e.Handled = true;

        _pointerPressedEventArgs = e;
        _isPointerTracked = true;
        _dragItemClickPosition = e.GetCurrentPoint(this).Position;

        if (_dragPopup.DragItemIndicator == null)
        {
            var imageControl = RenderToBitmap(this, _dragItemScaleFactor);
            _dragPopup.DragItemIndicator = imageControl;
        }
        if (!_dragPopup.IsOpen)
        {
            var mousePosition = _pointerPressedEventArgs.GetPosition(this);
            _dragPopup.HorizontalOffset = mousePosition.X - _dragItemClickPosition.X * _dragItemScaleFactor;
            _dragPopup.VerticalOffset = (Bounds.Height * _dragItemScaleFactor) - _dragItemClickPosition.Y * _dragItemScaleFactor + mousePosition.Y;
            _dragPopup.Open();
        }

        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (DataContext == null)
            return;

        if (e.Pointer.Type != PointerType.Mouse)
            return;

        if (!_isPointerTracked)
            return;
        _pointers.Remove(e.Pointer);
        e.Handled = true;

        DataObject data = new();
        data.Set(DataFormats.Text, DataContext.GetType().Name);
        data.Set("Object", this);

        DragDrop.DoDragDrop(_pointerPressedEventArgs, data, DragDropEffects.Move);
        ResetDragDropIndicator();
        InvalidateVisual();
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (e.Pointer.Type != PointerType.Mouse)
            return;

        if (!_isPointerTracked)
            return;
        _pointers.Remove(e.Pointer);
        ResetDragDropIndicator();
        InvalidateVisual();
    }

    private void ResetDragDropIndicator()
    {
        _isPointerTracked = false;
        _dragPopup.Close();
        _dragPopup.DragItemIndicator = null;
    }

    private void UpdatePointer(PointerEventArgs e)
    {
        if (!_pointers.TryGetValue(e.Pointer, out var info))
        {
            if (e.RoutedEvent == PointerMovedEvent)
                return;
            _pointers[e.Pointer] = info = new();
        }
        info.Point = e.GetPosition(this);
        InvalidateVisual();
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
