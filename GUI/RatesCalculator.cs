using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GUI;

public class RatesCalculator
{
    private readonly Valute _rouble = new(-1, "RUB", 1, "Рубль", 1, 1);

    public ObservableCollection<RelativeExchangeRate> RelativeExchangeRates { get; } = new();

    public void UpdateRelativeExchangeRates(Valute selectedValute, ExchangeRates selectedExchangeRates,
        ExchangeRates todayExchangeRates)
    {
        RelativeExchangeRates.Clear();
        if (selectedValute is null)
            return;

        var todayValutes = new List<Valute>() {_rouble};
        if (todayExchangeRates is not null)
            todayValutes.AddRange(todayExchangeRates.Valutes);

        var toRouble = new RelativeExchangeRate(selectedValute, _rouble, todayValutes);
        RelativeExchangeRates.Add(toRouble);
        
        var dollar = selectedExchangeRates?.Valutes?.FirstOrDefault(v => v.NumCode == 840);
        if (dollar is not null)
        {
            var toDollar = new RelativeExchangeRate(selectedValute, dollar, todayValutes);
            RelativeExchangeRates.Add(toDollar);
        }
    }
}