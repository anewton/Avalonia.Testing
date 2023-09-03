using WorkingVersion.Models;
using WorkingVersion.ViewModels;

namespace WorkingVersion.Controls;

public class DesignData
{
    public static MainViewModel DesignTimeMainViewModel => new();

    public static Box DesignTimeBox => new() { Name = "Box" };
}
