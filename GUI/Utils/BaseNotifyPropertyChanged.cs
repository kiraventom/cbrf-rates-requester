using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GUI.Utils;

public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected bool SetAndRaise<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (Equals(field, value))
            return false;
            
        field = value;
        RaisePropertyChanged(propertyName);
        return true;
    }

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}