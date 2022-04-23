using Microsoft.AspNetCore.Mvc;
using Pantrymony.back.Model;

namespace Pantrymony.back.Controllers;

[ApiController]
[Route("[controller]")]
public class VictualsController: ControllerBase
{
    private ILogger<VictualsController> _logger;

    public VictualsController(ILogger<VictualsController> logger)
    {
        _logger = logger;
    }
    
    [HttpGet(Name = "GetVictuals")]
    public ActionResult<IEnumerable<Victual>> Get()
    {
        return Ok(src.ApiFunctions.GetVictuals());
    }

    [HttpGet("{id}")]
    public ActionResult<Victual?> GetVictualBy([FromRoute (Name = "id")] Guid id)
    {
        var victual =  DataSource.Data.FirstOrDefault(entry => entry.Identifier == id);
        return victual == default(Victual) ? NotFound() : Ok(victual);
    }

    [HttpPut("{id}")]
    public IActionResult PutTodoItem(Guid id, Victual victual)
    {
        if (id != victual.Identifier)
        {
            return BadRequest();
        }

        if (DataSource.Data.All(v => v.Identifier != id))
        {
            return NotFound(victual);
        }

        DataSource.Data.RemoveAt(DataSource.Data.FindIndex(v => v.Identifier == id));
        DataSource.Data.Add(victual);

        return NoContent();
    }
    
    [HttpPost]
    public ActionResult<Victual> PostVictual(Victual victual)
    {
        DataSource.Data.Add(victual);
        return CreatedAtAction(nameof(GetVictualBy), new { id = victual.Identifier }, victual);
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteVictual(Guid id)
    {
        if (DataSource.Data.All(v => v.Identifier != id))
        {
            return NotFound();
        }

        DataSource.Data.RemoveAt(DataSource.Data.FindIndex(v => v.Identifier == id));

        return NoContent();
    }
}