using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace ReleaseVersion.Views;

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
        var page = AvaloniaXamlLoader.Load(new Uri($"avares://ReleaseVersion/Views/MainView.axaml", UriKind.RelativeOrAbsolute)) as Control;
        rootPanel.Children.Clear();
        rootPanel.Children.Add(page);
    }

    
}