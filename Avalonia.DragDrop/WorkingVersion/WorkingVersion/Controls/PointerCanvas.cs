using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace WorkingVersion.Controls;

public class PointerCanvas : Control
{
    private readonly Slider _slider;
    private readonly TextBlock _status;
    private int _events;
    private Stopwatch _stopwatch = Stopwatch.StartNew();
    private Dictionary<int, PointerPoints> _pointers = new();

    public PointerCanvas()
    {
            
    }


    class PointerPoints
    {
        struct CanvasPoint
        {
            public IBrush Brush;
            public Point Point;
            public double Radius;
        }

        readonly CanvasPoint[] _points = new CanvasPoint[1000];
        int _index;

        public void Render(DrawingContext context)
        {

            CanvasPoint? prev = null;
            for (var c = 0; c < _points.Length; c++)
            {
                var i = (c + _index) % _points.Length;
                var pt = _points[i];
                if (prev.HasValue && prev.Value.Brush != null && pt.Brush != null)
                    context.DrawLine(new Pen(Brushes.Black), prev.Value.Point, pt.Point);
                prev = pt;
                if (pt.Brush != null)
                    context.DrawEllipse(pt.Brush, null, pt.Point, pt.Radius, pt.Radius);

            }

        }

        void AddPoint(Point pt, IBrush brush, double radius)
        {
            _points[_index] = new CanvasPoint { Point = pt, Brush = brush, Radius = radius };
            _index = (_index + 1) % _points.Length;
        }

        public void HandleEvent(PointerEventArgs e, Visual v)
        {
            e.Handled = true;
            if (e.RoutedEvent == PointerPressedEvent)
                AddPoint(e.GetPosition(v), Brushes.Green, 10);
            else if (e.RoutedEvent == PointerReleasedEvent)
                AddPoint(e.GetPosition(v), Brushes.Red, 10);
            else
            {
                var pts = e.GetIntermediatePoints(v);
                for (var c = 0; c < pts.Count; c++)
                {
                    var pt = pts[c];
                    AddPoint(pt.Position, c == pts.Count - 1 ? Brushes.Blue : Brushes.Black,
                        c == pts.Count - 1 ? 5 : 2);
                }
            }
        }
    }

    public PointerCanvas(Slider slider, TextBlock status)
    {
        _slider = slider;
        _status = status;
        DispatcherTimer.Run(() =>
        {
            if (_stopwatch.Elapsed.TotalSeconds > 1)
            {
                _status.Text = "Events per second: " + (_events / _stopwatch.Elapsed.TotalSeconds);
                _stopwatch.Restart();
                _events = 0;
            }

            return this.GetVisualRoot() != null;
        }, TimeSpan.FromMilliseconds(10));
    }


    void HandleEvent(PointerEventArgs e)
    {
        _events++;

        InvalidateVisual();

        if (e.RoutedEvent == PointerReleasedEvent && e.Pointer.Type == PointerType.Touch)
        {
            _pointers.Remove(e.Pointer.Id);
            return;
        }

        if (!_pointers.TryGetValue(e.Pointer.Id, out var pt))
            _pointers[e.Pointer.Id] = pt = new PointerPoints();
        pt.HandleEvent(e, this);


    }

    public override void Render(DrawingContext context)
    {
        context.FillRectangle(Brushes.White, Bounds);
        foreach (var pt in _pointers.Values)
            pt.Render(context);
        base.Render(context);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            _pointers.Clear();
            InvalidateVisual();
            return;
        }

        HandleEvent(e);
        base.OnPointerPressed(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        HandleEvent(e);
        base.OnPointerMoved(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        HandleEvent(e);
        base.OnPointerReleased(e);
    }
}
