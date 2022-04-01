using Pantrymony.back.Model;

namespace Pantrymony.back;

public static class DataSource
{
     public static List<Victual> Data = new()
        {
             new Victual{
                 Identifier = Guid.NewGuid(), 
                 Name= "Fakes",
                 ImageUrl = "", 
                 Calories = 45, 
                 Carbs = 45, 
                 Fat = 3, 
                 Protein = 52, 
                 Quantity = 100,
                 Unit = new Unit("Grams")},
             new Victual{
                 Identifier = Guid.NewGuid(), 
                 Name= "Cucumbers",
                 ImageUrl = "", 
                 Calories = 13, 
                 Carbs = 2, 
                 Fat = 1, 
                 Protein = 5, 
                 Quantity = 40,
                 Unit = new Unit("Grams")},
             new Victual{
                 Identifier = Guid.NewGuid(), 
                 Name= "Onions",
                 ImageUrl = "", 
                 Calories = 27, 
                 Carbs = 18, 
                 Fat = 1, 
                 Protein = 6, 
                 Quantity = 230,
                 Unit = new Unit("Grams")},
             new Victual{
                 Identifier = Guid.NewGuid(), 
                 Name= "Meat",
                 ImageUrl = "", 
                 Calories = 450, 
                 Carbs = 2, 
                 Fat = 15, 
                 Protein = 50, 
                 Quantity = 570,
                 Unit = new Unit("Grams")},
        };
}