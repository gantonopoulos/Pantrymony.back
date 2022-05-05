using Microsoft.AspNetCore.Mvc;
using Pantrymony.back.Model;

namespace Pantrymony.back.Controllers;

[ApiController]
[Route("[controller]")]
public class UnitsController : ControllerBase
{
    [HttpGet(Name = "GetUnits")]
    public ActionResult<IEnumerable<Unit>> Get()
    {
        return Ok(Unit.SupportedUnits);
    }
}