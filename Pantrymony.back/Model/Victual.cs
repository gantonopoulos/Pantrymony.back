using System.Globalization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace Pantrymony.back.Model;


public class DateTimeConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object? value)
    {
        return value switch
        {
            string dateAsString => dateAsString,
            DateTime valueAsDate => valueAsDate.ToString(CultureInfo.InvariantCulture),
            _ => new DynamoDBNull()
        };
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        return DateTime.Parse(entry.AsString());
    }
}

public class GuidConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object? value)
    {
        return value switch
        {
            string guidStr => guidStr,
            Guid valueAsGuid => valueAsGuid.ToString(),
            _ => new DynamoDBNull()
        };
    }


    public object FromEntry(DynamoDBEntry entry)
    {
        return entry.AsGuid();
    }
}

public class UnitConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object? value)
    {
        if(value is not string unitSymbol ||
           !Unit.SupportedUnits.Any(unit => unit.Symbol.Equals(unitSymbol)))
            return new DynamoDBNull();
        return unitSymbol;
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        var applicableUnit = Unit.SupportedUnits.SingleOrDefault(unit => unit.Symbol.Equals(entry.AsString()));
        if (applicableUnit == null)
            throw new ArgumentOutOfRangeException($"No unit with Symbol:[{entry.AsString()}] exists!");
        return applicableUnit.Symbol;
    }
}

[DynamoDBTable("Victuals-dev")]
public class Victual
{
    [DynamoDBRangeKey("VictualId", typeof(GuidConverter))]
    public Guid VictualId { get; set; }
    
    [DynamoDBHashKey("UserId")]
    public string UserId { get; set; }
    
    [DynamoDBProperty("Name")]
    public string Name { get; set; }
    
    [DynamoDBIgnore]
    public string ImageUrl { get; set; }
    
    [DynamoDBProperty("Quantity")]
    public ushort Quantity { get; set; }
    
    [DynamoDBProperty("Calories")]
    public double Calories { get; set; }
    
    [DynamoDBProperty("Protein")]
    public double Protein { get; set; }
    
    [DynamoDBProperty("Fat")]
    public double Fat { get; set; }
    
    [DynamoDBProperty("Carbs")]
    public double Carbs { get; set; }
    
    [DynamoDBProperty("Expires", typeof(DateTimeConverter))]
    [DynamoDBLocalSecondaryIndexRangeKey("Expiration_Index")]
    public DateTime Expires { get; set; }
    
    [DynamoDBProperty("Unit", typeof(UnitConverter))]
    public string Unit { get; set; }
}