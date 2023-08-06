using System;
using System.Collections.Generic;
using Avalonia;

namespace ReleaseVersion.Desktop
{
    class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .With(new Win32PlatformOptions
            {
                OverlayPopups = true,
                WinUICompositionBackdropCornerRadius = 0,
                CompositionMode = new List<Win32CompositionMode>() { Win32CompositionMode.WinUIComposition, Win32CompositionMode.RedirectionSurface }
            })
            .With(new Avalonia.X11PlatformOptions
            {
                OverlayPopups = true,
                EnableMultiTouch = false
            })
            .LogToTrace();

    }
}
