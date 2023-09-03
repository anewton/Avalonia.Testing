using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;

namespace WorkingVersion.DragControls;

public class DraggableControl : Panel
{
    private DispatcherTimer _holdTimer;
    private PointerPressedEventArgs _pointerPressedEventArgs;
    private bool _isPointerTracked = false;

    private readonly Dictionary<IPointer, PointerInfo> _pointers = new();

    private DragPopup _dragPopup;
    Point _dragItemClickPosition = default;
    private double _dragItemScaleFactor = 0.8;

    public DraggableControl()
    {
    }

    class PointerInfo
    {
        public Point Point { get; set; }
        public Color Color { get; } = Colors.Yellow;
    }

    class DragPopup : Popup
    {
        public DragPopup()
        {
            PlacementMode = Avalonia.Controls.PlacementMode.Top;
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
            Children.Add(_dragPopup);
        }
    }

    private void ResetDragDropIndicator()
    {
        _dragPopup.Close();
        _dragPopup.DragItemIndicator = null;
    }

    private void OnPointerPressAndHold(object sender, EventArgs ergs)
    {
        _holdTimer.Stop();
        if (!_isPointerTracked)
            return;

        if (_dragPopup.DragItemIndicator == null)
        {
            var imageControl = new Avalonia.Controls.Image() { Source = RenderToBitmap(this) };
            Viewbox viewBoxDragIndicator = new()
            {
                IsHitTestVisible = false,
                Stretch = Stretch.Uniform,
                StretchDirection = StretchDirection.Both,
                Width = Bounds.Width * _dragItemScaleFactor,
                Child = imageControl
            };

            _dragPopup.DragItemIndicator = viewBoxDragIndicator;
        }
        if (!_dragPopup.IsOpen)
        {
            var mousePosition = _pointerPressedEventArgs.GetPosition(this);
            _dragPopup.HorizontalOffset = mousePosition.X - _dragItemClickPosition.X * _dragItemScaleFactor;
            _dragPopup.VerticalOffset = (Bounds.Height * _dragItemScaleFactor) - _dragItemClickPosition.Y * _dragItemScaleFactor + mousePosition.Y;
            _dragPopup.Open();
        }
    }

    private static Bitmap RenderToBitmap(Control target)
    {
        var targetLastPosition = target.Bounds.Position;
        var imageScaleFactor = 0.10;
        var targetWidth = target.Bounds.Width + (imageScaleFactor * target.Bounds.Width);
        var targetHeight = target.Bounds.Height + (imageScaleFactor * target.Bounds.Height);
        var pixelSize = new PixelSize((int)targetWidth, (int)targetHeight);
        var size = new Size(targetWidth, targetHeight);
        using RenderTargetBitmap bitmap = new RenderTargetBitmap(pixelSize, new Vector(96, 96));
        target.Measure(size);
        target.Arrange(new Rect(size));
        bitmap.Render(target);
        using var bitmapStream = new MemoryStream();
        bitmap.Save(bitmapStream);
        bitmapStream.Position = 0;
        var image = new Avalonia.Media.Imaging.Bitmap(bitmapStream);
        target.Arrange(new Rect(targetLastPosition, size));
        return image;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
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
            }
        }

        base.OnPointerMoved(e);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (_isPointerTracked)
            return;
        UpdatePointer(e);
        e.Pointer.Capture(this);
        e.Handled = true;
        if (_holdTimer == null)
            _holdTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Background, OnPointerPressAndHold);
        _holdTimer.Start();
        _pointerPressedEventArgs = e;
        _isPointerTracked = true;
        _dragItemClickPosition = e.GetCurrentPoint(this).Position;
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (!_isPointerTracked)
            return;
        _pointers.Remove(e.Pointer);
        e.Handled = true;

        if (_holdTimer != null && _holdTimer.IsEnabled)
            _holdTimer.Stop();

        DataObject data = new();
        data.Set(DataFormats.Text, this.DataContext.GetType().Name);
        data.Set("Object", this);

        DragDrop.DoDragDrop(_pointerPressedEventArgs, data, DragDropEffects.Move);

        _isPointerTracked = false;
        ResetDragDropIndicator();
        InvalidateVisual();
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (!_isPointerTracked)
            return;
        _pointers.Remove(e.Pointer);
        if (_holdTimer != null && _holdTimer.IsEnabled)
            _holdTimer.Stop();
        _isPointerTracked = false;
        ResetDragDropIndicator();
        InvalidateVisual();
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
}
