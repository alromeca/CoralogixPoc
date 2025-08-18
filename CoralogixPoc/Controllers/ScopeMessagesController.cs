using CoralogixPoc.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using NLog;
using System.Reflection;
using System.Text.Json;

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

    [HttpPost]
    [Route("NestedScopesWithCorrelationUpdated")]
    public ActionResult AddNestedScopeLogsToCoralogixUpdated()
    {
        var firstMessage = new
        {
            EntityType = EntityType.Customer.GetDisplayName(),
            EntityId = 123,
            ClassName = nameof(ScopeMessagesController),
            MethodName = nameof(AddMultipleScopeLogsToCoralogix),
        };

        var secondObject = new
        {
            NuevaCol = 321456,
        };

        var objToSend = PrepareLogEntryObject(firstMessage, secondObject);

        var firstCorrelationId = Guid.NewGuid().ToString();
        using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = firstCorrelationId, ["CompanyName"] = "First Company Name" }))
        {
            //Processing Customer:
            var serialized = JsonSerializer.Serialize(objToSend, null ?? new JsonSerializerOptions { WriteIndented = false });

            _logger.LogWarning(serialized);
            //_logger.LogWarning("Procesando entidad: {EntityType}, Id: {EntityId}, Clase: {ClassName}, Método: {MethodName}",
            //    firstMessage.EntityType,
            //    firstMessage.EntityId,
            //    firstMessage.ClassName,
            //    firstMessage.MethodName);

            var secondMessage = new
            {
                EntityType = EntityType.Invoice.GetDisplayName(),
                EntityId = 453,
                ClassName = nameof(ScopeMessagesController),
                MethodName = nameof(AddNestedScopeLogsToCoralogixUpdated),
            };

            var secondCorrelationId = Guid.NewGuid().ToString();

            using (ScopeContext.PushProperty("CorrelationId", secondCorrelationId))
            {
                //Processing Invoice:
                _logger.LogWarning("{@LogMessage}", secondMessage);
                //_logger.LogWarning("Procesando entidad: {EntityType}, Id: {EntityId}, Clase: {ClassName}, Método: {MethodName}",
                //    secondMessage.EntityType,
                //    secondMessage.EntityId,
                //    secondMessage.ClassName,
                //    secondMessage.MethodName);
            }
        }

        return Ok();
    }

    [HttpPost]
    [Route("NestedScopesWithCorrelationPreserved")]
    public ActionResult AddNestedScopeLogsToCoralogixPreserved()
    {
        var firstMessage = new
        {
            EntityType = EntityType.Customer.GetDisplayName(),
            EntityId = 123,
            ClassName = nameof(ScopeMessagesController),
            MethodName = nameof(AddMultipleScopeLogsToCoralogix),
        };

        var firstCorrelationId = Guid.NewGuid().ToString();

        var callingMethod = nameof(AddNestedScopeLogsToCoralogixPreserved);

        var scopeProperties = new { CorrelationId = firstCorrelationId, CompanyName = "First Company Name" };
        using (_logger.BeginScope(new Dictionary<string, object> { [callingMethod] = scopeProperties }))
        {
            //Processing Customer:
            _logger.LogWarning("{@LogMessage}", firstMessage);

            var secondMessage = new
            {
                EntityType = EntityType.Invoice.GetDisplayName(),
                EntityId = 453,
                ClassName = nameof(ScopeMessagesController),
                MethodName = nameof(AddMultipleScopeLogsToCoralogix),
            };

            var secondCorrelationId = Guid.NewGuid().ToString();
            using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = secondCorrelationId, ["CompanyName"] = "Second Company Name" }))
            {
                //Processing Invoice:
                _logger.LogWarning("{@LogMessage}", secondMessage);
            }
        }

        return Ok();
    }


    private static object PrepareLogEntryObject(object mainObject, object secondaryObject, string message = null)
    {
        if (mainObject == null)
            throw new ArgumentNullException(nameof(mainObject));

        // Create a dynamic object to hold all properties
        var result = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

        // First, copy all properties from mainObject
        var mainProps = mainObject.GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead);

        foreach (var prop in mainProps)
        {
            result[prop.Name] = prop.GetValue(mainObject);
        }

        // Then, copy or add properties from secondaryObject
        if (secondaryObject != null)
        {
            var secondaryProps = secondaryObject.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead);

            foreach (var prop in secondaryProps)
            {
                // Add or override property regardless of whether it exists in mainObject
                result[prop.Name] = prop.GetValue(secondaryObject);
            }
        }

        // Add message info if requested
        if (!string.IsNullOrWhiteSpace(message))
        {
            result["Message"] = message;
        }

        return result;
    }
}
