using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.TransitionControl.Transitions;

namespace Avalonia.TransitionControl.Controls;

[TemplatePart(ITEMSCONTAINER, typeof(Grid))]
public class ObjectPageControl : TemplatedControl
{
    private const string ITEMSCONTAINER = "PART_ItemsContainer";
    private Grid _itemsContainer;

    private bool _shouldAnimate;

    private CancellationTokenSource _currentTransition;

    public ObjectPageControl()
    {
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == SelectedObjectProperty)
        {
            UpdateContent(true);
            return;
        }
        //base.OnPropertyChanged(change);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (Control item in _itemsContainer.Children)
        {
            item.Height = finalSize.Height;
            item.Width = finalSize.Width;
        }
        return base.ArrangeOverride(finalSize);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemsContainer = e.NameScope.Find<Grid>(ITEMSCONTAINER);
        _itemsContainer.Children.AddRange(ChildControls);
        foreach (Control item in ChildControls)
        {
            item.IsVisible = SelectedObjectEqualsControlDataContext(item);
        }
        UpdateContent(false);
    }

    private bool SelectedObjectEqualsControlDataContext(Control item) => SelectedObject != null && item.DataContext != null && item.DataContext.Equals(SelectedObject);

    private void UpdateContent(bool shouldAnimate)
    {
        _shouldAnimate = shouldAnimate;
        if (ChildControls.Any())
        {
            Control toControl = null;
            Control fromControl = null;

            toControl = ChildControls.Where(SelectedObjectEqualsControlDataContext).FirstOrDefault();
            fromControl = ChildControls.Where(item => item.IsVisible).FirstOrDefault();

            if (toControl == null || fromControl == null)
                return;

            if (_shouldAnimate && !toControl.DataContext.Equals(fromControl.DataContext))
            {
                _currentTransition?.Cancel();

                if (PageTransition is { } transition)
                {
                    _shouldAnimate = false;

                    var cancel = new CancellationTokenSource();
                    _currentTransition = cancel;
                    transition.Start(fromControl, toControl, true, _currentTransition.Token);
                    //.ContinueWith(x =>
                    //{
                    //    if (!_currentTransition.IsCancellationRequested)
                    //    {
                    //        fromControl.IsVisible = false;
                    //    }
                    //}, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }
    }

    public static readonly StyledProperty<object> SelectedObjectProperty =
        AvaloniaProperty.Register<ObjectPageControl, object>(nameof(SelectedObject), null);

    public object SelectedObject
    {
        get => GetValue(SelectedObjectProperty);
        set => SetValue(SelectedObjectProperty, value);
    }

    public static readonly DirectProperty<ObjectPageControl, IEnumerable<Control>> ChildControlsProperty =
    AvaloniaProperty.RegisterDirect<ObjectPageControl, IEnumerable<Control>>(
        nameof(ChildControls),
        owner => owner.ChildControls,
        (owner, value) => owner.ChildControls = value);

    public IEnumerable<Control> ChildControls
    {
        get { return _childControls; }
        set { SetAndRaise(ChildControlsProperty, ref _childControls, value); }
    }
    private IEnumerable<Control> _childControls = new List<Control>();

    public static readonly StyledProperty<IPageTransition> PageTransitionProperty =
        AvaloniaProperty.Register<ObjectPageControl, IPageTransition>(
            nameof(PageTransition),
            //defaultValue: new ImmutableCrossFade(TimeSpan.FromMilliseconds(500)));
            defaultValue: new ImmutableCustomTransition(TimeSpan.FromMilliseconds(500)));
    

    public IPageTransition PageTransition
    {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    private class ImmutableCrossFade : IPageTransition
    {
        private readonly CrossFade _inner;

        public ImmutableCrossFade(TimeSpan duration) => _inner = new CrossFade(duration);

        public Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken)
        {
            return _inner.Start(from, to, cancellationToken);
        }
    }

    private class ImmutablePageSlide : IPageTransition
    {
        private readonly Avalonia.Animation.PageSlide _inner;

        public ImmutablePageSlide(TimeSpan duration, Avalonia.Animation.PageSlide.SlideAxis orientation) => _inner = new Avalonia.Animation.PageSlide(duration, orientation);

        public Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken)
        {
            return _inner.Start(from, to, forward, cancellationToken);
        }
    }

    private class ImmutableCustomTransition : IPageTransition
    {
        private readonly CustomTransition _inner;

        public ImmutableCustomTransition(TimeSpan duration) => _inner = new CustomTransition(duration);

        public Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken)
        {
            return _inner.Start(from, to, forward, cancellationToken);
        }
    }
}
