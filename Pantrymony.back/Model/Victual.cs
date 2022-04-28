using System.Globalization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace Pantrymony.back.Model;


public class DateTimeConverter : IPropertyConverter
{
    public DynamoDBEntry ToEntry(object? value)
    {
        return value is not DateTime valueAsDate
            ? new DynamoDBNull()
            : valueAsDate.ToString(CultureInfo.InvariantCulture);
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
        return value is not Guid valueAsDate
            ? new DynamoDBNull()
            : valueAsDate.ToString();
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
        return value is not Unit valueAsDate
            ? new DynamoDBNull()
            : valueAsDate.Name;
    }

    public object FromEntry(DynamoDBEntry entry)
    {
        return new Unit(entry.AsString());
    }
}

[DynamoDBTable("Victuals-dev")]
public class Victual
{
    [DynamoDBRangeKey("VictualId", typeof(GuidConverter))]
    public Guid Identifier { get; set; }
    
    [DynamoDBHashKey("UserId")]
    public string UserId { get; set; }
    
    [DynamoDBProperty("Name")]
    public string Name { get; set; }
    
    [DynamoDBProperty("ImageUrl")]
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
    public Unit Unit { get; set; }
}