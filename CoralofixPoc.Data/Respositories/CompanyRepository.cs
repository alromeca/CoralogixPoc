using CoralogixPoc.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CoralogixPoc.Data.Respositories;

public class CompanyRepository
{
    private readonly ILogger<CompanyRepository> _logger;
    private readonly Dictionary<CompanyName, Guid> _companies = new()
    {
        { CompanyName.AcmeCorp, Guid.Parse("1e72b35a-3f60-4f33-be4b-ec7e3c84ef4a") },
        { CompanyName.GlobexInc, Guid.Parse("e393350b-b411-4ea1-b046-8fa4fae2435c") },
        { CompanyName.Initech, Guid.Parse("8e32c7ea-8c24-435d-a239-4ae56c5eac34") },
        { CompanyName.UmbrellaLLC, Guid.Parse("17a983ab-e858-46d5-8fce-d97be400096a") },
        { CompanyName.StarkIndustries, Guid.Parse("1fc48c3d-29a8-416f-b7da-ae4977df602c") }
    };

    public CompanyRepository(ILogger<CompanyRepository> logger)
    {
        _logger = logger;
    }

    public Guid GetCompanyId(CompanyName companyName)
    {
        using (_logger.BeginScope(new Dictionary<string, object> { ["CompanyName"] = companyName }))
        {
            _logger.LogInformation("Getting company ID for {CompanyName}", companyName);

            if (_companies.TryGetValue(companyName, out var companyId))
            {
                return companyId;
            }
            throw new ArgumentException($"Company '{companyName}' not found.", nameof(companyName));
        }
    }

    public CompanyName[] GetAllCompanyNames()
    {
        _logger.LogInformation("Retrieving all company names.");
        return [.. _companies.Keys];
    }
}