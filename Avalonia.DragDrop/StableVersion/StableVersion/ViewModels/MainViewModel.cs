using System.Collections.Generic;
using System.Collections.ObjectModel;
using StableVersion.Models;

namespace StableVersion.ViewModels;

public class MainViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public ObservableCollection<Box> Boxes { get; set; }

    public MainViewModel()
    {
        Boxes = new ObservableCollection<Box>()
        {
            new Box { Name = "Box 1"},
            new Box { Name = "Box 2"}
        };
    }
}