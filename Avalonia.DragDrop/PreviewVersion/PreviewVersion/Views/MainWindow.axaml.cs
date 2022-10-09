using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace PreviewVersion.Views
{
    public partial class MainWindow : Window
    {
        public Popup DragDropIndicatorPopup { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            if(!Design.IsDesignMode)
            {
                DragDropIndicatorPopup = this.FindControl<Popup>("dragDropIndicatorPopup");
            }
        }
    }
}