using System;
using System.Linq;
using System.Windows;
using GUI.Utils;

namespace GUI;

public static class MainCommands
{
    public static SimpleWpfCommand<MainViewModel> RequestRatesCommand { get; } =
        new(ExecuteRequestRates, CanExecuteRequestRates);

    public static SimpleWpfCommand<RatesRequester> CancelRequestCommand { get; } =
        new(ExecuteCancelRequest, CanExecuteCancelRequest);

    public static SimpleWpfCommand<MainViewModel> SearchCommand { get; } =
        new(ExecuteSearch, CanExecuteSearch);

    private static async void ExecuteRequestRates(MainViewModel mainViewModel)
    {
        var date = DateOnly.FromDateTime(mainViewModel.SelectedDate);
        var exchangeRates = await mainViewModel.RatesRequester.GetRatesOn(date);
        if (exchangeRates is null)
            return;

        // There is no info on requested date (it's future or too far in the past)
        if (exchangeRates.Date != date)
        {
            MessageBox.Show("Нет информации по запрошенной дате", "Некорректная дата", MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        mainViewModel.Rates.Add(exchangeRates);
        mainViewModel.SelectedRates = exchangeRates;

        RequestRatesCommand.RaiseCanExecuteChanged();
    }

    private static bool CanExecuteRequestRates(MainViewModel mainViewModel)
    {
        if (mainViewModel.RatesRequester.IsLoading)
            return false;

        var date = DateOnly.FromDateTime(mainViewModel.SelectedDate);
        return mainViewModel.Rates.All(r => r.Date != date);
    }

    private static async void ExecuteCancelRequest(RatesRequester ratesRequester) =>
        await ratesRequester.Cancel();

    private static bool CanExecuteCancelRequest(RatesRequester ratesRequester) => ratesRequester.IsLoading;

    private static void ExecuteSearch(MainViewModel mainViewModel)
    {
        var searchString = mainViewModel.SearchString;
        if (string.IsNullOrWhiteSpace(searchString))
        {
            mainViewModel.ValutesCollectionView.Filter = null;
            return;
        }

        var searchPredicate = new Predicate<object>(o =>
            o is Valute v &&
            (v.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
             v.CharCode.Contains(searchString, StringComparison.OrdinalIgnoreCase)));

        mainViewModel.ValutesCollectionView.Filter = searchPredicate;
    }

    private static bool CanExecuteSearch(MainViewModel mainViewModel) =>
        mainViewModel.ValutesCollectionView is not null;
}