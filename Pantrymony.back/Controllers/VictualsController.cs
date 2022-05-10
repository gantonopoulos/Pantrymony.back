using System.ComponentModel.DataAnnotations;
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
    
    [HttpGet("/victuals")]
    public ActionResult<IEnumerable<Victual>> Get()
    {
        return Ok(src.ApiFunctions.GetVictuals());
    }

    [HttpGet("/uservictuals")]
    public ActionResult<Victual?> GetVictualsBy([FromQuery] string userId)
    {
        var victual =  DataSource.Data.Where(entry => entry.UserId == userId);
        return !victual.Any() ? NotFound() : Ok(victual);
    }

    [HttpGet("/uservictual")]
    public ActionResult<Victual?> GetVictualBy(
        [FromQuery] string userId,
        [FromQuery] Guid victualId)
    {
        var victual = DataSource.Data.FirstOrDefault(entry => entry.UserId == userId && 
                                                              entry.VictualId == victualId);
        return victual == default(Victual) ? NotFound() : Ok(victual);
    }

    [HttpPut("/updatevictual")]
    public IActionResult PutVictual(
        [FromQuery] string userId,
        [FromQuery] Guid victualId, 
        Victual victual)
    {
        if (userId != victual.UserId || victualId != victual.VictualId)
        {
            return BadRequest();
        }

        if (!DataSource.Data.Any(entry=> entry.UserId == userId && entry.VictualId == victualId))
        {
            return NotFound(victual);
        }

        DataSource.Data.RemoveAt(DataSource.Data.FindIndex(entry => entry.UserId == userId && 
                                                                    entry.VictualId == victualId));
        DataSource.Data.Add(victual);

        return NoContent();
    }
    
    [HttpPost("/createvictual")]
    public ActionResult<Victual> PostVictual(Victual victual)
    {
        DataSource.Data.Add(victual);
        return CreatedAtAction(nameof(PostVictual), victual);
    }

    [HttpDelete("/deletevictual")]
    public IActionResult DeleteVictual([FromQuery] string userId, [FromQuery] Guid victualId)
    {
        if (!DataSource.Data.Any(entry=> entry.UserId == userId && entry.VictualId == victualId))
        {
            return NotFound();
        }

        DataSource.Data.RemoveAt(DataSource.Data.FindIndex(entry => entry.UserId == userId && entry.VictualId == victualId));

        return NoContent();
    }
}