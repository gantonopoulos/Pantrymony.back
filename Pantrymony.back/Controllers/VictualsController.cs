using Microsoft.AspNetCore.Mvc;
using Pantrymony.back.Model;

namespace Pantrymony.back.Controllers;

[ApiController]
[Route("[controller]")]
public class VictualsController: ControllerBase
{
    private ILogger<VictualsController> _logger;

    private List<Victual> db = new()
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

    public VictualsController(ILogger<VictualsController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet(Name = "GetVictuals")]
    public ActionResult<IEnumerable<Victual>> Get()
    {
        return db.ToArray();
    }

    [HttpGet("title")]
    public ActionResult<string> GetTitle()
    {
        return new JsonResult("Success");
    }


    [HttpGet("{email}")]
    public IEnumerable<Victual> GetUserVictuals(string email = "")
    {
        return db.Skip(2);
    }
    
}