using CoralogixPoc.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace CoralogixPoc.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ScopeMessagesController(ILogger<ScopeMessagesController> logger) : ControllerBase
{
    private readonly ILogger<ScopeMessagesController> _logger = logger;

    [HttpPost]
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

        using (_logger.BeginScope(new { CorrelationId = correlationId }))
        {
            //Processing Customer:
            _logger.LogWarning("{@LogMessage}", firstMessage);

            //Processing Invoice:
            _logger.LogWarning("{@LogMessage}", secondMessage);
        }

        return Ok();
    }
}
