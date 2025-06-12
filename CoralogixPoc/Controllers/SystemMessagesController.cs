using CoralogixCoreSDK;
using CoralogixPoc.Enums;
using CoralogixPoc.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Extensions;
using System.Text.Json;

namespace DiagnosticImagingSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class SystemMessagesController(ILogger<SystemMessagesController> logger) : ControllerBase
{
    private readonly ILogger<SystemMessagesController> _logger = logger;

    [HttpPost]
    public ActionResult AddSystemLogToCoralogix(
        Severity severity, 
        EntityType entityType, 
        string entityId, 
        string externalEntityId,
        string message)
    {
        var errorMessage = new
        {
            EntityType = entityType.GetDisplayName(),
            EntityId = entityId,
            ExternalEntityId = externalEntityId,
            Message = message,
            ClassName = nameof(SystemMessagesController),
            MethodName = nameof(AddSystemLogToCoralogix)
        };

        var logMessage = $"Processing the message: {JsonSerializer.Serialize(errorMessage)}";

        switch (severity)
        {
            case Severity.Debug:
                _logger.LogDebug(logMessage);
                break;
            case Severity.Verbose:
                _logger.LogTrace(logMessage);
                break;
            case Severity.Info:
                _logger.LogInformation(logMessage);
                break;
            case Severity.Warning:
                _logger.LogWarning(logMessage);
                break;
            case Severity.Error:
                _logger.LogError(logMessage);
                break;
            case Severity.Critical:
                _logger.LogCritical(logMessage);
                break;
            default:
                break;
        }

        return Ok();
    }
}
