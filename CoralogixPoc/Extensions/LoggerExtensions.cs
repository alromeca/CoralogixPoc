using System.Text.Json;

namespace CoralogixPoc.Extensions;

public static class LoggerExtensions
{
    public static void LogMetric<T>(this ILogger<T> logger, string metricName, string value, object? properties = null)
    {
        var metric = new
        {
            Category = "Metric",
            MetricName = metricName,
            Value = value,
            Properties = properties
        };
        logger.LogInformation("[Metric] {Metric}", JsonSerializer.Serialize(metric));
    }

    /*public static void LogCritical<T>(this ILogger<T> logger, object obj)
       {
           logger.LogCritical("{@Object}", JsonSerializer.Serialize(obj));
       }

       public static void LogInformation<T>(this ILogger<T> logger, object obj)
       {
           logger.LogInformation("{@Object}", JsonSerializer.Serialize(obj));
       }

       public static void LogDebug<T>(this ILogger<T> logger, object obj)
       {
           logger.LogDebug("{@Object}", JsonSerializer.Serialize(obj));
       }

       public static void LogTrace<T>(this ILogger<T> logger, object obj)
       {
           logger.LogTrace("{@Object}", JsonSerializer.Serialize(obj));
       }

       public static void LogWarning<T>(this ILogger<T> logger, object obj)
       {
           logger.LogWarning("{@Object}", JsonSerializer.Serialize(obj));
       }*/
}
