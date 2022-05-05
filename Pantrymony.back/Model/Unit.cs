namespace Pantrymony.back.Model;

public class Unit
{
    public Unit(string name, string symbol)
    {
        Name = name;
        Symbol = symbol;
    }
    
    public string Name { get; }
    
    public string Symbol { get; }

    public static readonly Unit Kilograms = new("kg", "kilograms");
    public static readonly Unit Grams = new("g", "grams");
    public static readonly Unit Liters = new("L", "Liters");
    public static readonly Unit Milliliters = new("mL", "milliliters");

    public static readonly IEnumerable<Unit> SupportedUnits = new List<Unit>()
    {
        Kilograms, Grams, Liters, Milliliters
    };
}