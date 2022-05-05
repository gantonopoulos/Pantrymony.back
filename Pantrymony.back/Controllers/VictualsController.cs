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

    [HttpGet("{userId}")]
    public ActionResult<Victual?> GetVictualsBy([FromRoute (Name = "userId")] string userId)
    {
        var victual =  DataSource.Data.Where(entry => entry.UserId == userId);
        return !victual.Any() ? NotFound() : Ok(victual);
    }

    [HttpGet("{userId}/{victualId}")]
    public ActionResult<Victual?> GetVictualBy([FromRoute(Name = "userId")] string userId,
        [FromRoute(Name = "victualId")] Guid victualId)
    {
        var victual = DataSource.Data.FirstOrDefault(entry => entry.UserId == userId && 
                                                              entry.Identifier == victualId);
        return victual == default(Victual) ? NotFound() : Ok(victual);
    }

    [HttpPut("{userId}/{victualId}")]
    public IActionResult PutVictual(
        [FromRoute(Name = "userId")] string userId,
        [FromRoute(Name = "victualId")] Guid victualId, 
        Victual victual)
    {
        if (userId != victual.UserId || victualId != victual.Identifier)
        {
            return BadRequest();
        }

        if (!DataSource.Data.Any(entry=> entry.UserId == userId && entry.Identifier == victualId))
        {
            return NotFound(victual);
        }

        DataSource.Data.RemoveAt(DataSource.Data.FindIndex(entry => entry.UserId == userId && entry.Identifier == victualId));
        DataSource.Data.Add(victual);

        return NoContent();
    }
    
    [HttpPost]
    public ActionResult<Victual> PostVictual(Victual victual)
    {
        DataSource.Data.Add(victual);
        return CreatedAtAction(nameof(PostVictual), victual);
    }

    [HttpDelete("{userId}/{victualId}")]
    public IActionResult DeleteVictual([FromRoute(Name = "userId")] string userId,
        [FromRoute(Name = "victualId")] Guid victualId)
    {
        if (!DataSource.Data.Any(entry=> entry.UserId == userId && entry.Identifier == victualId))
        {
            return NotFound();
        }

        DataSource.Data.RemoveAt(DataSource.Data.FindIndex(entry => entry.UserId == userId && entry.Identifier == victualId));

        return NoContent();
    }
}