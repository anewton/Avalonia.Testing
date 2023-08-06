using System.Collections.ObjectModel;
using ReleaseVersion.Models;

namespace ReleaseVersion.ViewModels;

public class MainViewModel
{
    public ObservableCollection<Box> Boxes { get; set; }

    public ObservableCollection<Box> Boxes2 { get; set; }

    public MainViewModel()
    {
        Boxes = new ObservableCollection<Box>()
        {
            new Box { Name = "Box 1"},
            new Box { Name = "Box 2"}
        };

        Boxes2 = new ObservableCollection<Box>()
        {
            new Box { Name = "Box 3"},
            new Box { Name = "Box 4"}
        };
    }
}

