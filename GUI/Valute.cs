namespace GUI;

public class Valute
{
    public int NumCode { get; }
    public string CharCode { get; }
    public int Nominal { get; }
    public string Name { get; }
    public decimal Value { get; }
    public decimal VUnitRate { get; }

    public Valute(int numCode, string charCode, int nominal, string name, decimal value, decimal vUnitRate)
    {
        NumCode = numCode;
        CharCode = charCode;
        Nominal = nominal;
        Name = name;
        Value = value;
        VUnitRate = vUnitRate;
    }
}