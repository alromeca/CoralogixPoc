using CoralogixCoreSDK;
using CoralogixPoc.Common.Extensions;
using CoralogixPoc.Data.Respositories;
using CoralogixPoc.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace CoralogixPoc.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class SystemMessagesController : ControllerBase
{
    private readonly ILogger<SystemMessagesController> _logger;
    private readonly CompanyRepository _companyRepository;

    public SystemMessagesController(ILogger<SystemMessagesController> logger, CompanyRepository companyRepository)
    {
        _logger = logger;
        _companyRepository = companyRepository;
    }

    [HttpPost]
    public ActionResult AddSystemLogToCoralogixWithNestedScopes(
        Severity severity, 
        EntityType entityType, 
        string entityId, 
        string externalEntityId,
        AccountingSystem targetAccountingSystem,
        QuickBooksErrorCode quickBooksErrorCode,
        CompanyName companyName)
    {
        var errorInfo = QuickBooksErrorRepository.TryGetErrorInfo(quickBooksErrorCode, out var errorDetails);

        var random = new Random();

        var correlationId = Guid.NewGuid().ToString();

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            var logMessage = new
            {
                EntityType = entityType.GetDisplayName(),
                EntityId = entityId,
                ExternalEntityId = externalEntityId,
                ClassName = nameof(SystemMessagesController),
                MethodName = nameof(AddSystemLogToCoralogixWithNestedScopes),
                CompanyId = _companyRepository.GetCompanyId(companyName),
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
