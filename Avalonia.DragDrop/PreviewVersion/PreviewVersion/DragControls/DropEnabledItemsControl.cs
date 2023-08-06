using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using System;
using System.Collections;

namespace PreviewVersion.DragControls;

public class DropEnabledItemsControl : ItemsControl, IStyleable
{
    Type IStyleable.StyleKey => typeof(ItemsControl);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        //SetValue(DragDrop.AllowDropProperty, true);
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void Drop(object sender, DragEventArgs e)
    {
        IControl dragSource = e.Data.Get("Object") as IControl;
        IControl dropTarget = e.Source as IControl;

        if (dragSource != null && dropTarget != null)
        {
            var sourceItemsControl = FindFirstControlOfType<DropEnabledItemsControl>(dragSource) as DropEnabledItemsControl;
            var targetItemsControl = FindFirstControlOfType<DropEnabledItemsControl>(dropTarget) as DropEnabledItemsControl;

            if (sourceItemsControl == targetItemsControl)
            {
                var index = 0;
                var collectionSize = 0;
                var enumerator = targetItemsControl.Items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    collectionSize++;
                    if (enumerator.Current.Equals(dropTarget.DataContext))
                        break;
                    index++;
                }
                if (index >= collectionSize)
                    index = collectionSize - 1;
                if (index == -1)
                    index = 0;

                IList dropCollection = targetItemsControl.Items as IList;
                dropCollection.Remove((object)dragSource.DataContext);
                dropCollection.Insert(index, (object)dragSource.DataContext);
                e.Handled = true;
            }
            else if (sourceItemsControl != targetItemsControl)
            {
                var index = 0;
                var collectionSize = 0;
                var enumerator = targetItemsControl.Items.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    collectionSize++;
                    if (enumerator.Current.Equals(dropTarget.DataContext))
                        break;
                    index++;
                }
                if (index >= collectionSize)
                    index = collectionSize - 1;
                if (index == -1)
                    index = 0;

                IList sourceCollection = sourceItemsControl.Items as IList;
                sourceCollection.Remove((object)dragSource.DataContext);

                IList targetCollection = targetItemsControl.Items as IList;
                targetCollection.Insert(index, (object)dragSource.DataContext);
                e.Handled = true;
            }
        }
    }

    private IControl FindFirstControlOfType<T>(IControl aParent)
    {
        if (aParent.GetType() == typeof(T))
            return aParent;
        if (aParent.Parent != null)
            return FindFirstControlOfType<T>(aParent.Parent);
        else
            return null;
    }
}

