using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StableVersion.Controls;

public class DragIndicator
{
    private static DragPopup _instance = null;
    private static readonly object synclock = new object();

    private DragIndicator()
    {
    }

    public static DragPopup Instance
    {
        get
        {
            lock (synclock)
            {
                if (_instance == null)
                {
                    _instance = (DragPopup)App.MainWin.DragDropIndicatorPopup;
                }
                return _instance;
            }
        }
    }
}
