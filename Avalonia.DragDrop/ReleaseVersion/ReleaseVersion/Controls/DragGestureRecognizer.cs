using Avalonia;
using Avalonia.Input;
using Avalonia.Input.GestureRecognizers;
using System;

namespace ReleaseVersion.Controls;

public class DragGestureRecognizer : GestureRecognizer
{
    private IPointer _pointerContact;
    private Visual _visual;

    internal Action<PointerEventArgs> OnPointerMoved;
    internal Action<PointerPressedEventArgs> OnPointerPressed;
    internal Action<PointerReleasedEventArgs> OnPointerReleased;
    internal Action<PointerCaptureLostEventArgs> OnPointerCaptureLost;

    protected override void PointerCaptureLost(IPointer pointer)
    {
        if (pointer.Type == PointerType.Touch || pointer.Type == PointerType.Pen)
        {
            _pointerContact = null;
            var pointerCaptureLostEventArgs = new PointerCaptureLostEventArgs(_visual, pointer);
            pointerCaptureLostEventArgs.Handled = true;
            OnPointerCaptureLost?.Invoke(pointerCaptureLostEventArgs);
        }
    }

    protected override void PointerReleased(PointerReleasedEventArgs e)
    {
        if (e.Pointer.Type == PointerType.Touch || e.Pointer.Type == PointerType.Pen)
        {
            _pointerContact = null;
            OnPointerReleased?.Invoke(e);
        }
    }

    protected override void PointerMoved(PointerEventArgs e)
    {
        if (Target != null && Target is Visual visual && (e.Pointer.Type == PointerType.Touch || e.Pointer.Type == PointerType.Pen))
        {
            OnPointerMoved?.Invoke(e);
        }
    }

    protected override void PointerPressed(PointerPressedEventArgs e)
    {
        if (Target != null && Target is Visual visual && (e.Pointer.Type == PointerType.Touch || e.Pointer.Type == PointerType.Pen))
        {
            _pointerContact ??= e.Pointer;
            if (_pointerContact != null)
            {
                Capture(_pointerContact);
                _visual = visual;
                OnPointerPressed?.Invoke(e);
            }
        }
    }
}
