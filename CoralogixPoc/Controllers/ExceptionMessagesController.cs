using Microsoft.AspNetCore.Mvc;

namespace CoralogixPoc.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ExceptionMessagesController(ILogger<ExceptionMessagesController> logger) : ControllerBase
{
    private readonly ILogger<ExceptionMessagesController> _logger = logger;

    [HttpPost]
    public ActionResult AddErrorLogToCoralogix()
    {
        try
        {
            throw new InvalidOperationException("This is a test exception for Coralogix logging.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An error occurred while processing the request.");
        }

        return Ok();
    }
}
