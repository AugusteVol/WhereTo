using Microsoft.AspNetCore.Mvc;

namespace WhereTo.Api.Controllers;

[ApiController]
[Route("api/health")]
[Produces("application/json")]
public class HealthController : ControllerBase
{
    /// <summary>Patikrina, ar API veikia.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new { status = "ok" });
}
