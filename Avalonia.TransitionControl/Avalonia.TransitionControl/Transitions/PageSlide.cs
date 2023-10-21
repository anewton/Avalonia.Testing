using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;

namespace Avalonia.TransitionControl.Transitions;

/// <summary>
/// Transitions between two pages by sliding them horizontally or vertically.
/// </summary>
public class PageSlide : IPageTransition
{
    /// <summary>
    /// The axis on which the PageSlide should occur
    /// </summary>
    public enum SlideAxis
    {
        Horizontal,
        Vertical
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageSlide"/> class.
    /// </summary>
    public PageSlide()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageSlide"/> class.
    /// </summary>
    /// <param name="duration">The duration of the animation.</param>
    /// <param name="orientation">The axis on which the animation should occur</param>
    public PageSlide(TimeSpan duration, SlideAxis orientation = SlideAxis.Horizontal)
    {
        Duration = duration;
        Orientation = orientation;
    }

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Gets the duration of the animation.
    /// </summary>
    public SlideAxis Orientation { get; set; }

    /// <summary>
    /// Gets the forward direction of the animation
    /// </summary>
    public bool? Forward { get; set; }

    /// <summary>
    /// Gets or sets element entrance easing.
    /// </summary>
    public Easing SlideInEasing { get; set; } = new LinearEasing();

    /// <summary>
    /// Gets or sets element exit easing.
    /// </summary>
    public Easing SlideOutEasing { get; set; } = new LinearEasing();

    /// <inheritdoc />
    public virtual async Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        forward = Forward ?? true; // Set to styled property value

        List<Task> tasks = new();
        Visual parent = GetVisualParent(from, to);
        double distance = Orientation == SlideAxis.Horizontal ? parent.Bounds.Width : parent.Bounds.Height;
        StyledProperty<double> translateProperty = Orientation == SlideAxis.Horizontal ? TranslateTransform.XProperty : TranslateTransform.YProperty;

        if (from != null)
        {
            var animation = new Animation.Animation
            {
                Easing = SlideOutEasing,
                Children =
                {
                    new KeyFrame
                    {
                        Setters = { new Setter { Property = translateProperty, Value = 0d } },
                        Cue = new Cue(0d)
                    },
                    new KeyFrame
                    {
                        Setters = { new Setter { Property = translateProperty, Value = forward ? -distance : distance } },
                        Cue = new Cue(1d)
                    }
                },
                Duration = Duration
            };

            tasks.Add(animation.RunAsync(from, cancellationToken).ContinueWith((x) =>
            {
                if (x.IsCompletedSuccessfully)
                {
                    from.IsVisible = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()));
        }

        if (to != null)
        {

            to.IsVisible = true;
            var animation = new Animation.Animation
            {
                Easing = SlideInEasing,
                Children =
                {
                    new KeyFrame
                    {
                        Setters = {new Setter { Property = translateProperty, Value = forward ? distance : -distance } },
                        Cue = new Cue(0d)
                    },
                    new KeyFrame
                    {
                        Setters = { new Setter { Property = translateProperty, Value = 0d } },
                        Cue = new Cue(1d)
                    }
                },
                Duration = Duration
            };
            tasks.Add(animation.RunAsync(to, cancellationToken).ContinueWith((x) =>
            {
                if (x.IsCompletedSuccessfully)
                {
                    from.IsVisible = false;
                }
            }, TaskScheduler.FromCurrentSynchronizationContext()));
        }

        await Task.WhenAll(tasks);

        if (from != null && !cancellationToken.IsCancellationRequested)
        {
            from.IsVisible = false;
        }

    }

    /// <summary>
    /// Gets the common visual parent of the two control.
    /// </summary>
    /// <param name="from">The from control.</param>
    /// <param name="to">The to control.</param>
    /// <returns>The common parent.</returns>
    /// <exception cref="ArgumentException">
    /// The two controls do not share a common parent.
    /// </exception>
    /// <remarks>
    /// Any one of the parameters may be null, but not both.
    /// </remarks>
    protected static Visual GetVisualParent(Visual from, Visual to)
    {
        Visual p1 = (from ?? to)!.GetVisualParent();
        Visual p2 = (to ?? from)!.GetVisualParent();

        if (p1 != null && p2 != null && p1 != p2)
        {
            throw new ArgumentException("Controls for PageSlide must have same parent.");
        }

        return p1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
    }
}
