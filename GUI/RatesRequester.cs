using System;
using System.Globalization;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using GUI.Utils;

namespace GUI;

public class RatesRequester : BaseNotifyPropertyChanged
{
    private bool _isLoading;
    private readonly string _link;
    private readonly string _queryParameterName;
    private readonly string _dateParameterFormat;
    private readonly RatesDeserializer _deserializer;

    private readonly HttpClient _client = new();
    private CancellationTokenSource _cts;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetAndRaise(ref _isLoading, value);
    }

    public RatesRequester(string link, string queryParameterName, string dateParameterFormat,
        RatesDeserializer deserializer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(link);
        ArgumentException.ThrowIfNullOrWhiteSpace(queryParameterName);
        ArgumentException.ThrowIfNullOrWhiteSpace(dateParameterFormat);

        _link = link;
        _queryParameterName = queryParameterName;
        _dateParameterFormat = dateParameterFormat;
        _deserializer = deserializer;
    }

    public async Task<ExchangeRates> GetRatesOn(DateOnly date)
    {
        if (IsLoading)
            throw new InvalidOperationException(
                $"{nameof(GetRatesOn)} should not be called while loading is in process");

        IsLoading = true;

        _cts = new CancellationTokenSource();

        var queryParameterValue = date.ToString(_dateParameterFormat, CultureInfo.InvariantCulture);
        var url = _link + $"?{_queryParameterName}={HttpUtility.UrlEncode(queryParameterValue)}";

        try
        {
            using var response = await _client.GetAsync(url, _cts.Token);
            await using var contentStream = await response.Content.ReadAsStreamAsync(_cts.Token);
            return _deserializer.Deserialize(contentStream);
        }
        catch (TaskCanceledException)
        {
            return null;
        }
        finally
        {
            IsLoading = false;
            _cts.Dispose();
            _cts = null;
        }
    }

    public async Task Cancel()
    {
        if (IsLoading && _cts is not null)
        {
            await _cts.CancelAsync();
            return;
        }

        if (!IsLoading)
            throw new InvalidOperationException($"{nameof(Cancel)} should not be called if loading is not in process");

        if (_cts is null)
            throw new NotSupportedException($"{nameof(Cancel)} was called, but {nameof(_cts)} is null");
    }
}