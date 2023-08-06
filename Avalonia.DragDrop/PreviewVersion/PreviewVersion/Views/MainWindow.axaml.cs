using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace PreviewVersion.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        var rootPanel = this.FindControl<Panel>("ROOTPANEL");
        var page = AvaloniaXamlLoader.Load(new Uri($"avares://PreviewVersion/Views/MainView.axaml", UriKind.RelativeOrAbsolute)) as IControl;
        rootPanel.Children.Clear();
        rootPanel.Children.Add(page);
    }

    
}