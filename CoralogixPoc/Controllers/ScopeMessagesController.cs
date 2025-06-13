using CoralogixPoc.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Text.Json;

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

        var traceId = Guid.NewGuid().ToString();

        using (_logger.BeginScope(new { TraceId = traceId }))
        {
            _logger.LogWarning($"Processing Customer: {JsonSerializer.Serialize(firstMessage)}");
            _logger.LogWarning($"Processing Invoice: {JsonSerializer.Serialize(secondMessage)}");
        }

        return Ok();
    }
}
