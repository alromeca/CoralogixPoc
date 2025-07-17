using CoralogixPoc.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CoralogixPoc.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public sealed class ExceptionMessagesController(ILogger<ExceptionMessagesController> logger) : ControllerBase
{
    private readonly ILogger<ExceptionMessagesController> _logger = logger;

    [HttpPost]
    public ActionResult AddErrorLogToCoralogix(
        CompanyName companyName,
        ErrorType errorType = ErrorType.Default,
        string? customMessage = null)
    {
        try
        {
            switch (errorType)
            {
                case ErrorType.InvalidOperation:
                    throw new InvalidOperationException(customMessage ?? "Invalid operation occurred.");
                case ErrorType.Argument:
                    throw new ArgumentException(customMessage ?? "Invalid argument provided.");
                case ErrorType.NullReference:
                    throw new NullReferenceException(customMessage ?? "A null reference was encountered.");
                case ErrorType.TimeOut:
                    throw new TimeoutException(customMessage ?? "The operation timed out.");
                default:
                    throw new Exception(customMessage ?? "A generic error occurred.");
            }
        }
        catch (Exception ex)
        {
            var logObject = new
            {
                CompanyName = companyName.ToString(),
                ErrorType = errorType.ToString(),
                Message = customMessage ?? ex.Message
            };

            _logger.LogCritical("{@LogMessage}", logObject);
        }

        return Ok();
    }
}