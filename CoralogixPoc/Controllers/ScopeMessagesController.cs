using CoralogixPoc.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace CoralogixPoc.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ScopeMessagesController(ILogger<ScopeMessagesController> logger) : ControllerBase
{
    private readonly ILogger<ScopeMessagesController> _logger = logger;

    [HttpPost]
    [Route("SingleScope")]
    public ActionResult AddScopeLogToCoralogix()
    {
        var firstMessage = new
        {
            EntityType = EntityType.Customer.GetDisplayName(),
            EntityId = 123,
            ClassName = nameof(ScopeMessagesController),
            MethodName = nameof(AddScopeLogToCoralogix),
        };

        var secondMessage = new
        {
            EntityType = EntityType.Invoice.GetDisplayName(),
            EntityId = 453,
            ClassName = nameof(ScopeMessagesController),
            MethodName = nameof(AddScopeLogToCoralogix),
        };

        var correlationId = Guid.NewGuid().ToString();

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            //Processing Customer:
            _logger.LogWarning("{@LogMessage}", firstMessage);

            //Processing Invoice:
            _logger.LogWarning("{@LogMessage}", secondMessage);
        }

        return Ok();
    }

    [HttpPost]
    [Route("MultipleScopes")]
    public ActionResult AddMultipleScopeLogsToCoralogix()
    {
        var firstMessage = new
        {
            EntityType = EntityType.Customer.GetDisplayName(),
            EntityId = 123,
            ClassName = nameof(ScopeMessagesController),
            MethodName = nameof(AddMultipleScopeLogsToCoralogix),
        };

        var secondMessage = new
        {
            EntityType = EntityType.Invoice.GetDisplayName(),
            EntityId = 453,
            ClassName = nameof(ScopeMessagesController),
            MethodName = nameof(AddMultipleScopeLogsToCoralogix),
        };

        var firstCorrelationId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = firstCorrelationId, ["CompanyName"] = "First Company Name" }))
        {
            //Processing Customer:
            _logger.LogWarning("{@LogMessage}", firstMessage);
        }

        var secondCorrelationId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = secondCorrelationId, ["CompanyName"] = "Second Company Name" }))
        {
            //Processing Invoice:
            _logger.LogWarning("{@LogMessage}", secondMessage);
        }

        return Ok();
    }
}
