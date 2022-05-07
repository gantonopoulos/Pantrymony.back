namespace Pantrymony.back.Model;

public class Unit
{
    public Unit(string symbol, string name)
    {
        Name = name;
        Symbol = symbol;
    }
    
    public string Name { get; }
    
    public string Symbol { get; }

    public static readonly Unit Kilograms = new("kg", "kilograms");
    public static readonly Unit Grams = new("g", "grams");
    public static readonly Unit Liters = new("L", "liters");
    public static readonly Unit Milliliters = new("mL", "milliliters");

    public static readonly IEnumerable<Unit> SupportedUnits = new List<Unit>()
    {
        Kilograms, Grams, Liters, Milliliters
    };
}