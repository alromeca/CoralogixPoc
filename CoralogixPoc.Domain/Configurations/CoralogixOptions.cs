using Microsoft.Extensions.Logging;

namespace CoralogixPoc.Domain.Configurations;

public class CoralogixOptions
{
    public string PrivateKey { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string SubsystemName { get; set; } = string.Empty;
    public LogLevel MinimumLevel { get; set; } = LogLevel.Information;
    public string Url { get; set; } = string.Empty;
}
