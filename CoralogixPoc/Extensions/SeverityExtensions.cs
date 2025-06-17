using CoralogixCoreSDK;

namespace CoralogixPoc.Extensions;

public static class SeverityExtensions
{
    public static void LogBySeverity(this Severity severity, ILogger logger, object logMessage)
    {
        switch (severity)
        {
            case Severity.Debug:
                logger.LogDebug("{@LogMessage}", logMessage);
                break;
            case Severity.Verbose:
                logger.LogTrace("{@LogMessage}", logMessage);
                break;
            case Severity.Info:
                logger.LogInformation("{@LogMessage}", logMessage);
                break;
            case Severity.Warning:
                logger.LogWarning("{@LogMessage}", logMessage);
                break;
            case Severity.Error:
                logger.LogError("{@LogMessage}", logMessage);
                break;
            case Severity.Critical:
                logger.LogCritical("{@LogMessage}", logMessage);
                break;
            default:
                break;
        }
    }
}
