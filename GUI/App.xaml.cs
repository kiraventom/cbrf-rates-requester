using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace GUI;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        const string configFileName = "config.json";
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        AppConfig appConfig;
        try
        {
            using var stream = File.OpenRead(configFileName);
            appConfig = JsonSerializer.Deserialize<AppConfig>(stream);
            if (appConfig is null)
                throw new NotSupportedException($"{nameof(appConfig)} was deserialized as null");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error occured while reading config!\n\n{ex}");
            Environment.Exit(1);
            return;
        }

        var ratesDeserializer = new RatesDeserializer();
        var ratesRequester = new RatesRequester(appConfig.Link, appConfig.QueryParameterName,
            appConfig.DateParameterFormat, ratesDeserializer);
        var valuteConverter = new ValuteConverter();
        var ratesCalculator = new RatesCalculator();
            
        var mainViewModel = new MainViewModel(ratesRequester, ratesCalculator, valuteConverter);
        var mainView = new MainView()
        {
            DataContext = mainViewModel
        };

        mainView.Show();
    }
}