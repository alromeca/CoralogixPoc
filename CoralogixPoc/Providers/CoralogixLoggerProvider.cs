using CoralogixCoreSDK;
using System.Collections.Concurrent;
using System.Text.Json;
using CoralogixPoc.Configurations;

namespace CoralogixPoc.Providers;

public class CoralogixLoggerProvider : ILoggerProvider
{
    private readonly CoralogixOptions _coralogixOptions;
    private static bool _configured = false;
    private static readonly object _lock = new();
    private readonly ConcurrentDictionary<string, CoralogixLoggerAdapter> _loggers = new();

    public CoralogixLoggerProvider(CoralogixOptions coralogixOptions)
    {
        _coralogixOptions = coralogixOptions ?? throw new ArgumentNullException(nameof(coralogixOptions));
        lock (_lock)
        {
            if (!_configured)
            {
                // Configure the SDK only once per process
                var logger = CoralogixLogger.GetLogger("init");
                Environment.SetEnvironmentVariable("CORALOGIX_LOG_URL", _coralogixOptions.Url);
                logger.Configure(coralogixOptions.PrivateKey, coralogixOptions.ApplicationName, coralogixOptions.SubsystemName);
                _configured = true;
            }
        }
    }

    public ILogger CreateLogger(string categoryName)
    {
        // Reuse logger adapters per category
        return _loggers.GetOrAdd(categoryName, cat => new CoralogixLoggerAdapter(cat, _coralogixOptions));
    }

    public void Dispose()
    {
        // No explicit flush or shutdown method available in Coralogix SDK.
    }
}

class CoralogixLoggerAdapter : ILogger
{
    private readonly string _category;
    private readonly LogLevel _minLevel;
    private readonly CoralogixLogger _logger;

    public CoralogixLoggerAdapter(string category, CoralogixOptions opts)
    {
        _category = category;
        _minLevel = opts.MinimumLevel;
        _logger = CoralogixLogger.GetLogger(_category);
    }

    // AsyncLocal to hold the current scope for each async context
    private readonly AsyncLocal<Scope?> _currentScope = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var parent = _currentScope.Value;
        var newScope = new Scope(state, parent, _currentScope);
        _currentScope.Value = newScope;
        return newScope;
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel >= _minLevel;

    public void Log<TState>(
        LogLevel logLevel,
        EventId id,
        TState state,
        Exception? ex,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;

        var message = formatter(state, ex);

        // Gather scope information
        var scopeInfo = GetScopeInformation();
        if (!string.IsNullOrEmpty(scopeInfo))
        {
            message = $"{scopeInfo} {message}";
        }

        var severity = logLevel switch
        {
            LogLevel.Trace or LogLevel.Debug => Severity.Debug,
            LogLevel.Information => Severity.Info,
            LogLevel.Warning => Severity.Warning,
            LogLevel.Error => Severity.Error,
            LogLevel.Critical => Severity.Critical,
            _ => Severity.Info
        };

        try
        {
            if (ex != null)
            {
                _logger.Log(severity, $"{message} - Exception: {ex}");
            }
            else
            {
                _logger.Log(
                    severity,
                    message,//JsonSerializer.Serialize(message),
                    category: _category,
                    className: nameof(CoralogixLoggerProvider),
                    methodName: nameof(Log));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Logging error: {e.Message}");
        }
    }

    // Helper to walk the scope stack and build a string
    private string? GetScopeInformation()
    {
        var scope = _currentScope.Value;
        if (scope == null) return null;

        var scopes = new List<string>();
        while (scope != null)
        {
            scopes.Add(scope.State?.ToString());
            scope = scope.Parent;
        }
        scopes.Reverse();
        return string.Join(" => ", scopes);
    }

    // Scope class to manage scope lifetime
    private class Scope : IDisposable
    {
        public object? State { get; }
        public Scope? Parent { get; }
        private readonly AsyncLocal<Scope?> _scopeRef;

        public Scope(object? state, Scope? parent, AsyncLocal<Scope?> scopeRef)
        {
            State = state;
            Parent = parent;
            _scopeRef = scopeRef;
        }

        public void Dispose()
        {
            _scopeRef.Value = Parent;
        }
    }
}
