using ReleaseVersion.Models;
using ReleaseVersion.ViewModels;

namespace ReleaseVersion.Controls;

public class DesignData
{
    public static MainViewModel DesignTimeMainViewModel => new();

    public static Box DesignTimeBox => new() { Name = "Box" };
}
