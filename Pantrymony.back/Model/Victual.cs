namespace Pantrymony.back.Model;

public record Victual(
    Guid Identifier,
    string Name,
    string ImageUrl,
    ushort Quantity,
    double Calories,
    double Protein,
    double Fat,
    double Carbs,
    Unit Unit
);