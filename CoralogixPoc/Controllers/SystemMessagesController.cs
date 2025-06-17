using CoralogixCoreSDK;
using CoralogixPoc.Enums;
using CoralogixPoc.Extensions;
using CoralogixPoc.Respositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

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
        AccountingSystem targetAccountingSystem,
        QuickBooksErrorCode quickBooksErrorCode,
        CompanyName companyName)
    {
        var errorInfo = QuickBooksErrorRepository.TryGetErrorInfo(quickBooksErrorCode, out var errorDetails);
        var companyId = CompanyRepository.GetCompanyId(companyName);

        var random = new Random();

        var correlationId = Guid.NewGuid().ToString();

        using (_logger.BeginScope(new { CorrelationId = correlationId }))
        {
            var logMessage = new
            {
                EntityType = entityType.GetDisplayName(),
                EntityId = entityId,
                ExternalEntityId = externalEntityId,
                ClassName = nameof(SystemMessagesController),
                MethodName = nameof(AddSystemLogToCoralogix),
                CompanyId = companyId,
                CompanyName = companyName,
                Target = targetAccountingSystem.GetDisplayName(),
                QuickBooksErrorCode = quickBooksErrorCode.GetDisplayName(),
                errorDetails.RootCause,
                errorDetails.RecoveryPath
            };

            severity.LogBySeverity(_logger, logMessage);
        }        

        return Ok();
    }
}
