using CoralogixCoreSDK;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DiagnosticImagingSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class CoralogixController(ILogger<CoralogixController> logger) : ControllerBase
{
    private readonly ILogger<CoralogixController> _logger = logger;

    [HttpPost]
    public ActionResult AddLogToCoralogix(Severity severity, string message)
    {
        switch (severity)
        {
            case Severity.Debug:
                _logger.LogDebug(message);
                break;
            case Severity.Verbose:
                _logger.LogTrace(message);
                break;
            case Severity.Info:
                _logger.LogInformation(message);
                break;
            case Severity.Warning:
                _logger.LogWarning(message);
                break;
            case Severity.Error:
                _logger.LogError(message);
                break;
            case Severity.Critical:
                _logger.LogCritical(message);
                break;
            default:
                break;
        }

        return Ok();
    }
}
