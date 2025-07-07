namespace BuildingBlocks.Exceptions;
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message)
    { }
    public UnauthorizedException(string message, string details) : base(message)
    { }

    public string? Details { get; set; }
}

