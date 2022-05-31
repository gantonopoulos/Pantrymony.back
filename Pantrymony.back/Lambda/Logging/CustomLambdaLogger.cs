using Amazon.Lambda.Core;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Pantrymony.back.Lambda.Logging;

internal class CustomLambdaLogger : ILogger
{
    private readonly string _category;
    private readonly ILambdaLogger _lambdaLogger;

    public CustomLambdaLogger(string category, ILambdaLogger lambdaLogger)
    {
        _category = category;
        _lambdaLogger = lambdaLogger;
    }
    public IDisposable BeginScope<TState>(TState state)
    {
        return default!;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
            
        _lambdaLogger.LogLine($"{logLevel.ToString()} - {_category} - {formatter(state, exception)}");
    }
}