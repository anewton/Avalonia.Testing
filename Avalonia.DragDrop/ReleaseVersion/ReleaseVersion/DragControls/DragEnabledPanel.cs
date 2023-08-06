using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ReleaseVersion.DragControls;

public class DragEnabledPanel : Panel// Control, IChildIndexProvider
{
    ///// <summary>
    ///// Initializes a new instance of the <see cref="DragEnabledPanel"/> class.
    ///// </summary>
    //public DragEnabledPanel()
    //{
    //    Children.CollectionChanged += ChildrenChanged;
    //}

    class PointerInfo
    {
        public Point Point { get; set; }
        public Color Color { get; } = Colors.Yellow;
    }

    class DragPopup : Popup
    {
        public DragPopup()
        {
            Placement = PlacementMode.Top;
            HorizontalOffset = VerticalOffset = 0;
            IsHitTestVisible = false;
            IsLightDismissEnabled = false;
        }

        public Control DragItemIndicator
        {
            get { return Child; }
            set { Child = value; }
        }
    }

    private DispatcherTimer _holdTimer;
    private PointerPressedEventArgs _latestOnPressedEventArgs;
    private bool _isPointerTracked = false;

    private readonly Dictionary<IPointer, PointerInfo> _pointers = new();

    private DragPopup _dragPopup;
    Point _dragItemClickPosition = default;
    private double _dragItemScaleFactor = 0.8;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (_dragPopup == null)
        {
            _dragPopup = new DragPopup()
            {
                PlacementTarget = this,
                IsVisible = false
            };
            this.Children.Add(_dragPopup);
        }
    }

    private void ResetDragDropIndicator()
    {
        _dragPopup.Close();
        _dragPopup.Child = null;
        _dragPopup.DragItemIndicator = null;
    }

    private void OnPointerPressAndHold(object sender, EventArgs ergs)
    {
        _holdTimer.Stop();
        if (!_isPointerTracked)
            return;

        Control dragControl = null;
        if (_dragPopup.DragItemIndicator == null)
        {
            dragControl = (Control)Activator.CreateInstance(this.GetType());
            dragControl.IsHitTestVisible = false;
            dragControl.Opacity = 0.8;
            dragControl.Height = this.Bounds.Height;
            dragControl.Width = this.Bounds.Width;
            dragControl.DataContext = this.DataContext;

            Viewbox viewBoxDragIndicator = new()
            {
                IsHitTestVisible = false,
                Stretch = Stretch.Uniform,
                StretchDirection = StretchDirection.Both,
                Width = this.Bounds.Width * _dragItemScaleFactor,
                Child = dragControl
            };
            _dragPopup.DragItemIndicator = viewBoxDragIndicator;
        }
        _dragPopup.Open();
    }

    //public override void Render(DrawingContext context)
    //{
    //    foreach (var pt in _pointers.Values)
    //    {
    //        if (_dragPopup.IsOpen)
    //        {
    //            var mousePosition = pt.Point;
    //            _dragPopup.HorizontalOffset = mousePosition.X - _dragItemClickPosition.X * _dragItemScaleFactor;
    //            _dragPopup.VerticalOffset = _dragPopup.Child.Bounds.Height - _dragItemClickPosition.Y * _dragItemScaleFactor + mousePosition.Y;
    //        }
    //    }
    //    base.Render(context);
    //}

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_isPointerTracked)
        {
            UpdatePointer(e);
            foreach (var pt in _pointers.Values)
            {
                if (_dragPopup.IsOpen)
                {
                    var mousePosition = pt.Point;
                    _dragPopup.HorizontalOffset = mousePosition.X - _dragItemClickPosition.X * _dragItemScaleFactor;
                    _dragPopup.VerticalOffset = _dragPopup.Child.Bounds.Height - _dragItemClickPosition.Y * _dragItemScaleFactor + mousePosition.Y;
                    _dragPopup.UpdateLayout();
                }
            }
            e.Handled = true;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (_holdTimer == null)
            _holdTimer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Background, OnPointerPressAndHold);
        _holdTimer.Start();
        _latestOnPressedEventArgs = e;
        _isPointerTracked = true;
        _dragItemClickPosition = e.GetCurrentPoint(this).Position;
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (_holdTimer != null && _holdTimer.IsEnabled)
            _holdTimer.Stop();

        DataObject data = new();
        data.Set(DataFormats.Text, this.DataContext.GetType().Name);
        data.Set("Object", this);

        Dispatcher.UIThread.Post(() => DragDrop.DoDragDrop(_latestOnPressedEventArgs, data, DragDropEffects.Move), DispatcherPriority.Background);

        _isPointerTracked = false;
        ResetDragDropIndicator();
        InvalidateVisual();
        base.OnPointerReleased(e);
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Pen)
            return;
        if (_holdTimer != null && _holdTimer.IsEnabled)
            _holdTimer.Stop();
        _isPointerTracked = false;
        e.Handled = true;
        ResetDragDropIndicator();
        InvalidateVisual();
    }

    private void UpdatePointer(PointerEventArgs e)
    {
        if (!_pointers.TryGetValue(e.Pointer, out var info))
        {
            _pointers[e.Pointer] = info = new();
        }

        info.Point = e.GetPosition(this);
        InvalidateVisual();
    }

    #region IChildIndexProvider Implementation

    //private EventHandler<ChildIndexChangedEventArgs> _childIndexChanged;

    ///// <summary>
    ///// Gets the children of the <see cref="DragEnabledPanel"/>.
    ///// </summary>
    //[Content]
    //public Avalonia.Controls.Controls Children { get; } = new Avalonia.Controls.Controls();

    //event EventHandler<ChildIndexChangedEventArgs> IChildIndexProvider.ChildIndexChanged
    //{
    //    add
    //    {
    //        if (_childIndexChanged is null)
    //            Children.PropertyChanged += ChildrenPropertyChanged;
    //        _childIndexChanged += value;
    //    }

    //    remove
    //    {
    //        _childIndexChanged -= value;
    //        if (_childIndexChanged is null)
    //            Children.PropertyChanged -= ChildrenPropertyChanged;
    //    }
    //}

    ///// <summary>
    ///// Called when the <see cref="Children"/> collection changes.
    ///// </summary>
    ///// <param name="sender">The event sender.</param>
    ///// <param name="e">The event args.</param>
    //protected virtual void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //    switch (e.Action)
    //    {
    //        case NotifyCollectionChangedAction.Add:
    //            if (!IsItemsHost)
    //            {
    //                LogicalChildren.InsertRange(e.NewStartingIndex, e.NewItems!.OfType<Control>().ToList());
    //            }
    //            VisualChildren.InsertRange(e.NewStartingIndex, e.NewItems!.OfType<Visual>());
    //            break;

    //        case NotifyCollectionChangedAction.Move:
    //            if (!IsItemsHost)
    //            {
    //                LogicalChildren.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
    //            }
    //            VisualChildren.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
    //            break;

    //        case NotifyCollectionChangedAction.Remove:
    //            if (!IsItemsHost)
    //            {
    //                LogicalChildren.RemoveAll(e.OldItems!.OfType<Control>().ToList());
    //            }
    //            VisualChildren.RemoveAll(e.OldItems!.OfType<Visual>());
    //            break;

    //        case NotifyCollectionChangedAction.Replace:
    //            for (var i = 0; i < e.OldItems!.Count; ++i)
    //            {
    //                var index = i + e.OldStartingIndex;
    //                var child = (Control)e.NewItems![i]!;
    //                if (!IsItemsHost)
    //                {
    //                    LogicalChildren[index] = child;
    //                }
    //                VisualChildren[index] = child;
    //            }
    //            break;

    //        case NotifyCollectionChangedAction.Reset:
    //            throw new NotSupportedException();
    //    }

    //    _childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
    //    InvalidateMeasureOnChildrenChanged();
    //}

    //private protected virtual void InvalidateMeasureOnChildrenChanged()
    //{
    //    InvalidateMeasure();
    //}

    //private void ChildrenPropertyChanged(object sender, PropertyChangedEventArgs e)
    //{
    //    if (e.PropertyName == nameof(Children.Count) || e.PropertyName is null)
    //        _childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.TotalCountChanged);
    //}

    ///// <summary>
    ///// Gets whether the <see cref="DragEnabledPanel"/> hosts the items created by an <see cref="ItemsPresenter"/>.
    ///// </summary>
    //public bool IsItemsHost { get; internal set; }

    //int IChildIndexProvider.GetChildIndex(ILogical child)
    //{
    //    return child is Control control ? Children.IndexOf(control) : -1;
    //}

    ///// <inheritdoc />
    //bool IChildIndexProvider.TryGetTotalCount(out int count)
    //{
    //    count = Children.Count;
    //    return true;
    //}

    #endregion
}

