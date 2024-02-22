using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using GUI.Utils;

namespace GUI;

public class MainViewModel : BaseNotifyPropertyChanged
{
    private readonly Dictionary<DateOnly, ExchangeRates> _rates = new();

    private ExchangeRates TodayExchangeRates =>
        _rates.TryGetValue(DateOnly.FromDateTime(DateTime.Now), out var rates) ? rates : null;

    private string _searchString;
    private Valute _selectedValute;
    private ICollectionView _valutesCollectionView;

    private DateTime _selectedDate;
    private ExchangeRates _selectedRates;

    public static DateTime NextDay { get; } = DateTime.Now.AddDays(1);

    public RatesRequester RatesRequester { get; }
    public RatesCalculator Calculator { get; }
    public ValuteConverter ValuteConverter { get; }

    public ObservableCollection<ExchangeRates> Rates { get; }

    public ICollectionView ValutesCollectionView
    {
        get => _valutesCollectionView;
        private set
        {
            SetAndRaise(ref _valutesCollectionView, value);
            SearchString = null;
        }
    }

    public ExchangeRates SelectedRates
    {
        get => _selectedRates;
        set
        {
            SetAndRaise(ref _selectedRates, value);
            ValutesCollectionView = SelectedRates is not null
                ? new ListCollectionView(SelectedRates.Valutes.ToIList())
                : null;
        }
    }

    public Valute SelectedValute
    {
        get => _selectedValute;
        set
        {
            SetAndRaise(ref _selectedValute, value);
            Calculator.UpdateRelativeExchangeRates(_selectedValute, SelectedRates, TodayExchangeRates);
        }
    }

    public DateTime SelectedDate
    {
        get => _selectedDate;
        set
        {
            SetAndRaise(ref _selectedDate, value);
            var dateOnly = DateOnly.FromDateTime(_selectedDate);
            SelectedRates = _rates.TryGetValue(dateOnly, out var rates) ? rates : null;
        }
    }

    public string SearchString
    {
        get => _searchString;
        set => SetAndRaise(ref _searchString, value);
    }
    
    public MainViewModel(RatesRequester ratesRequester, RatesCalculator calculator, ValuteConverter valuteConverter)
    {
        RatesRequester = ratesRequester;
        Calculator = calculator;
        ValuteConverter = valuteConverter;

        Rates = new ObservableCollection<ExchangeRates>();
        Rates.CollectionChanged += OnRatesCollectionChanged;

        // Requesting today's rates on startup
        SelectedDate = DateTime.Now;
        MainCommands.RequestRatesCommand.Execute(this);
    }

    private void OnRatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var newItem = e.NewItems!.OfType<ExchangeRates>().Single();
                _rates.Add(newItem.Date, newItem);
                break;

            case NotifyCollectionChangedAction.Remove:
                var oldItem = e.OldItems!.OfType<ExchangeRates>().Single();
                _rates.Remove(oldItem.Date);
                break;

            case NotifyCollectionChangedAction.Reset:
                _rates.Clear();
                break;

            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Move:
            default:
                throw new NotSupportedException($"{e.Action} is not expected on {nameof(Rates)}");
        }
    }
}