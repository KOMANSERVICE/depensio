namespace depensio.Application.Exceptions;

/// <summary>
/// Exception thrown when an external service call fails.
/// This maps to HTTP 502 Bad Gateway.
/// </summary>
public class ExternalServiceException : Exception
{
    public string ServiceName { get; }

    public ExternalServiceException(string serviceName, string message)
        : base($"External service '{serviceName}' error: {message}")
    {
        ServiceName = serviceName;
    }

    public ExternalServiceException(string serviceName, string message, Exception innerException)
        : base($"External service '{serviceName}' error: {message}", innerException)
    {
        ServiceName = serviceName;
    }
}
