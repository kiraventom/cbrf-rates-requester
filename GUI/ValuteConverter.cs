using GUI.Utils;

namespace GUI;

public class ValuteConverter : BaseNotifyPropertyChanged
{
    private Valute _convertTo;
    private Valute _convertFrom;
    private string _convertToValue;
    private string _convertFromValue;

    public Valute ConvertFrom
    {
        get => _convertFrom;
        set
        {
            SetAndRaise(ref _convertFrom, value);
            if (_convertFrom is null)
            {
                _convertFromValue = null;
                RaisePropertyChanged(nameof(ConvertFromValue));
                return;
            }

            Convert();
        }
    }

    public Valute ConvertTo
    {
        get => _convertTo;
        set
        {
            SetAndRaise(ref _convertTo, value);
            if (_convertTo is null)
            {
                _convertToValue = null;
                RaisePropertyChanged(nameof(ConvertToValue));
                return;
            }
            
            ConvertBackwards();
        }
    }

    public string ConvertFromValue
    {
        get => _convertFromValue;
        set
        {
            SetAndRaise(ref _convertFromValue, value);
            Convert();
        }
    }

    public string ConvertToValue
    {
        get => _convertToValue;
        set
        {
            SetAndRaise(ref _convertToValue, value);
            ConvertBackwards();
        }
    }

    private void Convert()
    {
        var converted = Convert(ConvertFrom, ConvertTo, ConvertFromValue);
        _convertToValue = converted;
        RaisePropertyChanged(nameof(ConvertToValue));
    }

    private void ConvertBackwards()
    {
        var converted = Convert(ConvertTo, ConvertFrom, ConvertToValue);
        _convertFromValue = converted;
        RaisePropertyChanged(nameof(ConvertFromValue));
    }

    private static string Convert(Valute from, Valute to, string value)
    {
        if (from is null || to is null)
            return null;

        if (!decimal.TryParse(value, out var amount))
            return null;

        var converted = (from.VUnitRate / to.VUnitRate) * amount;
        return converted.ToString("0.###");
    }
}