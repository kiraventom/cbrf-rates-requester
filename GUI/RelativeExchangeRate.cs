using System;
using System.Collections.Generic;
using System.Linq;
using GUI.Utils;

namespace GUI;

public class RelativeExchangeRate : BaseNotifyPropertyChanged
{
    public Valute RelativeTo { get; }
    private Valute Valute { get; }
    private IReadOnlyCollection<Valute> TodayValutes { get; }

    public decimal Rate => Valute.VUnitRate / RelativeTo.VUnitRate;

    public decimal Difference
    {
        get
        {
            if (TodayValutes is null)
                return 0;

            var todayValute = TodayValutes.FirstOrDefault(v => v.NumCode == Valute.NumCode);
            if (todayValute is null)
                return 0;

            var todayRelativeToValute = TodayValutes.FirstOrDefault(v => v.NumCode == RelativeTo.NumCode);
            if (todayRelativeToValute is null)
                return 0;

            var todayRate = todayValute.VUnitRate / todayRelativeToValute.VUnitRate;
            return Rate - todayRate;
        }
    }

    public RelativeExchangeRate(Valute valute, Valute relativeTo, IReadOnlyCollection<Valute> todayValutes)
    {
        ArgumentNullException.ThrowIfNull(valute);
        ArgumentNullException.ThrowIfNull(relativeTo);

        Valute = valute;
        RelativeTo = relativeTo;
        TodayValutes = todayValutes;
    }
}