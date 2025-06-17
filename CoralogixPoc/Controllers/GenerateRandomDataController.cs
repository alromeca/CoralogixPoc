using CoralogixCoreSDK;
using CoralogixPoc.Enums;
using CoralogixPoc.Extensions;
using CoralogixPoc.Respositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Text.Json;

namespace CoralogixPoc.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class GenerateRandomDataController(ILogger<GenerateRandomDataController> logger) : ControllerBase
{
    private readonly ILogger<GenerateRandomDataController> _logger = logger;
    private readonly CompanyName[] _companyNames = CompanyRepository.GetAllCompanyNames();

    [HttpPost]
    public ActionResult GenerateRandomData()
    {
        var random = new Random();
        var startTime = DateTime.UtcNow;
        var oneHour = TimeSpan.FromHours(1);

        var severities = Enum.GetValues<Severity>();
        var accountingSystems = Enum.GetValues<AccountingSystem>();
        var quickBooksErrorCodes = Enum.GetValues<QuickBooksErrorCode>();
        var entityTypes = Enum.GetValues<EntityType>();

        int logCount = 0;

        while (DateTime.UtcNow - startTime < oneHour)
        {
            var severity = severities[random.Next(severities.Length)];
            var accountingSystem = accountingSystems[random.Next(accountingSystems.Length)];
            var quickBooksErrorCode = quickBooksErrorCodes[random.Next(quickBooksErrorCodes.Length)];
            var entityType = entityTypes[random.Next(entityTypes.Length)];
            var entityId = random.Next(1000, 9999).ToString();
            var externalEntityId = Guid.NewGuid().ToString();

            var errorInfo = QuickBooksErrorRepository.TryGetErrorInfo(quickBooksErrorCode, out var errorDetails);

            var correlationId = Guid.NewGuid().ToString();

            var randomCompanyName = _companyNames[random.Next(_companyNames.Length)];

            using (_logger.BeginScope(new { CorrelationId = correlationId }))
            {
                var logMessage = new
                {
                    EntityType = entityType.GetDisplayName(),
                    EntityId = entityId,
                    ExternalEntityId = externalEntityId,
                    ClassName = nameof(GenerateRandomDataController),
                    MethodName = nameof(GenerateRandomData),
                    CompanyId = CompanyRepository.GetCompanyId(randomCompanyName),
                    CompanyName = randomCompanyName,
                    Target = accountingSystem.GetDisplayName(),
                    QuickBooksErrorCode = quickBooksErrorCode.GetDisplayName(),
                    errorDetails.RootCause,
                    errorDetails.RecoveryPath
                };

                severity.LogBySeverity(_logger, logMessage);
            }

            Thread.Sleep(TimeSpan.FromSeconds(15));
        }

        return Ok($"Logging completed. Total logs sent: {logCount}");
    }
}