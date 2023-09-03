using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Styling;
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace WorkingVersion.DragControls;

public class DropEnabledItemsControl : ItemsControl, IStyleable
{
    //protected override Type StyleKeyOverride => typeof(ItemsControl);
    Type IStyleable.StyleKey => typeof(ItemsControl);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void Drop(object sender, DragEventArgs e)
    {
        Control dragSource = e.Data.Get("Object") as Control;
        Control dropTarget = e.Source as Control;

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
                var dataContext = dragSource.DataContext;

                dropCollection.Remove((object)dragSource.DataContext);
                dragSource.DataContext = dataContext;
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
                if (index > collectionSize)
                    index = collectionSize - 1;
                if (index == -1)
                    index = 0;

                var dataContext = dragSource.DataContext;
                IList sourceCollection = sourceItemsControl.Items as IList;
                sourceCollection.Remove((object)dragSource.DataContext);

                if (targetItemsControl.Items == null)
                {

                    dragSource.DataContext = dataContext;
                    targetItemsControl.Items = new ObservableCollection<object>() { (object)dragSource.DataContext };
                }
                else
                {
                    IList targetCollection = targetItemsControl.Items as IList;
                    dragSource.DataContext = dataContext;
                    targetCollection.Insert(index, (object)dragSource.DataContext);
                    targetItemsControl.Items = targetCollection;
                }

                e.Handled = true;
            }
        }
    }

    private Control FindFirstControlOfType<T>(Control aParent)
    {
        if (aParent.GetType() == typeof(T))
            return aParent;
        if (aParent.Parent != null)
            return FindFirstControlOfType<T>((Control)aParent.Parent);
        else
            return null;
    }
}