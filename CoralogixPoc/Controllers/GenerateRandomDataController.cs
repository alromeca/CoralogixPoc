using CoralogixCoreSDK;
using CoralogixPoc.Common.Extensions;
using CoralogixPoc.Data.Respositories;
using CoralogixPoc.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace CoralogixPoc.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class GenerateRandomDataController : ControllerBase
{
    private readonly ILogger<GenerateRandomDataController> _logger;
    private readonly CompanyRepository _companyRepository;
    private readonly CompanyName[] _companyNames;

    public GenerateRandomDataController(ILogger<GenerateRandomDataController> logger, CompanyRepository companyRepository)
    {
        _logger = logger;
        _companyRepository = companyRepository;
        _companyNames = _companyRepository.GetAllCompanyNames();
    }

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

            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
            {
                var logMessage = new
                {
                    EntityType = entityType.GetDisplayName(),
                    EntityId = entityId,
                    ExternalEntityId = externalEntityId,
                    ClassName = nameof(GenerateRandomDataController),
                    MethodName = nameof(GenerateRandomData),
                    CompanyId = _companyRepository.GetCompanyId(randomCompanyName),
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