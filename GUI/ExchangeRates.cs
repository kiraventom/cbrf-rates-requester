using System;
using System.Collections.Generic;

namespace GUI;

public class ExchangeRates
{
    public DateOnly Date { get; }

    public IReadOnlyList<Valute> Valutes { get; }

    public ExchangeRates(DateOnly date, IReadOnlyList<Valute> valutes)
    {
        Date = date;
        Valutes = valutes;
    }
}