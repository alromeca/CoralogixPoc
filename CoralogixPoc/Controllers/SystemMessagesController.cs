using CoralogixCoreSDK;
using CoralogixPoc.Enums;
using CoralogixPoc.Respositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Text.Json;

namespace DiagnosticImagingSystem.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class SystemMessagesController(ILogger<SystemMessagesController> logger) : ControllerBase
{
    private readonly ILogger<SystemMessagesController> _logger = logger;
    private readonly Guid[] _companyIds =
    {
        Guid.Parse("1e72b35a-3f60-4f33-be4b-ec7e3c84ef4a"),
        Guid.Parse("e393350b-b411-4ea1-b046-8fa4fae2435c"),
        Guid.Parse("8e32c7ea-8c24-435d-a239-4ae56c5eac34"),
        Guid.Parse("17a983ab-e858-46d5-8fce-d97be400096a"),
        Guid.Parse("1fc48c3d-29a8-416f-b7da-ae4977df602c")
    };

    [HttpPost]
    public ActionResult AddSystemLogToCoralogix(
        Severity severity, 
        EntityType entityType, 
        string entityId, 
        string externalEntityId,
        AccountingSystem targetAccountingSystem,
        QuickBooksErrorCode quickBooksErrorCode,
        string message)
    {
        var errorInfo = QuickBooksErrorRepository.TryGetErrorInfo(quickBooksErrorCode, out var errorDetails);

        var random = new Random();

        var traceId = Guid.NewGuid().ToString();

        using (_logger.BeginScope(new { TraceId = traceId }))
        {
            var errorMessage = new
            {
                EntityType = entityType.GetDisplayName(),
                EntityId = entityId,
                ExternalEntityId = externalEntityId,
                Message = message,
                ClassName = nameof(SystemMessagesController),
                MethodName = nameof(AddSystemLogToCoralogix),
                CompanyId = _companyIds[random.Next(_companyIds.Length)],
                Target = targetAccountingSystem.GetDisplayName(),
                QuickBooksErrorCode = quickBooksErrorCode.GetDisplayName(),
                RootCause = errorDetails.RootCause,
                RecoveryPath = errorDetails.RecoveryPath
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
        }        

        return Ok();
    }
}
