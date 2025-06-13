using CoralogixCoreSDK;
using CoralogixPoc.Enums;
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
    private readonly Guid[] _companyIds =
    {
        Guid.Parse("1e72b35a-3f60-4f33-be4b-ec7e3c84ef4a"),
        Guid.Parse("e393350b-b411-4ea1-b046-8fa4fae2435c"),
        Guid.Parse("8e32c7ea-8c24-435d-a239-4ae56c5eac34"),
        Guid.Parse("17a983ab-e858-46d5-8fce-d97be400096a"),
        Guid.Parse("1fc48c3d-29a8-416f-b7da-ae4977df602c")
    };

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
            var message = $"Random log message #{++logCount}";

            var errorInfo = QuickBooksErrorRepository.TryGetErrorInfo(quickBooksErrorCode, out var errorDetails);

            var correlationId = Guid.NewGuid().ToString();

            using (_logger.BeginScope(new { CorrelationId = correlationId }))
            {
                var logMessage = new
                {
                    EntityType = entityType.GetDisplayName(),
                    EntityId = entityId,
                    ExternalEntityId = externalEntityId,
                    Message = message,
                    ClassName = nameof(GenerateRandomDataController),
                    MethodName = nameof(GenerateRandomData),
                    CompanyId = _companyIds[random.Next(_companyIds.Length)],
                    Target = accountingSystem.GetDisplayName(),
                    QuickBooksErrorCode = quickBooksErrorCode.GetDisplayName(),
                    RootCause = errorDetails.RootCause,
                    RecoveryPath = errorDetails.RecoveryPath
                };

                switch (severity)
                {
                    case Severity.Debug:
                        _logger.LogDebug("{@LogMessage}", logMessage);
                        break;
                    case Severity.Verbose:
                        _logger.LogTrace("{@LogMessage}", logMessage);
                        break;
                    case Severity.Info:
                        _logger.LogInformation("{@LogMessage}", logMessage);
                        break;
                    case Severity.Warning:
                        _logger.LogWarning("{@LogMessage}", logMessage);
                        break;
                    case Severity.Error:
                        _logger.LogError("{@LogMessage}", logMessage);
                        break;
                    case Severity.Critical:
                        _logger.LogCritical("{@LogMessage}", logMessage);
                        break;
                    default:
                        break;
                }
            }

            Thread.Sleep(TimeSpan.FromSeconds(15));
        }

        return Ok($"Logging completed. Total logs sent: {logCount}");
    }
}