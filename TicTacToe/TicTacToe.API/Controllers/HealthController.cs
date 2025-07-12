using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult CheckHealth() => Ok();
}