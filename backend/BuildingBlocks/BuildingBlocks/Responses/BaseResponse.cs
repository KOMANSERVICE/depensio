namespace BuildingBlocks.Responses;

public record BaseResponse<T>(
    T Data,
    string Message,
    int StatusCode,
    DateTime Timestamp,
    bool Success = true
);
