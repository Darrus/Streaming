using Microsoft.AspNetCore.Mvc;

namespace Streaming.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController() : ControllerBase {
}