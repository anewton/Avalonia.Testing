using PreviewVersion.Models;
using PreviewVersion.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreviewVersion.Controls;

public class DesignData
{
    public static MainViewModel DesignTimeMainViewModel => new();

    public static Box DesignTimeBox => new() { Name = "Box" };
}
