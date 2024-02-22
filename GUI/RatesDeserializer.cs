using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace GUI;

public class RatesDeserializer
{
    [Serializable]
    [XmlRoot("ValCurs")]
    public sealed class ValCursDto
    {
        [XmlAttribute("Date")] public string Date { get; set; }

        [XmlElement("Valute", IsNullable = true)]
        public ValuteDto[] Valutes { get; set; }

        public static ExchangeRates ToExchangeRates(ValCursDto valCursDto)
        {
            if (!DateOnly.TryParse(valCursDto.Date, out var date))
                date = DateOnly.MinValue;

            var valutes = new List<Valute>();
            if (valCursDto.Valutes is not null)
                valutes.AddRange(valCursDto.Valutes.Select(ValuteDto.ToValute));

            return new ExchangeRates(date, valutes);
        }
    }

    [Serializable]
    [XmlRoot("Valute")]
    public sealed class ValuteDto
    {
        private static readonly CultureInfo RuCulture = CultureInfo.GetCultureInfo("ru");

        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public string Nominal { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string VunitRate { get; set; }

        public static Valute ToValute(ValuteDto valuteDto)
        {
            if (!int.TryParse(valuteDto.NumCode, out var numCode))
                numCode = -1;

            if (!int.TryParse(valuteDto.Nominal, out var nominal))
                nominal = -1;

            if (!decimal.TryParse(valuteDto.Value, RuCulture, out var value))
                value = -1;

            if (!decimal.TryParse(valuteDto.VunitRate, RuCulture, out var vUnitRate))
                vUnitRate = -1;

            return new Valute(numCode, valuteDto.CharCode, nominal, valuteDto.Name, value, vUnitRate);
        }
    }

    private readonly XmlSerializer _serializer = new(typeof(ValCursDto));

    public ExchangeRates Deserialize(Stream stream)
    {
        var deserialized = _serializer.Deserialize(stream);
        if (deserialized is ValCursDto valCursDto)
            return ValCursDto.ToExchangeRates(valCursDto);

        return null;
    }
}