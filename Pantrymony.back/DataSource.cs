using Microsoft.VisualBasic;
using Pantrymony.back.Model;

namespace Pantrymony.back;

public static class DataSource
{
     public static List<Victual> Data = new()
        {
             new Victual{
                 UserId = "georanto@gmail.com",
                 Identifier = Guid.NewGuid(), 
                 Name= "Fakes",
                 ImageUrl = "", 
                 Calories = 45, 
                 Carbs = 45, 
                 Fat = 3, 
                 Protein = 52, 
                 Quantity = 100,
                 Unit = Unit.Kilograms,
                 Expires = DateTime.Parse("2022-05-04")
             },
             new Victual{
                 UserId = "georanto@gmail.com",
                 Identifier = Guid.NewGuid(), 
                 Name= "Cucumbers",
                 ImageUrl = "", 
                 Calories = 13, 
                 Carbs = 2, 
                 Fat = 1, 
                 Protein = 5, 
                 Quantity = 40,
                 Unit = Unit.Grams,
                 Expires = DateTime.Parse("2021-01-02")
             },
             
             new Victual{
                 UserId = "georanto@gmail.com",
                 Identifier = Guid.NewGuid(), 
                 Name= "Onions",
                 ImageUrl = "", 
                 Calories = 27, 
                 Carbs = 18, 
                 Fat = 1, 
                 Protein = 6, 
                 Quantity = 230,
                 Unit = Unit.Kilograms,
                 Expires = DateTime.Parse("2022-02-12")
             },
             
             new Victual{
                 UserId = "georanto@gmail.com",
                 Identifier = Guid.NewGuid(), 
                 Name= "Meat",
                 ImageUrl = "", 
                 Calories = 450, 
                 Carbs = 2, 
                 Fat = 15, 
                 Protein = 50, 
                 Quantity = 570,
                 Unit = Unit.Kilograms,
                 Expires = DateTime.Parse("2022-02-13")
             },
             new Victual{
                 UserId = "georgios.antonopoulos@philips.com",
                 Identifier = Guid.NewGuid(), 
                 Name= "Chicken",
                 ImageUrl = "", 
                 Calories = 250, 
                 Carbs = 2, 
                 Fat = 1, 
                 Protein = 40, 
                 Quantity = 1000,
                 Unit = Unit.Grams,
                 Expires = DateTime.Parse("2022-05-02")
             },
             new Victual{
                 UserId = "georgios.antonopoulos@philips.com",
                 Identifier = Guid.NewGuid(), 
                 Name= "Lamb",
                 ImageUrl = "", 
                 Calories = 550, 
                 Carbs = 2, 
                 Fat = 35, 
                 Protein = 450, 
                 Quantity = 750,
                 Unit = Unit.Grams,
                 Expires = DateTime.Parse("2022-01-05")
             },
        };
}