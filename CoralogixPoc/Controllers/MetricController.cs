using CoralogixPoc.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using CoralogixPoc.Common.Extensions;

namespace CoralogixPoc.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class MetricController(ILogger<MetricController> logger) : ControllerBase
{
    private readonly ILogger<MetricController> _logger = logger;

    [HttpPost]
    public ActionResult AddLogMetric(
        string metricName,
        string metricValue,
        EntityType entityType,
        string entityId,
        string message)
    {
        var correlationId = Guid.NewGuid().ToString();

        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
        {
            _logger.LogMetric(metricName, metricValue, new
            {
                CorrelationId = correlationId,
                EntityType = entityType,
                EntityId = entityId,
                Message = message
            });
        }

        return Ok();
    }
}
