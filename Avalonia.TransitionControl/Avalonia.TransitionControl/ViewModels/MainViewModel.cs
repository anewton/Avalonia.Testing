using System.ComponentModel;

namespace Avalonia.TransitionControl.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public object SelectedValue
    {
        get => _selectedValue;
        set
        {
            _selectedValue = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedValue)));
        }
    }
    private object _selectedValue;

    public void SetSelectedValue(string parameter)
    {
        SelectedValue = parameter;
    }

    public MainViewModel()
    {
        SelectedValue = "Red";
    }

}
