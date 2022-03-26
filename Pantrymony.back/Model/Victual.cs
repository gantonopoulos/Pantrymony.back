namespace Pantrymony.back.Model;

public class Victual
{
    public Guid Identifier { get; set; }
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public ushort Quantity { get; set; }
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }
    public double Carbs { get; set; }
    
    public Unit Unit { get; set; }
}