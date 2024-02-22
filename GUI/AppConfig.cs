using System.Text.Json.Serialization;

namespace GUI;

public class AppConfig
{
    public string Link { get; }
    public string QueryParameterName { get; }
    public string DateParameterFormat { get; }

    [JsonConstructor]
    public AppConfig(string link, string queryParameterName, string dateParameterFormat)
    {
        Link = link;
        QueryParameterName = queryParameterName;
        DateParameterFormat = dateParameterFormat;
    }
}