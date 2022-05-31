using Amazon.Lambda.Core;
using Pantrymony.back.Lambda.Logging;

namespace Pantrymony.back.Lambda.Extensions;

internal static class LoggingExtensions
{
    internal static ILogger GetCustomLogger(this ILambdaContext lambdaContext)
    {
        return new CustomLambdaLogger(lambdaContext.FunctionName, lambdaContext.Logger);
    }
}